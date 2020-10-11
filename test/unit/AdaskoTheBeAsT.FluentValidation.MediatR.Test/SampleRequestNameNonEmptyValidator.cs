using FluentValidation;

namespace AdaskoTheBeAsT.FluentValidation.MediatR.Test
{
    public class SampleRequestNameNonEmptyValidator
        : AbstractValidator<SampleRequest>
    {
        public SampleRequestNameNonEmptyValidator()
        {
            RuleFor(s => s.Name).NotEmpty();
        }
    }
}
