using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PmtAdmin.Domain.Entities
{
    [Table("status")]
    public class Status
    {
        [Key]
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column("status_name")]
        [Required]
        public string? StatusName { get; set; }

        // Navigation properties
        public ICollection<BoardColumn> BoardColumns { get; set; }
    }

}
