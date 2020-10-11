using FluentValidation;

namespace AdaskoTheBeAsT.FluentValidation.MediatR.Test
{
    public class SampleRequestIdGreaterThanZeroValidator
        : AbstractValidator<SampleRequest>
    {
        public SampleRequestIdGreaterThanZeroValidator()
        {
            RuleFor(s => s.Id).GreaterThan(0);
        }
    }
}
