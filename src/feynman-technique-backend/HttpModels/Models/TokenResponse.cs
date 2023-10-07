using System.Text.Json.Serialization;

namespace FeynmanTechniqueBackend.HttpModels.Models
{
    public class TokenResponse
    {
        public string? Name { get; set; }
        public string? Lemma { get; set; }
        [JsonPropertyName("part_of_speech")]
        public string? PartOfSpeech { get; set; }
    }
}

