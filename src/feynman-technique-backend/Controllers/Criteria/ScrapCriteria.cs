using FeynmanTechniqueBackend.Services.Interfaces;

namespace FeynmanTechniqueBackend.Controllers.Criteria
{
    public class ScrapCriteria : ICriteria
    {
        public HashSet<string> Links { get; set; }
    }
}

