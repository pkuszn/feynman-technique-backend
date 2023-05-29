using FeynmanTechniqueBackend.Models;
using FeynmanTechniqueBackend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FeynmanTechniqueBackend.Services
{
    public class ServiceUtilitiesService : IServiceUtilitiesService
    {
        private readonly ILogger<ServiceUtilitiesService> Logger;
        private readonly FeynmanTechniqueBackendContext FeynmanTechniqueBackendContext;


        public ServiceUtilitiesService(ILogger<ServiceUtilitiesService> logger, FeynmanTechniqueBackendContext feynmanTechniqueBackendContext)
        {
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            FeynmanTechniqueBackendContext = feynmanTechniqueBackendContext ?? throw new ArgumentNullException(nameof(feynmanTechniqueBackendContext));
        }

        public async Task<bool> GetAsync(CancellationToken cancellationToken)
        {
            return await ExecuteStoredProcedureAsync(cancellationToken);
        }

        private async Task<bool> ExecuteStoredProcedureAsync(CancellationToken cancellationToken)
        {
            return await FeynmanTechniqueBackendContext.Database.ExecuteSqlInterpolatedAsync($"call `remove_duplicates`", cancellationToken: cancellationToken) > 0;
        }
    }
}
