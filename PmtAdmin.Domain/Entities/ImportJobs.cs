using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace PmtAdmin.Domain.Entities
{
    public class ImportJobs
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Type { get; set; } = string.Empty;

        [MaxLength(50)]
        public string? Source { get; set; }

        [MaxLength(50)]
        public string Status { get; set; } = "pending";

        [ForeignKey("StartedBy")]
        public int StartedBy { get; set; }

        public DateTime? StartedAt { get; set; }

        public DateTime? FinishedAt { get; set; }

        public JsonDocument? Summary { get; set; }

        public JsonDocument? Details { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public virtual User? StartedByUser { get; set; }
    }
}