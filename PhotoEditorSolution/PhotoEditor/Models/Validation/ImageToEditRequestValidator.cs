using FluentValidation;
using PhotoEditor.Models.Requests;

namespace PhotoEditor.Interface.Models.Validation;

public sealed class ImageToEditRequestValidator : AbstractValidator<ImageToEditRequest>
{
    public ImageToEditRequestValidator()
    {
        RuleFor(model => model.File.Length).GreaterThan(0);
        RuleFor(model => model.Intensity).InclusiveBetween(0, 2);
        RuleFor(model => model.BlurRadius).InclusiveBetween(1, 50);
        RuleFor(model => model.DownscalingRatio).InclusiveBetween(2, 20);
    }
}
