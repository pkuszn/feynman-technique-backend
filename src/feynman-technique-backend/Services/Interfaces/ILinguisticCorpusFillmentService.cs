using FeynmanTechniqueBackend.Controllers.Criteria;

namespace FeynmanTechniqueBackend.Services.Interfaces
{
    public interface ILinguisticCorpusFillmentService :
        IPostService<ScrapCriteria, bool>
    {
        Task<bool> ManualPostAsync(List<string> words, CancellationToken cancellationToken);
    }
}
