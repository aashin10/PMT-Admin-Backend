using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PmtAdmin.Domain.Entities
{

    [Table("board_columns")]
    public class BoardColumn
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Column("status_id")]
        public Guid? StatusId { get; set; }

        [Column("board_column_name")]
        public string? BoardColumnName { get; set; }

        [Column("board_color")]
        public string? BoardColor { get; set; }

        [Column("position")]
        public int? Position { get; set; }

        // Navigation properties
        [ForeignKey("StatusId")]
        public Status? Status { get; set; }
    }

}
