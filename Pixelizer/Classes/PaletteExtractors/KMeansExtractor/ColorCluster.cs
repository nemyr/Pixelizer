using Pixelizer.Classes.Drawers;

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

        public void AddColor(Action<ColorCluster> addMethod)
        {
            addMethod?.Invoke(this);
        }
    }
}
