using Pixelizer.Data;
using System.Drawing.Imaging;
using System.Drawing;

namespace Pixelizer.Classes
{
    public class ImageProcessor
    {
        private byte[] _resultImage;
        private Bitmap _bitmap;
        private FileData _fileData;

        private ProcessingSettings _settings;

        public ImageProcessor(ProcessingSettings settings, FileData fileData) { 
            _settings = settings;
            _fileData = fileData;
            _bitmap = new Bitmap(Image.FromStream(fileData.Data));
            /*
            using var graphics = Graphics.FromImage(bm);

            graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;
            graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Bicubic;
            graphics.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
            */
        }

        public ImageProcessor ProcessImage()
        {
            for (int x = 0; x < _bitmap.Width; x++)
                for (int y = 0; y < _bitmap.Height; y++)
                {
                    var px = _bitmap.GetPixel(x, y);
                    var t = System.Drawing.Color.FromArgb(
                        (int)(_settings.GrayscaleLevels.Red * px.R),
                        (int)(_settings.GrayscaleLevels.Green * px.G),
                        (int)(_settings.GrayscaleLevels.Blue * px.B));
                    _bitmap.SetPixel(x, y, t);
                }
            //      graphics.DrawRectangle(p, 1, 1, 100, 100);

            using var output = new MemoryStream();

            var qualityParamId = Encoder.Quality;
            var encoderParameters = new EncoderParameters(1);
            encoderParameters.Param[0] = new EncoderParameter(qualityParamId, _settings.Quality);

            var codec = ImageCodecInfo.GetImageDecoders()
                .FirstOrDefault(codec => codec.FormatID == ImageFormat.Jpeg.Guid);

            _bitmap.Save(output, codec, encoderParameters);

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
