using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp;
using System.Collections.Concurrent;
using System.Numerics;

namespace PhotoEditor.Effects;

internal sealed class BilinearSampling
{
    private readonly int _newWidth;
    private readonly int _newHeight;

    public BilinearSampling(int newWidth, int newHeight)
    {
        _newWidth = newWidth;
        _newHeight = newHeight;
    }

    public Image<Rgba32> Apply(Image<Rgba32> image)
    {
        Image<Rgba32> resizedImage = new(_newWidth, _newHeight);

        IEnumerable<int> columnIndexes = Enumerable.Range(0, resizedImage.Width);
        IEnumerable<int> rowIndexes = Enumerable.Range(0, resizedImage.Height);

        OrderablePartitioner<int> columnPartitioner = Partitioner.Create(columnIndexes);
        OrderablePartitioner<int> rowPartitioner = Partitioner.Create(rowIndexes);

        float xScale = (float)_newWidth / image.Width;
        float yScale = (float)_newHeight / image.Height;

        columnPartitioner.AsParallel().ForAll(x =>
        {
            rowPartitioner.AsParallel().ForAll(y =>
            {
                resizedImage[x, y] = InterpolatePixel(new Vector2(x, y), xScale, yScale, image);
            });
        });

        return resizedImage;
    }

    private Rgba32 InterpolatePixel(Vector2 resizedPosition, float xScale, float yScale, Image<Rgba32> image)
    {
        Vector2 originalPixelPosition = new(x: resizedPosition.X / xScale, y: resizedPosition.Y / yScale);

        int xLeft = (int)Math.Floor(originalPixelPosition.X);
        int yTop = (int)Math.Floor(originalPixelPosition.Y);
        int xRight = Math.Min(xLeft + 1, image.Width - 1);
        int yBottom = Math.Min(yTop + 1, image.Height - 1);

        Rgba32 topLeftPixel = image[xLeft, yTop];
        Rgba32 topRightPixel = image[xRight, yTop];
        Rgba32 bottomLeftPixel = image[xLeft, yBottom];
        Rgba32 bottomRightPixel = image[xRight, yBottom];

        Vector3 topTwoPixelInterpolationResult;
        Vector3 bottomTwoPixelInterpolationResult;

        if (xLeft == xRight)
        {
            topTwoPixelInterpolationResult = AveragePixel(topLeftPixel, topRightPixel);
            bottomTwoPixelInterpolationResult = AveragePixel(bottomLeftPixel, bottomRightPixel);
        }
        else
        {
            topTwoPixelInterpolationResult = ApplyHorizontalInterpolation(topLeftPixel, topRightPixel, xLeft, xRight, originalPixelPosition.X);
            bottomTwoPixelInterpolationResult = ApplyHorizontalInterpolation(bottomLeftPixel, bottomRightPixel, xLeft, xRight, originalPixelPosition.X);
        }

        Rgba32 pixelResult;

        if (yTop == yBottom)
        {
            pixelResult = AveragePixel(topTwoPixelInterpolationResult, bottomTwoPixelInterpolationResult);
        }
        else
        {
            pixelResult = ApplyVerticalInterpolation(topTwoPixelInterpolationResult, bottomTwoPixelInterpolationResult, yTop, yBottom, originalPixelPosition.Y);
        }

        return pixelResult;
    }

    private Vector3 AveragePixel(Rgba32 firstPixel, Rgba32 secondPixel) =>
        new(
            x: (firstPixel.R + secondPixel.R) / 2f,
            y: (firstPixel.G + secondPixel.G) / 2f,
            z: (firstPixel.B + secondPixel.B) / 2f);

    private Rgba32 AveragePixel(Vector3 firstPixel, Vector3 secondPixel) =>
        new(
            r: (byte)((firstPixel.X + secondPixel.X) / 2),
            g: (byte)((firstPixel.Y + secondPixel.Y) / 2),
            b: (byte)((firstPixel.Z + secondPixel.Z) / 2));

    private Vector3 ApplyHorizontalInterpolation(Rgba32 firstPixel, Rgba32 secondPixel, int firstX, int secondX, float originalX) =>
        new(
            x: firstPixel.R * (secondX - originalX) + secondPixel.R * (originalX - firstX),
            y: firstPixel.G * (secondX - originalX) + secondPixel.G * (originalX - firstX),
            z: firstPixel.B * (secondX - originalX) + secondPixel.B * (originalX - firstX));

    private Rgba32 ApplyVerticalInterpolation(Vector3 firstPixel, Vector3 secondPixel, int firstY, int secondY, float originalY) =>
        new(
            r: (byte)(firstPixel.X * (secondY - originalY) + secondPixel.X * (originalY - firstY)),
            g: (byte)(firstPixel.Y * (secondY - originalY) + secondPixel.Y * (originalY - firstY)),
            b: (byte)(firstPixel.Z * (secondY - originalY) + secondPixel.Z * (originalY - firstY)));
}
