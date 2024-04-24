using PhotoEditor.Effects.Models;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace PhotoEditor.Effects;

public sealed class BloomEffect
{
    private readonly GaussianBlur _blur;
    private readonly Downsampling _downsampling;
    private readonly Thresholding _thresholding;
    private readonly OneToOneBlending _blending;

    public BloomEffect(BloomConfiguration bloomConfiguration)
    {
        _blur = new GaussianBlur(bloomConfiguration.BlurRadius);
        _downsampling = new Downsampling(bloomConfiguration.DownscalingRatio);
        _thresholding = new Thresholding(bloomConfiguration.Threashold);
        _blending = new OneToOneBlending(bloomConfiguration.Intensity);
    }

    public Image<Rgba32> Apply(Image<Rgba32> image)
    {
        Image<Rgba32> downscaledImage = _downsampling.Apply(image);
        Image<Rgba32> blurredDownscaledImage = _blur.Apply(downscaledImage);
        Image<Rgba32> thresholdedImage = _thresholding.Apply(blurredDownscaledImage);

        BilinearSampling upsampling = new(image.Width, image.Height);

        Image<Rgba32> upscaledImage = upsampling.Apply(thresholdedImage);

        return _blending.Apply(image, upscaledImage);
    }
}
