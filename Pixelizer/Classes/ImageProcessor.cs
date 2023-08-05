using Pixelizer.Data;
using System.Drawing.Imaging;
using System.Drawing;

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
            /*
            using var graphics = Graphics.FromImage(bm);
            graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;
            graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Bicubic;
            graphics.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
            */
        }
        
        public ImageProcessor ProcessImage()
        {
            using var _bitmap = new Bitmap(Image.FromStream(_fileData.Data));
            PaletteExtractor extractor = new KMeansPaletteExtractor(_bitmap);
            _palette = extractor.GetPalette(_settings.Colors);
            int pixelsPerWidth = _bitmap.Width / _settings.Width;
            int pixelsPerHeight = _bitmap.Height / _settings.Height;
            
            using var resultBitmap = new Bitmap(pixelsPerWidth * _settings.Width, pixelsPerHeight* _settings.Height );
            using var graphics = Graphics.FromImage(resultBitmap);
            graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;
            graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Bicubic;
            graphics.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
            
            //todo: here need to be actual processing

            for (var rx = 0 ; rx < _settings.Width; rx++)
                for (var ry = 0 ; ry < _settings.Height; ry++)
                {
                    List<Color> colorsInPixel = new List<Color>();
                    for(var x = rx * pixelsPerWidth ; x < rx * pixelsPerWidth + pixelsPerWidth; x++)
                        for (var y = ry * pixelsPerHeight ; y < ry * pixelsPerHeight + pixelsPerHeight; y++)
                        {
                            colorsInPixel.Add(_bitmap.GetPixel(x, y));
                        }
                    var pixelColor = PaletteExtractor.GetAverageColor(colorsInPixel);

                    Brush brush = new SolidBrush(pixelColor);
                    Pen pen = new Pen(brush, Math.Max(pixelsPerWidth, pixelsPerHeight));
                    graphics.DrawRectangle(pen, rx * pixelsPerWidth, ry * pixelsPerHeight, pixelsPerWidth, pixelsPerHeight);
                }

            /*
            for (int x = 0; x < _bitmap.Width; x++)
                for (int y = 0; y < _bitmap.Height; y++)
                {
                    var px = _bitmap.GetPixel(x, y);
                    var t = System.Drawing.Color.FromArgb(
                        (int)(_settings.GrayscaleLevels.Red * px.R),
                        (int)(_settings.GrayscaleLevels.Green * px.G),
                        (int)(_settings.GrayscaleLevels.Blue * px.B));
                    _bitmap.SetPixel(x, y, t);
                    t.Equals (t);
                }
            //      graphics.DrawRectangle(p, 1, 1, 100, 100);
            */
            using var output = new MemoryStream();

            var qualityParamId = Encoder.Quality;
            var encoderParameters = new EncoderParameters(1);
            encoderParameters.Param[0] = new EncoderParameter(qualityParamId, _settings.Quality);

            var codec = ImageCodecInfo.GetImageDecoders()
                .FirstOrDefault(codec => codec.FormatID == ImageFormat.Jpeg.Guid);

            resultBitmap.Save(output, codec, encoderParameters);

            _resultImage = output.ToArray();
            return this;
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
