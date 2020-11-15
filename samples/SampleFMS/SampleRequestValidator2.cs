using FluentValidation;

namespace SampleFMS
{
    public class SampleRequestValidator2 : AbstractValidator<SampleRequest>
    {
        public SampleRequestValidator2()
        {
            RuleFor(r => r.Id).LessThan(10);
        }
    }
}
