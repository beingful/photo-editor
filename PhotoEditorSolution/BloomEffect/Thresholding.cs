using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;
using System.Collections.Concurrent;

namespace PhotoEditor.Effects;

public sealed class Thresholding
{
    private readonly byte _threshold;

    public Thresholding(byte threshold)
    {
        _threshold = threshold;
    }

    public Image<Rgba32> Apply(Image<Rgba32> image)
    {
        Image<Rgba32> contributedImage = new(image.Width, image.Height);

        var x = image.GetPixelMemoryGroup();

        IEnumerable<int> columnIndexes = Enumerable.Range(0, image.Width);
        IEnumerable<int> rowIndexes = Enumerable.Range(0, image.Height);

        OrderablePartitioner<int> columnPartitioner = Partitioner.Create(columnIndexes);
        OrderablePartitioner<int> rowPartitioner = Partitioner.Create(rowIndexes);

        columnPartitioner.AsParallel().ForAll(x =>
        {
            rowPartitioner.AsParallel().ForAll(y =>
            {
                contributedImage[x, y] = ContributedPixel(image[x, y]);
            });
        });

        return image;
    }

    private Rgba32 ContributedPixel(Rgba32 pixel)
    {
        byte brightness = Math.Max(pixel.R, Math.Max(pixel.G, pixel.B));
        double contribution = Math.Max(brightness - _threshold, byte.MinValue) / Math.Max(brightness, 0.00001);

        return new Rgba32(
            r: (byte)(contribution * pixel.R),
            g: (byte)(contribution * pixel.G),
            b: (byte)(contribution * pixel.B));
    }
}
