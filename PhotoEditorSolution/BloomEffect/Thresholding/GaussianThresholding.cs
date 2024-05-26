using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp;
using System.Collections.Concurrent;

namespace PhotoEditor.Effects.Thresholding;

public sealed class GaussianThresholding
{
    private readonly float _thresholdRate;

    public GaussianThresholding(float thresholdRate)
    {
        _thresholdRate = thresholdRate;
    }

    public Image<Rgba32> Apply(Image<Rgba32> image, Image<Rgba32> blurredImage)
    {
        Image<Rgba32> binarizedImage = new(image.Width, image.Height);

        IEnumerable<int> columnIndexes = Enumerable.Range(0, image.Width);
        IEnumerable<int> rowIndexes = Enumerable.Range(0, image.Height);

        OrderablePartitioner<int> columnPartitioner = Partitioner.Create(columnIndexes);
        OrderablePartitioner<int> rowPartitioner = Partitioner.Create(rowIndexes);

        columnPartitioner.AsParallel().ForAll(x =>
        {
            rowPartitioner.AsParallel().ForAll(y =>
            {
                binarizedImage[x, y] = ApplyToPixel(image[x, y], blurredImage[x, y]);
            });
        });

        return binarizedImage;
    }

    private Rgba32 ApplyToPixel(Rgba32 originalPixel, Rgba32 blurredPixel)
    {
        float originalPixelLuminosity = 0.2126f * originalPixel.R + 0.7152f * originalPixel.G + 0.0722f * originalPixel.B;
        float blurredPixelLuminosity = 0.2126f * blurredPixel.R + 0.7152f * blurredPixel.G + 0.0722f * blurredPixel.B;

        byte binarizedValue = Convert.ToByte(originalPixelLuminosity > (_thresholdRate + 1) * blurredPixelLuminosity);

        return new Rgba32(
            r: (byte)(binarizedValue * originalPixel.R),
            g: (byte)(binarizedValue * originalPixel.G),
            b: (byte)(binarizedValue * originalPixel.B));
    }
}
