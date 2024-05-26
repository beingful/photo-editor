using PhotoEditor.Effects.Blending;
using PhotoEditor.Effects.Blur;
using PhotoEditor.Effects.Models;
using PhotoEditor.Effects.Scaling;
using PhotoEditor.Effects.Thresholding;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using StackExchange.Profiling;

namespace PhotoEditor.Effects;

public sealed class BloomEffect
{
    private readonly GaussianBlur _blur;
    private readonly GaussianThresholding _thresholding;
    private readonly AdditiveBlending _blending;
    private readonly BloomConfiguration _configuration;

    public BloomEffect(BloomConfiguration bloomConfiguration)
    {
        _blur = new GaussianBlur(bloomConfiguration.BlurRadius);
        _thresholding = new GaussianThresholding(bloomConfiguration.Threashold);
        _blending = new AdditiveBlending(bloomConfiguration.Intensity);
        _configuration = bloomConfiguration;
    }

    public Image<Rgba32> Apply(Image<Rgba32> image)
    {
        Image<Rgba32> downscaledImage;

        BilinearInterpolation downscaling = new(image.Width / _configuration.DownscalingRatio, image.Height / _configuration.DownscalingRatio);

        using (MiniProfiler.Current.Step("Bilinear Downscaling"))
        {
            downscaledImage = downscaling.Apply(image);
        }

        Image<Rgba32> thresholdedImage;

        using (MiniProfiler.Current.Step("Gaussian Thresholding"))
        {
            Image<Rgba32> blurredDownscaledImage = _blur.Apply(downscaledImage);

            thresholdedImage = _thresholding.Apply(downscaledImage, blurredDownscaledImage);
        }

        Image<Rgba32> blurredThresholdedDownscaledImage;

        using (MiniProfiler.Current.Step("Gaussian blur"))
        {
            blurredThresholdedDownscaledImage = _blur.Apply(thresholdedImage);
        }

        BilinearInterpolation upsampling = new(image.Width, image.Height);

        Image<Rgba32> upscaledImage;

        using (MiniProfiler.Current.Step("Bilinear Upscaling"))
        {
            upscaledImage = upsampling.Apply(blurredThresholdedDownscaledImage);
        }

        Image<Rgba32> result;

        using (MiniProfiler.Current.Step("Additive Blending"))
        {
            result = _blending.Apply(image, upscaledImage);
        }

        return result;
    }
}
