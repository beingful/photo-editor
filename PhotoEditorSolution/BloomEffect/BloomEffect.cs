using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.Collections.Concurrent;

namespace PhotoEditor.Effects;

public class BloomEffect
{
    private GaussianBlur _blur;
    private AdaptiveGaussianThresholding _thresholding;
    private ScreenBlending _screenBlending;

    public BloomEffect(GaussianBlur blur, AdaptiveGaussianThresholding thresholding, ScreenBlending screenBlending)
    {
        _blur = blur;
        _thresholding = thresholding;
        _screenBlending = screenBlending;
    }

    public Image<Rgba32> Apply(Image<Rgba32> image)
    {
        Image<Rgba32> blurredImage = _blur.Apply(image);
        Image<Rgba32> binarizedImage = _thresholding.Apply(image);

        return BlendImages(image, blurredImage, binarizedImage);
    }

    private Image<Rgba32> BlendImages(Image<Rgba32> originalImage, Image<Rgba32> blurredImage, Image<Rgba32> binarizedImage)
    {
        Image<Rgba32> imageResult = new(originalImage.Width, originalImage.Height);

        IEnumerable<int> columnIndexes = Enumerable.Range(0, imageResult.Width);
        OrderablePartitioner<int> columnPartitioner = Partitioner.Create(columnIndexes);

        columnPartitioner.AsParallel().ForAll(x =>
        {
            for (int y = 0; y < imageResult.Height; y++)
            {
                imageResult[x, y] = binarizedImage[x, y].R == 0
                    ? _screenBlending.Apply(originalImage[x, y], blurredImage[x, y])
                    : originalImage[x, y];
            }
        });

        return imageResult;
    }
}
