using BloomEffect;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.Collections.Concurrent;

namespace PhotoEditor.Effects;

public class AdaptiveGaussianThresholding
{
    public GaussianBlur GaussianBlur;
    public LuminosityMethod LuminosityMethod;

    public AdaptiveGaussianThresholding(GaussianBlur gaussianBlur)
    {
        GaussianBlur = gaussianBlur;
        LuminosityMethod = new();
    }

    public Image<Rgba32> Apply(Image<Rgba32> image)
    {
        Image<Rgba32> blurredImage = GaussianBlur.Apply(image);

        return GetBinarizedImage(image, blurredImage);
    }

    private Image<Rgba32> GetBinarizedImage(Image<Rgba32> image, Image<Rgba32> blurredImage)
    {
        Image<Rgba32> binarizedImage = new(image.Width, image.Height);

        IEnumerable<int> columnIndexes = Enumerable.Range(0, image.Width);
        OrderablePartitioner<int> columnPartitioner = Partitioner.Create(columnIndexes);

        columnPartitioner.AsParallel().ForAll(x =>
        {
            for (int y = 0; y < image.Height; y++)
            {
                binarizedImage[x, y] = GetBinarizedPixel(image[x, y], blurredImage[x, y]);
            }
        });

        return binarizedImage;
    }

    private Rgba32 GetBinarizedPixel(Rgba32 pixel, Rgba32 blurredPixel) =>
        LuminosityMethod.Calculate(pixel) > LuminosityMethod.Calculate(blurredPixel)
        ? new Rgba32(r: 255, g: 255, b: 255)
        : new Rgba32(r: 0, g: 0, b: 0);
}
