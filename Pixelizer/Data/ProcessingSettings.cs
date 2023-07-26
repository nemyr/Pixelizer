namespace Pixelizer.Data
{
    public class ProcessingSettings
    {
        public int Quality { get; set; } = 100;
        public int Width { get; set; }
        public int Height { get; set; }
        public bool KeepAspectRatio { get; set; }
        public int Colors { get; set; }
        public GrayscaleLevels GrayscaleLevels { get; set; } = new GrayscaleLevels();
    }
}
