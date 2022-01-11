using FluentValidation;

namespace AdaskoTheBeAsT.FluentValidation.MediatR.Test;

#pragma warning disable CA1710 // Identifiers should have correct suffix
public class SampleStreamRequestNameNonEmptyValidator
    : AbstractValidator<SampleRequest>
{
    public SampleStreamRequestNameNonEmptyValidator()
    {
        RuleFor(s => s.Name).NotEmpty();
    }
}
#pragma warning restore CA1710 // Identifiers should have correct suffix
