using FluentValidation;

namespace AdaskoTheBeAsT.FluentValidation.MediatR.Test;

#pragma warning disable CA1710 // Identifiers should have correct suffix
public class SampleRequestEmailNotEmptyValidator
    : AbstractValidator<SampleRequest>
{
    public SampleRequestEmailNotEmptyValidator()
    {
        RuleFor(s => s.Email).NotEmpty();
    }
}
#pragma warning restore CA1710 // Identifiers should have correct suffix
