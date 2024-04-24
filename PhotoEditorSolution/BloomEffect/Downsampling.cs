using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System.Collections.Concurrent;
using System.Numerics;

namespace PhotoEditor.Effects;

internal sealed class Downsampling
{
    private readonly int _ratio;
    
    public Downsampling(int ratio = 8)
    {
        _ratio = ratio;
    }

    public Image<Rgba32> Apply(Image<Rgba32> image)
    {
        Image<Rgba32> paddedImage = PadOriginalImage(image);
        Image<Rgba32> resizedImage = new(paddedImage.Width / _ratio, paddedImage.Height / _ratio);

        IEnumerable<int> columnIndexes = Enumerable.Range(0, paddedImage.Width).Where(x => x % _ratio == 0);
        IEnumerable<int> rowIndexes = Enumerable.Range(0, paddedImage.Height).Where(x => x % _ratio == 0);

        OrderablePartitioner<int> columnPartitioner = Partitioner.Create(columnIndexes);
        OrderablePartitioner<int> rowPartitioner = Partitioner.Create(rowIndexes);

        columnPartitioner.AsParallel().ForAll(x =>
        {
            rowPartitioner.AsParallel().ForAll(y =>
            {
                resizedImage[x / _ratio, y / _ratio] = GetAveragedPixel(new Vector2(x, y), paddedImage);
            });
        });

        return resizedImage;
    }

    private Image<Rgba32> PadOriginalImage(Image<Rgba32> image)
    {
        int paddedColumns = _ratio - (image.Width % _ratio);
        int paddedRows = _ratio - (image.Height % _ratio);

        int paddedWidth = image.Width + paddedColumns;
        int paddedHeight = image.Height + paddedRows;

        Image<Rgba32> paddedImage = image.Clone(x => x.Resize(paddedWidth, paddedHeight));

        for (int x = image.Width, xBack = image.Width - 1; x < paddedWidth; x++, xBack--)
        {
            for (int y = image.Height, yBack = image.Height - 1; y < paddedHeight; y++, yBack--)
            {
                paddedImage[x, y] = image[xBack, yBack];
            }
        }

        return paddedImage;
    }

    private Rgba32 GetAveragedPixel(Vector2 startPosition, Image<Rgba32> image)
    {
        int totalRed = 0;
        int totalGreen = 0;
        int totalBlue = 0;

        for (int i = 0; i < _ratio; i++)
        {
            for (int j = 0; j < _ratio; j++)
            {
                Rgba32 pixel = image[(int)startPosition.X + i, (int)startPosition.Y + j];

                totalRed += pixel.R;
                totalGreen += pixel.G;
                totalBlue += pixel.B;
            }
        }

        return new Rgba32(
            r: (byte)(totalRed / (_ratio * _ratio)),
            g: (byte)(totalGreen / (_ratio * _ratio)),
            b: (byte)(totalBlue / (_ratio * _ratio)));
    }
}
