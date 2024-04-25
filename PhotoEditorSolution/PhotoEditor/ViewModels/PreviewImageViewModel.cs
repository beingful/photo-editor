using PhotoEditor.Effects.Models;

namespace PhotoEditor.Interface.ViewModels;

public sealed record class PreviewImageViewModel(IFormFile File, byte[] Image, BloomConfiguration BloomConfiguration);
