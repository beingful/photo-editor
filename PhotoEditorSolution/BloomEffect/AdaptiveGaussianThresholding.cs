using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.Collections.Concurrent;

namespace PhotoEditor.Effects;

public class AdaptiveGaussianThresholding
{
    private GaussianBlur _gaussianBlur;
    private LuminosityMethod _luminosityMethod;

    public AdaptiveGaussianThresholding(GaussianBlur gaussianBlur)
    {
        _gaussianBlur = gaussianBlur;
        _luminosityMethod = new();
    }

    public Image<Rgba32> Apply(Image<Rgba32> image)
    {
        Image<Rgba32> grayscaleImage = ConvertToGrayscale(image);
        Image<Rgba32> blurredGrayscaleImage = _gaussianBlur.Apply(grayscaleImage);

        return GetBinarizedImage(grayscaleImage, blurredGrayscaleImage);
    }

    private Image<Rgba32> ConvertToGrayscale(Image<Rgba32> image)
    {
        Image<Rgba32> grayscaleImage = new(image.Width, image.Height);

        IEnumerable<int> columnIndexes = Enumerable.Range(0, image.Width);
        OrderablePartitioner<int> columnPartitioner = Partitioner.Create(columnIndexes);

        columnPartitioner.AsParallel().ForAll(x =>
        {
            for (int y = 0; y < image.Height; y++)
            {
                byte grey = (byte)_luminosityMethod.Calculate(image[x, y]);

                grayscaleImage[x, y] = new Rgba32(r: grey, g: grey, b: grey);
            }
        });

        return grayscaleImage;
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
        _luminosityMethod.Calculate(pixel) > _luminosityMethod.Calculate(blurredPixel)
        ? new Rgba32(r: 255, g: 255, b: 255)
        : new Rgba32(r: 0, g: 0, b: 0);
}
