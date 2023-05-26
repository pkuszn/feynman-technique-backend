using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FeynmanTechniqueBackend.Models
{
    [Table("user")]
    public class User : IEntity<int>
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Required]
        [Column("role")]
        public int Role { get; set; }
        [Required]
        [Column("name")]
        public string Name { get; set; }
        [Required]
        [Column("password")]
        public string Password { get; set; }
        [Required]
        [Column("created_date")]
        public DateTime CreatedDate { get; set; }

        public void ClearKey()
        {
            Id = 0;
        }
    }
}
