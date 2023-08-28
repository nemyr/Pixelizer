using Pixelizer.Classes.Drawers;
using Pixelizer.Data;

namespace Pixelizer.Classes
{
    public class ImageProcessor
    {
        private byte[] _resultImage;
        private FileData _fileData;
        private List<Color> _palette = new(0);
        public List<Color> Palette { get => _palette; }
        private ProcessingSettings _settings;
        private readonly IDrawer drawer;
        private readonly IPaletteExtractor paletteExtractor;

        public ImageProcessor(ProcessingSettings settings, IDrawer drawer, IPaletteExtractor paletteExtractor) {
            _settings = settings;
            this.drawer = drawer;
            this.paletteExtractor = paletteExtractor;
        }

        public ImageProcessor ProcessImage(FileData fileData)
        {
            _fileData = fileData;
            drawer.LoadImage(fileData);
            if (_settings.UseGrayscale)
                BitmapToGrayscale();
            _palette.Clear();
            int pixelsPerWidth = drawer.Width / _settings.Width;
            int pixelsPerHeight = drawer.Height / _settings.Height;

            var aspect = Math.Min(pixelsPerWidth, pixelsPerHeight);
            _settings.Width = drawer.Width / aspect;
            _settings.Height = drawer.Height / aspect;

            Color[,] resultMatrix = new Color[_settings.Width, _settings.Height];

            //todo: here need to be actual processing

            List<Color> colorsInPixel = new(aspect * aspect);

            for (var rx = 0; rx < _settings.Width; rx++)
                for (var ry = 0; ry < _settings.Height; ry++)
                {
                    for (var x = rx * aspect; x < (rx * aspect + aspect); x++)
                        for (var y = ry * aspect; y < (ry * aspect + aspect); y++)
                            colorsInPixel.Add(drawer.GetPixel(x, y));
                    resultMatrix[rx, ry] = PaletteExtractor.GetAverageColor(colorsInPixel);
                    colorsInPixel.Clear();
                }
            this._resultImage = GetResultImage(aspect, resultMatrix);
            return this;
        }

        private byte[] GetResultImage(int aspect, Color[,] pixels)
        {
            Func<Color, Color> pickColor;
            if (_settings.UsePalette)
            {
                _palette = paletteExtractor.GetPalette(_settings.Colors);
                pickColor = (col) => { return _palette.MinBy(c => c.GetDistance(col)); };
            } 
            else
                pickColor = (col) => { return col; };

            return drawer.DrawMatrix(aspect, pixels, pickColor);
        }

        private void BitmapToGrayscale()
        {
            for ( int x = 0; x < drawer.Width; x++ )
                for (int y = 0; y < drawer.Height; y++ )
                    drawer.SetPixel(x, y, drawer.GetPixel(x,y).AsGrayScale(_settings.GrayscaleLevels));
        }

        public byte[] AsArray()
        {
            return _resultImage;
        }

        public string AsBase64()
        {
            string result = Convert.ToBase64String(_resultImage);
            return $"data:image/{ Path.GetExtension(_fileData.FileName).Replace(".", "")};base64,{result}";
        }

    }
}
