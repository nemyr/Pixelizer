using System.Drawing;

namespace Pixelizer.Classes.PaletteExtractors.KMeansExtractor
{
    public class ColorCluster
    {
        public Color Color;
        public ClusteredColor NewColor;
        public int Count = 0;

        public ColorCluster()
        {
            Color = new Color();
            NewColor = new ClusteredColor();
        }

        public delegate void ColorAddDelegate(ColorCluster cluster, KeyValuePair<Color, int> color);

        public void AddColor(KeyValuePair<Color, int> color, ColorAddDelegate d)
        {
            d?.Invoke(this, color);
        }
    }
}
