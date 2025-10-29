using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PmtAdmin.Domain.Entities
{
    [Table("status")]
    public class Status
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("status_name")]
        public string? StatusName { get; set; }

        // Navigation properties
        public ICollection<BoardColumn> BoardColumns { get; set; }
    }

}
