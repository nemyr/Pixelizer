using System.Drawing;

namespace Pixelizer.Classes.PaletteExtractors.KMeansExtractor
{
    public class ClusteredColor
    {
        public int R;
        public int G;
        public int B;
        public static implicit operator ClusteredColor(Color c)
        {
            return new ClusteredColor { R = c.R, G = c.G, B = c.B };
        }
        public static implicit operator Color(ClusteredColor c)
        {
            return Color.FromArgb(c.R, c.G, c.B);
        }
        public static ClusteredColor operator +(ClusteredColor c1, Color c2)
        {
            return new ClusteredColor { R = c1.R + c2.R, G = c1.G + c2.G, B = c1.B + c2.B };
        }
    }
}
