using FluentValidation;

namespace AdaskoTheBeAsT.FluentValidation.MediatR.Test;

#pragma warning disable CA1710 // Identifiers should have correct suffix
public class SampleStreamRequestIdGreaterThanZeroValidator
    : AbstractValidator<SampleStreamRequest>
{
    public SampleStreamRequestIdGreaterThanZeroValidator()
    {
        RuleFor(s => s.Id).GreaterThan(0);
    }
}
#pragma warning restore CA1710 // Identifiers should have correct suffix
