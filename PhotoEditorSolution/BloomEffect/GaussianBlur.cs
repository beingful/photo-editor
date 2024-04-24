using PhotoEditor.Effects.Exceptions;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.Collections.Concurrent;
using System.Numerics;

namespace PhotoEditor.Effects;

internal sealed class GaussianBlur
{
    private readonly float[][] _weights;

    public GaussianBlur(int radius = 1)
    {
        _weights = new GaussianKernel(radius).Calculate();
    }

    public Image<Rgba32> Apply(Image<Rgba32> image)
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

        IEnumerable<int> columnIndexes = Enumerable.Range(0, image.Width);
        IEnumerable<int> rowIndexes = Enumerable.Range(0, image.Height);

        OrderablePartitioner<int> columnPartitioner = Partitioner.Create(columnIndexes);
        OrderablePartitioner<int> rowPartitioner = Partitioner.Create(rowIndexes);

        columnPartitioner.AsParallel().ForAll(x =>
        {
            rowPartitioner.AsParallel().ForAll(y =>
            {
                blurredImage[x, y] = ApplyToPixel(new Vector2(x, y), image);
            });
        });

        return blurredImage;
    }

    private Rgba32 ApplyToPixel(Vector2 center, Image<Rgba32> image)
    {
        (float redWeighted, float greenWeighted, float blueWeighted) = (0, 0, 0);

        int regionStartX = (int)center.X - _weights.Length / 2;
        int regionEndX = (int)center.X + _weights.Length / 2;
        int regionStartY = (int)center.Y - _weights.Length / 2;
        int regionEndY = (int)center.Y + _weights.Length / 2;

        for (int x = regionStartX, i = 0; x <= regionEndX && x < image.Width; x++, i++)
        {
            for (int y = regionStartY, k = 0; y <= regionEndY && y < image.Height; y++, k++)
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
