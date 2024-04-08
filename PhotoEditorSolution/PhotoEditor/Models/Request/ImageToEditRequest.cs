namespace PhotoEditor.Models.Request;

public sealed class ImageToEditRequest
{
    public IFormFile File { get; init; }
}
