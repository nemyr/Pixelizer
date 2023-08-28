using Pixelizer.Classes.Drawers;

namespace Pixelizer.Classes
{
    public abstract class PaletteExtractor : IPaletteExtractor
    {
        protected readonly IDrawer bitmap;

        public PaletteExtractor(IDrawer bitmap)
        {
            this.bitmap = bitmap;
        }

        public abstract List<Color> GetPalette(int colorsCount);

        public static Dictionary<string, int> GetColorsRange(List<Color> colors)
        {
            return new Dictionary<string, int>
                {
                    { "R", (int) Math.Round((double)(colors.Max(c => c.R) - colors.Min(c => c.R)))},
                    { "G", (int) Math.Round((double)(colors.Max(c => c.G) - colors.Min(c => c.G)))},
                    { "B", (int) Math.Round((double)(colors.Max(c => c.B) - colors.Min(c => c.B)))}
                };
        }


        protected List<Color> GetColors()
        {
            List<Color> colors = new();
            for (int x = 0; x < bitmap.Width; x++)
                for (int y = 0; y < bitmap.Height; y++)
                {
                    colors.Add(bitmap.GetPixel(x, y));
                }
            return colors.ToList();
        }

        protected void GetColors(ref ICollection<Color> colors)
        {
            for (int x = 0; x < bitmap.Width; x++)
                for (int y = 0; y < bitmap.Height; y++)
                    colors.Add(bitmap.GetPixel(x, y));
        }

        protected Dictionary<Color, int> GetColorsByCount()
        {
            var colors = GetColors();
            return colors.GroupBy(c => c).ToDictionary(k => k.Key, v => v.Count());
        }

        public static Color GetAverageColor(IEnumerable<Color> colors)
        {
            return Color.FromRGB(
                    (byte)colors.Average(c => c.R),
                    (byte)colors.Average(c => c.G),
                    (byte)colors.Average(c => c.B)
                    );
        }
    }
}
