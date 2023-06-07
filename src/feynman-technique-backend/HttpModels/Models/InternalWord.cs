namespace FeynmanTechniqueBackend.HttpModels.Models
{
    public class InternalWord
    {
        public int IdLink { get; set; }
        public HashSet<string>? Words { get; set; }
    }
}

