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
    }
}
