using Pixelizer.Classes.Drawers;

namespace Pixelizer.Classes
{
    public interface IPaletteExtractor
    {
        List<Color> GetPalette(int colorsCount);
    }
}