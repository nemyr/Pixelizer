using System.Drawing;

namespace Pixelizer.Classes
{
    public interface IPaletteExtractor
    {
        List<Color> GetPalette(int colorsCount);
    }
}