using FeynmanTechniqueBackend.Controllers.Criteria;
using FluentValidation;

namespace FeynmanTechniqueBackend.Validators
{
    public class ValidateUserValidator : AbstractValidator<ValidateUserCriteria>
    {
        public ValidateUserValidator()
        {
            RuleFor(r => r.Name)
                .Length(3, 50)
                .NotNull();
            RuleFor(r => r.Password)
                .Length(3, 50)
                .NotNull();
        }
    }
}
