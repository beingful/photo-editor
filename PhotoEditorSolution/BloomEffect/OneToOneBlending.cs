using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.Collections.Concurrent;

namespace PhotoEditor.Effects;

internal sealed class OneToOneBlending
{
    private readonly float _colorIntensity;

    public OneToOneBlending(float colorIntensity)
    {
        _colorIntensity = colorIntensity;
    }

    public Image<Rgba32> Apply(Image<Rgba32> targetImage, Image<Rgba32> otherImage)
    {
        Image<Rgba32> imageResult = new(targetImage.Width, targetImage.Height);

        IEnumerable<int> columnIndexes = Enumerable.Range(0, imageResult.Width);
        IEnumerable<int> rowIndexes = Enumerable.Range(0, imageResult.Height);

        OrderablePartitioner<int> columnPartitioner = Partitioner.Create(columnIndexes);
        OrderablePartitioner<int> rowPartitioner = Partitioner.Create(rowIndexes);

        columnPartitioner.AsParallel().ForAll(x =>
        {
            rowPartitioner.AsParallel().ForAll(y =>
            {
                Rgba32 originalPixel = targetImage[x, y];
                Rgba32 blurredPixel = otherImage[x, y];

                imageResult[x, y] = new Rgba32(
                    r: (byte)Math.Min(originalPixel.R + _colorIntensity * blurredPixel.R, byte.MaxValue),
                    g: (byte)Math.Min(originalPixel.G + _colorIntensity * blurredPixel.G, byte.MaxValue),
                    b: (byte)Math.Min(originalPixel.B + _colorIntensity * blurredPixel.B, byte.MaxValue));
            });
        });

        return imageResult;
    }
}
