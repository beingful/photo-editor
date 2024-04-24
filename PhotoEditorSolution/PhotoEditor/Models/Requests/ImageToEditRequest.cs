namespace PhotoEditor.Models.Requests;

public sealed class ImageToEditRequest
{
    public IFormFile File { get; init; }
}
