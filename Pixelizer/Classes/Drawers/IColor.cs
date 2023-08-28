using Pixelizer.Data;

namespace Pixelizer.Classes.Drawers
{
    public interface IColor
    {
        public byte R { get; set; }
        public byte G { get; set; }
        public byte B { get; set; }
        public IColor AsGrayScale(GrayscaleLevels levels);
        public double GetDistance(IColor anotherColor);
    }
}
