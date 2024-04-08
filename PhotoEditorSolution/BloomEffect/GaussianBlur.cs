using PhotoEditor.Effects;
using PhotoEditor.Effects.Exceptions;
using PhotoEditor.Effects.Models;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace BloomEffect;

public sealed class GaussianBlur
{
    private float[][] _weights;

    public GaussianBlur(int radius = 1)
    {
        _weights = new GaussianKernel(radius).Calculate();
    }

    public Image<Rgba32> ApplyAsync(Image<Rgba32> image)
    {
        if (MatchesWithGaussianMatrix(image) == false)
        {
            throw new ArgumentException(ErrorMessages.ImageDoesNotMatchWithMatrix);
        }

        return ApplyToImage(image);
    }

    public Image<Rgba32> ApplyToImage(Image<Rgba32> image)
    {
        Image<Rgba32> blurredImage = new(image.Width, image.Height);

        for (int x = 0; x < image.Width; x++)
        {
            for (int y  = 0; y < image.Height; y++)
            {
                blurredImage[x, y] = ApplyToPixel(target: new Position(x, y), image);
            }
        }

        return blurredImage;
    }

    private Rgba32 ApplyToPixel(Position target, Image<Rgba32> image)
    {
        (float redWeighted, float greenWeighted, float blueWeighted) = (0, 0, 0);

        for (int x = target.X - _weights.Length / 2, i = 0; x <= target.X + _weights.Length / 2 && x < image.Width; x++, i++)
        {
            for (int y = target.Y - _weights.Length / 2, k = 0; y <= target.Y + _weights.Length / 2 && y < image.Height; y++, k++)
            {
                if (x >= 0 && y >= 0)
                {
                    float weight = _weights[k][i];
                    Rgba32 pixel = image[x, y];

                    redWeighted += pixel.R * weight;
                    greenWeighted += pixel.G * weight;
                    blueWeighted += pixel.B * weight;
                }
            }
        }

        return new Rgba32(r: (byte)redWeighted, g: (byte)greenWeighted, b: (byte)blueWeighted);
    }

    public bool MatchesWithGaussianMatrix(Image image) =>
        image.Width >= _weights.Length && image.Height >= _weights.Length;
}
