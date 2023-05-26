using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FeynmanTechniqueBackend.Models
{
    [Table("word")]
    public class Word
    {
        [Key]
        [Column("id")]
        public int IdWord { get; set; }
        [Required]
        [Column("word")]
        public string Name { get; set; }
        [Required]
        [Column("part_of_speech")]
        public int PartOfSpeech { get; set; }
        [Required]
        [Column("created_date")]
        public DateTime CreatedDate { get; set; }
        [Column("context")]
        public string? Context { get; set; }
        [Column("link")]
        public string? Link { get; set; }
    }
}
