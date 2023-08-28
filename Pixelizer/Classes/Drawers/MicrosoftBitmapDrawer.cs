using Pixelizer.Data;
using System.Drawing;
using System.Drawing.Imaging;

namespace Pixelizer.Classes.Drawers
{
    public class MicrosoftBitmapDrawer : Drawer
    {
        public override int Width { get => _bitmap.Width; }
        public override int Height { get => _bitmap.Height; }

        private Bitmap _bitmap;

        public override void Dispose()
        {
            _bitmap?.Dispose();
        }

        public override Color GetPixel(int x, int y)
        {
            var c = _bitmap.GetPixel(x, y);
            return Color.FromRGB(c.R, c.G, c.B);
        }

        public override void LoadImage(FileData fileData)
        {
            _bitmap = new Bitmap(Image.FromStream(fileData.Data));
        }

        public override void SetPixel(int x, int y, Color color)
        {
            _bitmap.SetPixel(x, y, System.Drawing.Color.FromArgb(color.R, color.G, color.B));
        }

        public override byte[] DrawMatrix(int aspect, Color[,] pixels, Func<Color, Color> pickColor)
        {
            int width = pixels.GetLength(0);
            int height = pixels.GetLength(1);
            using var resultBitmap = new Bitmap(aspect * width, aspect * height);

            using var graphics = Graphics.FromImage(resultBitmap);

            graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;
            graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Bicubic;
            graphics.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;

            Brush brush = new SolidBrush(System.Drawing.Color.Black);
            Pen pen = new Pen(brush, aspect);
            for (var rx = 0; rx < width; rx++)
                for (var ry = 0; ry < height; ry++)
                {
                    var tColor = pickColor(pixels[rx, ry]);
                    pen.Color = System.Drawing.Color.FromArgb(tColor.R, tColor.G, tColor.B);
                    graphics.DrawRectangle(pen, rx * aspect, ry * aspect, aspect, aspect);
                }

            using var output = new MemoryStream();
            resultBitmap.Save(output, ImageFormat.Png);
            return output.ToArray();
        }
    }
}
