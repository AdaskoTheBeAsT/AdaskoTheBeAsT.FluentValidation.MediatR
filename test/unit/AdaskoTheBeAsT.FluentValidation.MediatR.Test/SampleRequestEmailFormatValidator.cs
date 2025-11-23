using FluentValidation;

namespace AdaskoTheBeAsT.FluentValidation.MediatR.Test;

#pragma warning disable CA1710 // Identifiers should have correct suffix
public class SampleRequestEmailFormatValidator
    : AbstractValidator<SampleRequest>
{
    public SampleRequestEmailFormatValidator()
    {
        RuleFor(s => s.Email).Must(e => e != null && e.Contains("@")).WithMessage("'Email' must contain '@' symbol.");
    }
}
#pragma warning restore CA1710 // Identifiers should have correct suffix
