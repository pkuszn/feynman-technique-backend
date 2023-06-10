namespace FeynmanTechniqueBackend.HttpModels.Models
{
    public class DetailedWordResponse
    {
        public string? Source { get; set; }
        public List<DetailedWordDtoKey> Words { get; set; } = new();
    }
}
