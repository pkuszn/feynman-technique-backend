using FluentValidation;

namespace FeynmanTechniqueBackend.Validators
{
    public class ScrapValidator : AbstractValidator<ScrapCriteria>
    {
        public ScrapValidator()
        {
            RuleFor(r => r.Links)
                .NotEmpty();
        }
    }
}
