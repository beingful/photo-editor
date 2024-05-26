using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.Collections.Concurrent;

namespace PhotoEditor.Effects.Blending;

internal sealed class AdditiveBlending
{
    private readonly float _colorIntensityRate;

    public AdditiveBlending(float colorIntensity)
    {
        _colorIntensityRate = colorIntensity;
    }

    public Image<Rgba32> Apply(Image<Rgba32> originalImage, Image<Rgba32> blurredImage)
    {
        Image<Rgba32> imageResult = new(originalImage.Width, originalImage.Height);

        IEnumerable<int> columnIndexes = Enumerable.Range(0, imageResult.Width);
        IEnumerable<int> rowIndexes = Enumerable.Range(0, imageResult.Height);

        OrderablePartitioner<int> columnPartitioner = Partitioner.Create(columnIndexes);
        OrderablePartitioner<int> rowPartitioner = Partitioner.Create(rowIndexes);

        columnPartitioner.AsParallel().ForAll(x =>
        {
            rowPartitioner.AsParallel().ForAll(y =>
            {
                Rgba32 originalPixel = originalImage[x, y];
                Rgba32 blurredPixel = blurredImage[x, y];

                imageResult[x, y] = new Rgba32(
                    r: (byte)Math.Min(originalPixel.R + 7 * _colorIntensityRate * blurredPixel.R, byte.MaxValue),
                    g: (byte)Math.Min(originalPixel.G + 7 * _colorIntensityRate * blurredPixel.G, byte.MaxValue),
                    b: (byte)Math.Min(originalPixel.B + 7 * _colorIntensityRate * blurredPixel.B, byte.MaxValue));
            });
        });

        return imageResult;
    }
}
