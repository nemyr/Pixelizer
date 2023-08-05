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
    }
}
