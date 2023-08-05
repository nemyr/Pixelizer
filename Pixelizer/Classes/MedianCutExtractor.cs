using System.Drawing;

namespace Pixelizer.Classes
{
    public class MedianCutExtractor : PaletteExtractor
    {
        public MedianCutExtractor(Bitmap bitmap) : base(bitmap)
        {
        }

        public override List<Color> GetPalette(int colorsCount)
        {
            List<Color> palette = new();
            var colors = GetColors();
            for (int i = 0; i < colorsCount - 1; i++)
            {
                Dictionary<string, int> colorComponentRanges = GetColorsRange(colors);

                KeyValuePair<string, int> colorRange = GetMaxColorRange(colorComponentRanges);

                colors = colors.OrderBy(c => c.GetColorComponent(colorRange.Key)).ToList();

                var mid = colors.Count / 2;
                var median = (colors.Count % 2 != 0)
                    ? colors[mid].GetColorComponent(colorRange.Key)
                    : (colors[mid].GetColorComponent(colorRange.Key) + colors[mid - 1].GetColorComponent(colorRange.Key)) / 2;

                var aboveMedianColors = colors.TakeWhile(c => c.GetColorComponent(colorRange.Key) < median).ToList();
                var belowMedianColors = colors.Skip(aboveMedianColors.Count).ToList();
                (var colorsForPalette, colors) =
                    GetMaxColorRange(GetColorsRange(aboveMedianColors)).Value < GetMaxColorRange(GetColorsRange(belowMedianColors)).Value
                    ? (aboveMedianColors, belowMedianColors) : (belowMedianColors, aboveMedianColors);
                palette.Add(GetAverageColor(colorsForPalette));
            }
            palette.Add(GetAverageColor(colors));
            return palette;
        }

        private static KeyValuePair<string, int> GetMaxColorRange(Dictionary<string, int> colorComponentRanges)
        {
            return colorComponentRanges.OrderByDescending(c => c.Value).First();
        }
    }
}
