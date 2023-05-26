namespace FeynmanTechniqueBackend.Controllers.Criteria
{
    public class WordCriteria
    {
        public int IdWord { get; set; }
        public string Name { get; set; }
        public int PartOfSpeech { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? Context { get; set; }
        public string? Link { get; set; }
    }
}