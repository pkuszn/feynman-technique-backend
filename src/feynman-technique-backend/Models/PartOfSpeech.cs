using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FeynmanTechniqueBackend.Models
{
    [Table("part_of_speech")]
    public class PartOfSpeech : IEntity<int>
    {
        [Key]
        [Column("id")] 
        public int Id { get; set; }
        [Required]
        [Column("name")]
        public string Name { get; set; }
    }
}
