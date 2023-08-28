using Pixelizer.Data;

namespace Pixelizer.Classes.Drawers
{
    public class Color
    {
        public byte R { get; set; }
        public byte G { get; set; }
        public byte B { get; set; }

        public Color AsGrayScale(GrayscaleLevels levels)
        {
            byte value = (byte)(R * levels.Red + G * levels.Green + B * levels.Blue);
            return FromRGB(value, value, value);
        }

        public static Color FromRGB(byte r, byte g, byte b)
        {
            return new Color() { R = r, G = g, B = b };
        }
        public double GetDistance(Color anotherColor)
        {
            return Math.Sqrt(
                Math.Pow(R - anotherColor.R, 2) +
                Math.Pow(G - anotherColor.G, 2) +
                Math.Pow(B - anotherColor.B, 2));
        }
    }
}
