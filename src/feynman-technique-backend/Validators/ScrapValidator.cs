using FeynmanTechniqueBackend.Controllers.Criteria;
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
