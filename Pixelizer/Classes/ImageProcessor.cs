using Pixelizer.Data;
using System.Drawing.Imaging;
using System.Drawing;

namespace Pixelizer.Classes
{
    public class ImageProcessor
    {
        private byte[] _resultImage;
        private FileData _fileData;
        private List<Color> _palette = new List<Color>(0);
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

        private List<Color> GetColors(Bitmap bm)
        {
            List<Color> colors = new ();
            for (int x = 0;  x < bm.Width; x++)
                for (int y = 0; y < bm.Height; y++)
                {
                    colors.Add(bm.GetPixel (x, y));
                }
            return colors.ToList();
        }

        private Color GetAverageColor(IEnumerable<Color> colors)
        {
            return Color.FromArgb(
                    (int)colors.Average(c => c.R),
                    (int)colors.Average(c => c.G),
                    (int)colors.Average(c => c.B)
                    );
        }


        private List<Color> GetPalette(int colorsCount, List<Color> colors)
        {
            List<Color> palette = new List<Color>();
            for (int i = 0; i < colorsCount - 1; i++)
            {
                Dictionary<string, int> colorComponentRanges = new Dictionary<string, int>
                {
                    { "R", colors.Max(c => c.R) - colors.Min(c => c.R) },
                    { "G", colors.Max(c => c.G) - colors.Min(c => c.G) },
                    { "B", colors.Max(c => c.B) - colors.Min(c => c.B) }
                };

                var colorRange = colorComponentRanges.Aggregate((m1, m2) => m1.Value > m2.Value ? m1 : m2);

                //var colorGroups = colors.GroupBy(c => c.GetColorComponent(colorRange.Key) > colorRange.Value/2.0);
                colors = colors.OrderBy(c => c.GetColorComponent(colorRange.Key)).ToList();
                /*
                var median = colors[colors.Count / 2].GetColorComponent(colorRange.Key);
                var colorGroups = colors.GroupBy(c => c.GetColorComponent(colorRange.Key) > median);
                var newColorPart = colorGroups.Aggregate((c1, c2) => c1.Count() < c2.Count()? c1 : c2);
                palette.Add(GetAverageColor(newColorPart));
                colors = colorGroups.Aggregate((c1, c2) => c1.Count() > c2.Count() ? c1 : c2).ToList();
                */
                var colorChunks = colors.Chunk(colors.Count / 2);
                //var newColorPart = colors.GetRange(0, colors.Count / 2);
                palette.Add(GetAverageColor(colorChunks.Last()));
                colors = colorChunks.First().ToList();
            }
            palette.Add(GetAverageColor(colors));
            return palette;

        }

        public ImageProcessor ProcessImage()
        {
            using var _bitmap = new Bitmap(Image.FromStream(_fileData.Data));
            var colors = GetColors(_bitmap);

            _palette = GetPalette(_settings.Colors, colors);

            //todo: here need to be actual processing

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
