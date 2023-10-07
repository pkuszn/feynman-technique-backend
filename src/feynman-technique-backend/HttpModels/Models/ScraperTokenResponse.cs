namespace FeynmanTechniqueBackend.HttpModels.Models
{
    public class ScraperTokenResponse
    {
        public string? Source { get; set; }
        public List<TokenResponse> Words { get; set; } = new();
    }
}
