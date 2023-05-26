using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FeynmanTechniqueBackend.Models
{
    [Table("role")]
    public class Role
    {
        [Key]
        [Column("id")]
        public int IdRole { get; set; }
        [Required]
        [Column("name")]
        public string Name { get; set; } 
    }
}
