using FeynmanTechniqueBackend.Controllers.Criteria;
using FeynmanTechniqueBackend.Models;

namespace FeynmanTechniqueBackend.Services.Interfaces
{
    public interface ILinguisticCorpusFillmentService : IPostService<ScrapCriteria, List<Word>> { }
}
