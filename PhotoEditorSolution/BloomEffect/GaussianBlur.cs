using PhotoEditor.Effects.Exceptions;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.Collections.Concurrent;
using System.Numerics;

namespace PhotoEditor.Effects;

internal sealed class GaussianBlur
{
    private readonly float[] _kernelBasis;

    public GaussianBlur(int radius = 1)
    {
        _kernelBasis = new GaussianKernel(radius).CalculateBasis();
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
        IEnumerable<int> columnIndexes = Enumerable.Range(0, image.Width);
        IEnumerable<int> rowIndexes = Enumerable.Range(0, image.Height);

        OrderablePartitioner<int> columnPartitioner = Partitioner.Create(columnIndexes);
        OrderablePartitioner<int> rowPartitioner = Partitioner.Create(rowIndexes);

        Vector3[,] verticallyBlurredImage = new Vector3[image.Width, image.Height];

        columnPartitioner.AsParallel().ForAll(x =>
        {
            rowPartitioner.AsParallel().ForAll(y =>
            {
                verticallyBlurredImage[x, y] = ApplyVertically(new Vector2(x, y), image);
            });
        });

        Image<Rgba32> blurredImageResult = new(image.Width, image.Height);

        columnPartitioner.AsParallel().ForAll(x =>
        {
            rowPartitioner.AsParallel().ForAll(y =>
            {
                blurredImageResult[x, y] = ApplyHorizontally(new Vector2(x, y), verticallyBlurredImage);
            });
        });

        return blurredImageResult;
    }

    private Vector3 ApplyVertically(Vector2 center, Image<Rgba32> image)
    {
        (float redWeighted, float greenWeighted, float blueWeighted) = (0, 0, 0);

        int pixelNeighbouhoodStart = (int)center.Y - (_kernelBasis.Length / 2);
        int pixelNeighbouhoodEnd = (int)center.Y + (_kernelBasis.Length / 2);

        int leftBound = Math.Max(pixelNeighbouhoodStart, 0);
        int rightBound = Math.Min(pixelNeighbouhoodEnd, image.Height - 1);

        int kernelStart = leftBound - pixelNeighbouhoodStart;

        for (int y = leftBound, i = kernelStart; y <= rightBound; y++, i++)
        {
            Rgba32 pixel = image[(int)center.X, y];
            float weight = _kernelBasis[i];

            redWeighted += pixel.R * weight;
            greenWeighted += pixel.G * weight;
            blueWeighted += pixel.B * weight;
        }

        return new Vector3(x: redWeighted, y: greenWeighted, z: blueWeighted);
    }

    private Rgba32 ApplyHorizontally(Vector2 center, Vector3[,] image)
    {
        Vector3 bluredPixel = new();

        int pixelNeighbouhoodStart = (int)center.X - (_kernelBasis.Length / 2);
        int pixelNeighbouhoodEnd = (int)center.X + (_kernelBasis.Length / 2);

        int leftBound = Math.Max(pixelNeighbouhoodStart, 0);
        int rightBound = Math.Min(pixelNeighbouhoodEnd, image.GetLength(0) - 1);

        int kernelStart = leftBound - pixelNeighbouhoodStart;

        for (int x = leftBound, i = kernelStart; x <= rightBound; x++, i++)
        {
            Vector3 pixel = image[x, (int)center.Y];
            float weight = _kernelBasis[i];

            bluredPixel.X += pixel.X * weight;
            bluredPixel.Y += pixel.Y * weight;
            bluredPixel.Z += pixel.Z * weight;
        }

       return new Rgba32(
           r: (byte)Math.Round(bluredPixel.X),
           g: (byte)Math.Round(bluredPixel.Y),
           b: (byte)Math.Round(bluredPixel.Z));
    }

    public bool MatchesWithGaussianMatrix(Image image) =>
        image.Width >= _kernelBasis.Length && image.Height >= _kernelBasis.Length;
}
