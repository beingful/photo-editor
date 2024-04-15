using SixLabors.ImageSharp.PixelFormats;

namespace PhotoEditor.Effects;

public class ScreenBlending
{
    public Rgba32 Apply(Rgba32 firstPixel, Rgba32 secondPixel) =>
        new(
            r: ApplyToColor(firstPixel.R, secondPixel.R),
            g: ApplyToColor(firstPixel.G, secondPixel.G),
            b: ApplyToColor(firstPixel.B, secondPixel.B));

    private byte ApplyToColor(byte firstColor, byte secondColor) =>
        (byte)(255 - ((255 - firstColor) * (255 - secondColor) / 255));
}
