using FluentValidation;

namespace AdaskoTheBeAsT.FluentValidation.MediatR.Test;

#pragma warning disable CA1710 // Identifiers should have correct suffix
public class SampleRequestNameMaxLengthValidator
    : AbstractValidator<SampleRequest>
{
    public SampleRequestNameMaxLengthValidator()
    {
        RuleFor(s => s.Name).Must(n => n != null && n.Length >= 5).WithMessage("The length of 'Name' must be at least 5 characters.");
    }
}
#pragma warning restore CA1710 // Identifiers should have correct suffix
