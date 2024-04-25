using PhotoEditor.Effects.Models;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace PhotoEditor.Interface.ViewModels;

public sealed record class EditImageViewModel(Image<Rgba32> OriginalImage, Image<Rgba32> EditedImage, BloomConfiguration BloomConfiguration);
