using FeynmanTechniqueBackend.Controllers.Criteria;

namespace FeynmanTechniqueBackend.Services.Interfaces
{
    public interface IUserManagementService : IPostService<ValidateUserCriteria, bool> { }
}
