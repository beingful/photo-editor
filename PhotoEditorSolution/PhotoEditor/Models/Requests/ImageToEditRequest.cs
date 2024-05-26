namespace PhotoEditor.Models.Requests;

public sealed class ImageToEditRequest
{
    public IFormFile File { get; init; }

    public int BlurRadius { get; init; } = 10;

    public int DownscalingRatio { get; init; } = 16;

    public float Threashold { get; init; } = 0.25f;

    public float Intensity { get; init; } = 0.25f;
}
