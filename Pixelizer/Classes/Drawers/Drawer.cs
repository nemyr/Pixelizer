using Pixelizer.Data;

namespace Pixelizer.Classes.Drawers
{
    public abstract class Drawer : IDrawer, IDisposable
    {
        public abstract int Width { get; }
        public abstract int Height { get; }

        public abstract void Dispose();
        public abstract byte[] DrawMatrix(int aspect, Color[,] pixels, Func<Color, Color> pickColor);
        public abstract Color GetPixel(int x, int y);
        public abstract void LoadImage(FileData fileData);
        public abstract void SetPixel(int x, int y, Color color);
    }
}
