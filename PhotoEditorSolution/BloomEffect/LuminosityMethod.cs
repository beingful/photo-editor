using SixLabors.ImageSharp.PixelFormats;

namespace PhotoEditor.Effects;

public class LuminosityMethod
{
    public float Calculate(Rgba32 pixel) =>
        0.21f * pixel.R + 0.72f * pixel.G + 0.07f * pixel.B;
}
