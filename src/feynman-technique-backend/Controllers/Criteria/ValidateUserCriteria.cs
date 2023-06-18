using FeynmanTechniqueBackend.Services.Interfaces;

namespace FeynmanTechniqueBackend.Controllers.Criteria
{
    public class ValidateUserCriteria : ICriteria
    {
        public string? Name { get; set; }
        public string? Password { get; set; }
    }
}
