using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FeynmanTechniqueBackend.Models
{
    [Table("part_of_speech")]
    public class PartOfSpeech
    {
        [Key]
        [Column("id")] 
        public int IdPartOfSpeech { get; set; }
        [Required]
        [Column("name")]
        public string Name { get; set; } 
    }
}
