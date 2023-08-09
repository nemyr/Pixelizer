using Pixelizer.Data;
using System.Drawing;

namespace Pixelizer.Classes
{
    public static class ColorExtention
    {
        public static int GetColorComponent(this Color c, string componentName)
        {
            return componentName.ToUpper() switch
            {
                "R" => c.R,
                "G" => c.G,
                "B" => c.B,
                _ => throw new NotSupportedException()
            };
        }

        public static double GetDistance(this Color c, Color anotherColor)
        {
            return Math.Sqrt(
                Math.Pow(c.R - anotherColor.R, 2) + 
                Math.Pow(c.G - anotherColor.G, 2) + 
                Math.Pow(c.B - anotherColor.B, 2));
        }

        public static Color AsGrayScale(this Color c, GrayscaleLevels levels) {
            int value = (int)(c.R * levels.Red + c.G * levels.Green + c.B * levels.Blue);
            return Color.FromArgb(value, value, value);
        }
    }
}
