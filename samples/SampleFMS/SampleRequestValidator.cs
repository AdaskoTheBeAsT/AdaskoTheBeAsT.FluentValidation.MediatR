using FluentValidation;

namespace SampleFMS
{
    public class SampleRequestValidator : AbstractValidator<SampleRequest>
    {
        public SampleRequestValidator()
        {
            RuleFor(r => r.Id).GreaterThan(0);
        }
    }
}
