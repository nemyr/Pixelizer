using System.Drawing;

namespace Pixelizer.Classes
{
    public class KMeansPaletteExtractor : PaletteExtractor
    {

        public float DeltaPrecision = 1f;

        private class ClusteredColor
        {
            public int R;
            public int G;
            public int B;
            public static implicit operator ClusteredColor (Color c)
            {
                return new ClusteredColor { R = c.R, G = c.G, B = c.B };
            }
            public static implicit operator Color (ClusteredColor c)
            {
                return Color.FromArgb(c.R, c.G, c.B);
            }
            public static ClusteredColor operator +(ClusteredColor c1, Color c2)
            {
                return new ClusteredColor { R = c1.R + c2.R, G = c1.G + c2.G, B = c1.B + c2.B };
            }
        }

        private class ColorCluster
        {
            public Color Color;
            public ClusteredColor NewColor;
            public int Count = 0;

            public ColorCluster()
            {
                Color = new Color();
                NewColor = new ClusteredColor();
            }
        }

        public KMeansPaletteExtractor(Bitmap bitmap) : base(bitmap)
        {
        }

        public override List<Color> GetPalette(int colorsCount)
        {
            Random rnd = new ();
            var colors = GetColorsByCount();
            var palette = new ColorCluster[colorsCount];
            for (int i = 0; i < colorsCount; i++)
                palette[i] = new ColorCluster { NewColor = colors.ElementAt(rnd.Next(colors.Count)).Key };
                //palette[i] = new ColorCluster { NewColor = Color.FromArgb(rnd.Next(0xffffff)) };

            float minDelta = 0f;
            float minDeltaOld = 0;
            float currentPrecision = 0f;
            
            Color color1 = Color.FromArgb(0xff42c6);
            var d = color1.GetDistance(color1);
            do
            {
                minDeltaOld = minDelta;

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
                    cluster.Count++;
                    cluster.NewColor += color.Key;
                }

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
