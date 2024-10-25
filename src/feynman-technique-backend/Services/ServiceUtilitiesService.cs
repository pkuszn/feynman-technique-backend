using FeynmanTechniqueBackend.Models;
using FeynmanTechniqueBackend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FeynmanTechniqueBackend.Services;

public class ServiceUtilitiesService : IServiceUtilitiesService
{
    private readonly FeynmanTechniqueCorpusContext DbContext;

    public ServiceUtilitiesService(FeynmanTechniqueCorpusContext dbContext)
    {
        DbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async Task<bool> GetAsync(CancellationToken cancellationToken)
    {
        return await ExecuteStoredProcedureAsync(cancellationToken);
    }

    private async Task<bool> ExecuteStoredProcedureAsync(CancellationToken cancellationToken)
    {
        return await DbContext.Database.ExecuteSqlInterpolatedAsync($"call `remove_duplicates`", cancellationToken: cancellationToken) > 0;
    }
}
