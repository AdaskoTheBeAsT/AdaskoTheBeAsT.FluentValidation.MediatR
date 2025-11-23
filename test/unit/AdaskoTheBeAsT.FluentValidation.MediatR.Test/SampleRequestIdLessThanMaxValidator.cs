using FluentValidation;

namespace AdaskoTheBeAsT.FluentValidation.MediatR.Test;

#pragma warning disable CA1710 // Identifiers should have correct suffix
public class SampleRequestIdLessThanMaxValidator
    : AbstractValidator<SampleRequest>
{
    public SampleRequestIdLessThanMaxValidator()
    {
        RuleFor(s => s.Id).GreaterThanOrEqualTo(10).WithMessage("'Id' must be at least 10.");
    }
}
#pragma warning restore CA1710 // Identifiers should have correct suffix
