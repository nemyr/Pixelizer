using Pixelizer.Data;

namespace Pixelizer.Classes.Drawers
{
    public interface IDrawer
    {
        public int Width { get; }
        public int Height { get; }
        public Color GetPixel(int x, int y);
        public void SetPixel(int x, int y, Color color);
        public void LoadImage(FileData fileData);
        public byte[] DrawMatrix(int aspect, Color[,] pixels, Func<Color, Color> pickColor);
    }
}
