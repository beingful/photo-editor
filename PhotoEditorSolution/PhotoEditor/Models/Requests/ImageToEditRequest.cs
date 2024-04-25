namespace PhotoEditor.Models.Requests;

public sealed class ImageToEditRequest
{
    public IFormFile File { get; init; }

    public int BlurRadius { get; init; } = 10;

    public int DownscalingRatio { get; init; } = 16;

    public byte Threashold { get; init; } = 50;

    public decimal Intensity { get; init; } = 1;
}
