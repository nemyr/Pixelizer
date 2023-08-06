using System.Drawing;

namespace Pixelizer.Classes.PaletteExtractors.KMeansExtractor
{
    public class KMeansPaletteExtractor : PaletteExtractor
    {

        public float DeltaPrecision = .01f;
        public bool UseColorsWeights = false;

        private void AddColorWithWeight(ColorCluster cluster, KeyValuePair<Color, int> color)
        {
            cluster.Count += color.Value;
            cluster.NewColor.R += color.Key.R * color.Value;
            cluster.NewColor.G += color.Key.G * color.Value;
            cluster.NewColor.B += color.Key.B * color.Value;
        }

        private void AddColorWithoutWeight(ColorCluster cluster, KeyValuePair<Color, int> color)
        {
            cluster.Count++;
            cluster.NewColor += color.Key;
        }

        public KMeansPaletteExtractor(Bitmap bitmap) : base(bitmap)
        {
        }

        public override List<Color> GetPalette(int colorsCount)
        {
            Random rnd = new();
            var colors = GetColorsByCount();
            var palette = new ColorCluster[colorsCount];

            for (int i = 0; i < colorsCount; i++)
                palette[i] = new ColorCluster { NewColor = colors.ElementAt(rnd.Next(colors.Count)).Key };

            float minDelta = 0f;
            float minDeltaOld = 0f;
            float currentPrecision = 0f;

            ColorCluster.ColorAddDelegate ColorSumMethod = UseColorsWeights ? AddColorWithWeight : AddColorWithoutWeight;

            do
            {
                for (var i = 0; i < palette.Length; i++)
                {
                    palette[i].Count = 0;
                    palette[i].Color = palette[i].NewColor;
                    palette[i].NewColor = Color.Black;
                }

                foreach (var color in colors)
                {
                    var cluster = palette.MinBy(cluster => color.Key.GetDistance(cluster.Color));
                    if (cluster == null)
                        continue;
                    cluster.AddColor(color, ColorSumMethod);
                }

                minDeltaOld = minDelta;
                minDelta = 0;

                for (var i = 0; i < palette.Length; i++)
                {
                    var count = palette[i].Count;
                    if (count == 0)
                        palette[i].NewColor = palette[i].Color;
                    else
                    {
                        palette[i].NewColor.R /= count;
                        palette[i].NewColor.G /= count;
                        palette[i].NewColor.B /= count;
                    }
                    var delta = palette[i].Color.GetDistance(palette[i].NewColor);
                    if (delta > minDelta)
                        minDelta = (float)delta;
                }

                currentPrecision = Math.Abs(minDelta - minDeltaOld);

            } while (currentPrecision > DeltaPrecision);

            return palette.Select(c => c.Color).ToList();
        }
    }
}
