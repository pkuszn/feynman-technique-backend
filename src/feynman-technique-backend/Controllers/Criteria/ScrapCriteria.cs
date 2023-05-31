using FeynmanTechniqueBackend.Services.Interfaces;

namespace FeynmanTechniqueBackend;

public class ScrapCriteria : ICriteria
{
    public HashSet<string> Links { get; set; }
}
