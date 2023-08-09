namespace Pixelizer.Data
{
    public class ProcessingSettings
    {
        public int Quality { get; set; } = 100;
        public int Width { get; set; } = 10;
        public int Height { get; set; } = 10;
        public bool KeepAspectRatio { get; set; }
        public int Colors { get; set; } = 5;
        public bool UseGrayscale { get; set; } = false;
        public bool UsePalette { get; set; } = true;
        public GrayscaleLevels GrayscaleLevels { get; set; } = new GrayscaleLevels();
    }
}
