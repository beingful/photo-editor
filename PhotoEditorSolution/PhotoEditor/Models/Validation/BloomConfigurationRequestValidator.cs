using FluentValidation;
using PhotoEditor.Interface.Models.Requests;

namespace PhotoEditor.Interface.Models.Validation;

public sealed class BloomConfigurationRequestValidator : AbstractValidator<BloomConfigurationRequest>
{
    public BloomConfigurationRequestValidator()
    {
        RuleFor(model => model.Intensity).InclusiveBetween(0, 2);
        RuleFor(model => model.BlurRadius).InclusiveBetween(1, 50);
        RuleFor(model => model.DownscalingRatio).InclusiveBetween(2, 20);
    }
}
