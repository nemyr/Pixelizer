using Pixelizer.Classes.PaletteExtractors.KMeansExtractor;
using Pixelizer.Data;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Imaging;

namespace Pixelizer.Classes
{
    public class ImageProcessor
    {
        private byte[] _resultImage;
        private FileData _fileData;
        private List<Color> _palette = new(0);
        public List<Color> Palette { get => _palette; }
        private ProcessingSettings _settings;

        public ImageProcessor(ProcessingSettings settings, FileData fileData) {
            _settings = settings;
            _fileData = fileData;
        }

        public ImageProcessor ProcessImage()
        {
            using var _bitmap = new Bitmap(Image.FromStream(_fileData.Data));
            if (_settings.UseGrayscale)
                BitmapToGrayscale(_bitmap);
            _palette.Clear();
            int pixelsPerWidth = _bitmap.Width / _settings.Width;
            int pixelsPerHeight = _bitmap.Height / _settings.Height;

            var aspect = Math.Min(pixelsPerWidth, pixelsPerHeight);
            _settings.Width = _bitmap.Width / aspect;
            _settings.Height = _bitmap.Height / aspect;

            Color[,] resultMatrix = new Color[_settings.Width, _settings.Height];

            //todo: here need to be actual processing

            List<Color> colorsInPixel = new(aspect * aspect);

            for (var rx = 0; rx < _settings.Width; rx++)
                for (var ry = 0; ry < _settings.Height; ry++)
                {
                    for (var x = rx * aspect; x < (rx * aspect + aspect); x++)
                        for (var y = ry * aspect; y < (ry * aspect + aspect); y++)
                            colorsInPixel.Add(_bitmap.GetPixel(x, y));
                    resultMatrix[rx, ry] = PaletteExtractor.GetAverageColor(colorsInPixel);
                    colorsInPixel.Clear();
                }
            this._resultImage = GetResultImage(aspect, resultMatrix, _bitmap);
            return this;
        }

        private byte[] GetResultImage(int aspect, Color[,] pixels, Bitmap bitmap)
        {
            using var resultBitmap = new Bitmap(aspect * _settings.Width, aspect * _settings.Height);
            using var graphics = Graphics.FromImage(resultBitmap);

            IPaletteExtractor extractor = new KMeansPaletteExtractor(bitmap);

            Func<Color, Color> pickColor;
            if (_settings.UsePalette)
            {
                _palette = extractor.GetPalette(_settings.Colors);
                pickColor = (col) => { return _palette.MinBy(c => c.GetDistance(col)); };
            } 
            else
                pickColor = (col) => { return col; };

            graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;
            graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Bicubic;
            graphics.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;

            Brush brush = new SolidBrush(Color.Black);
            Pen pen = new Pen(brush, aspect);
            for (var rx = 0; rx < _settings.Width; rx++)
                for (var ry = 0; ry < _settings.Height; ry++)
                {
                    pen.Color = pickColor(pixels[rx, ry]);
                    graphics.DrawRectangle(pen, rx * aspect, ry * aspect, aspect, aspect);
                }

            using var output = new MemoryStream();
            var qualityParamId = Encoder.Quality;
            var encoderParameters = new EncoderParameters(1);
            encoderParameters.Param[0] = new EncoderParameter(qualityParamId, _settings.Quality);

            var codec = ImageCodecInfo.GetImageDecoders()
                .FirstOrDefault(codec => codec.FormatID == ImageFormat.Jpeg.Guid);

            resultBitmap.Save(output, codec, encoderParameters);

            return output.ToArray();
        }

        private void BitmapToGrayscale(Bitmap bitmap)
        {
            for ( int x = 0; x < bitmap.Width; x++ )
                for (int y = 0; y < bitmap.Height; y++ )
                    bitmap.SetPixel(x, y, bitmap.GetPixel(x,y).AsGrayScale(_settings.GrayscaleLevels));
        }


        public byte[] AsArray()
        {
            return _resultImage;
        }

        public string AsBase64()
        {
            string result = Convert.ToBase64String(_resultImage);
            return $"data:image/{ Path.GetExtension(_fileData.FileName).Replace(".", "")};base64,{result}";
        }

    }
}
