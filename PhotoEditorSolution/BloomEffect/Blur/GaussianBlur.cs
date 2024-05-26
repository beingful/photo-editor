using PhotoEditor.Effects.Exceptions;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.Collections.Concurrent;
using System.Numerics;

namespace PhotoEditor.Effects.Blur;

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

    public Image<Rgba32> Adjust(Image<Rgba32> image, Image<Rgba32> blurredImage, Image<Rgba32> thresholdedImage)
    {
        if (MatchesWithGaussianMatrix(image) == false)
        {
            throw new ArgumentException(ErrorMessages.ImageDoesNotMatchWithMatrix);
        }

        return AdjustForImage(image, blurredImage, thresholdedImage);
    }

    public bool MatchesWithGaussianMatrix(Image image) =>
        image.Width >= _kernelBasis.Length && image.Height >= _kernelBasis.Length;

    private Image<Rgba32> ApplyToImage(Image<Rgba32> image)
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

        int pixelNeighbouhoodStart = (int)center.Y - _kernelBasis.Length / 2;
        int pixelNeighbouhoodEnd = (int)center.Y + _kernelBasis.Length / 2;

        //int leftBound = Math.Max(pixelNeighbouhoodStart, 0);
        //int rightBound = Math.Min(pixelNeighbouhoodEnd, image.Height - 1);

        //int kernelStart = leftBound - pixelNeighbouhoodStart;

        for (int y = pixelNeighbouhoodStart, i = 0; y <= pixelNeighbouhoodEnd; y++, i++)
        {
            float weight = _kernelBasis[i];

            Rgba32 pixel = y >= 0 && y < image.Height
                ? image[(int)center.X, y]
                : image[(int)center.X, (int)center.Y];

            redWeighted += pixel.R * weight;
            greenWeighted += pixel.G * weight;
            blueWeighted += pixel.B * weight;
        }

        return new Vector3(x: redWeighted, y: greenWeighted, z: blueWeighted);
    }

    private Rgba32 ApplyHorizontally(Vector2 center, Vector3[,] image)
    {
        Vector3 bluredPixel = new();

        int pixelNeighbouhoodStart = (int)center.X - _kernelBasis.Length / 2;
        int pixelNeighbouhoodEnd = (int)center.X + _kernelBasis.Length / 2;

        //int leftBound = Math.Max(pixelNeighbouhoodStart, 0);
        //int rightBound = Math.Min(pixelNeighbouhoodEnd, image.GetLength(0) - 1);

        //int kernelStart = leftBound - pixelNeighbouhoodStart;

        for (int x = pixelNeighbouhoodStart, i = 0; x <= pixelNeighbouhoodEnd; x++, i++)
        {
            float weight = _kernelBasis[i];

            Vector3 pixel = x >= 0 && x < image.GetLength(0)
                ? image[x, (int)center.Y]
                : image[(int)center.X, (int)center.Y];

            bluredPixel.X += pixel.X * weight;
            bluredPixel.Y += pixel.Y * weight;
            bluredPixel.Z += pixel.Z * weight;
        }

        return new Rgba32(
            r: (byte)Math.Round(bluredPixel.X),
            g: (byte)Math.Round(bluredPixel.Y),
            b: (byte)Math.Round(bluredPixel.Z));
    }

    private Image<Rgba32> AdjustForImage(Image<Rgba32> image, Image<Rgba32> blurredImage, Image<Rgba32> thresholdedImage)
    {
        Image<Rgba32> adjustedBlurredImage = new(image.Width, image.Height);

        IEnumerable<int> columnIndexes = Enumerable.Range(0, image.Width);
        IEnumerable<int> rowIndexes = Enumerable.Range(0, image.Height);

        OrderablePartitioner<int> columnPartitioner = Partitioner.Create(columnIndexes);
        OrderablePartitioner<int> rowPartitioner = Partitioner.Create(rowIndexes);

        columnPartitioner.AsParallel().ForAll(x =>
        {
            rowPartitioner.AsParallel().ForAll(y =>
            {
                adjustedBlurredImage[x, y] = AdjustForPixel(image[x, y], blurredImage[x, y], thresholdedImage[x, y]);
            });
        });

        return adjustedBlurredImage;
    }

    private Rgba32 AdjustForPixel(Rgba32 originalPixel, Rgba32 blurredPixel, Rgba32 thresholdedPixel)
    {
        if (thresholdedPixel.R == 255)
        {

        }
        else if (thresholdedPixel.R < 255 && thresholdedPixel.R != 0)
        {

        }
        byte originalPixelMultiplier = (byte)Math.Max(1 - thresholdedPixel.R, 0);
        byte blurredPixelMultiplier = (byte)Math.Max(thresholdedPixel.R - byte.MaxValue + 1, 0);

        return new Rgba32(
            r: blurredPixelMultiplier * blurredPixel.R + originalPixelMultiplier * originalPixel.R,
            g: blurredPixelMultiplier * blurredPixel.G + originalPixelMultiplier * originalPixel.G,
            b: blurredPixelMultiplier * blurredPixel.B + originalPixelMultiplier * originalPixel.B);
    }
}
