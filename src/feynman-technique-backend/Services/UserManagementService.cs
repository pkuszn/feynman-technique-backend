using FeynmanTechniqueBackend.Controllers.Criteria;
using FeynmanTechniqueBackend.Models;
using FeynmanTechniqueBackend.Repository.Interfaces;
using FeynmanTechniqueBackend.Services.Interfaces;
using FluentValidation;
using FluentValidation.Results;

namespace FeynmanTechniqueBackend.Services
{
    public class UserManagementService : IUserManagementService
    {
        private const string ErrorMessage = "Authenticate {response} failed. {request} is null or empty. <{status}>";
        private readonly ILogger<UserManagementService> Logger;
        private readonly IValidator<ValidateUserCriteria> Validator;
        private readonly IRepositoryAsync Repository;

        public UserManagementService(ILogger<UserManagementService> logger,
            IValidator<ValidateUserCriteria> validator,
            IRepositoryAsync repository)
        {
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            Validator = validator ?? throw new ArgumentNullException(nameof(validator));
            Repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }
        public async Task<bool> PostAsync(ValidateUserCriteria criteria, CancellationToken cancellationToken)
        {
            ValidationResult result = await Validator.ValidateAsync(criteria, cancellationToken);
            if (!result.IsValid)
            {
                Logger.LogError(ErrorMessage, nameof(User), nameof(ValidateUserCriteria), StatusCodes.Status400BadRequest);
                return false;
            }

            List<User> users = await Repository.GetAsync<User>(f => f.Name == criteria.Name, cancellationToken);
            if ((users?.Count ?? 0) == 0)
            {
                Logger.LogInformation("There is no user about given data: {login}", criteria.Name);
                return false;
            }

            return users.Any(w => w.Password == criteria.Password);
        }
    }
}
