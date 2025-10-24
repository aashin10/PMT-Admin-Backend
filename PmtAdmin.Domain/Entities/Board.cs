using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PmtAdmin.Domain.Entities
{
    public class Board
    {
        public int Id { get; set; }

        [Required]
        public int ProjectId { get; set; }

        public int? TeamId { get; set; }

        public string? Name { get; set; }

        public string? Description { get; set; }

        [MaxLength(50)]
        public string? Type { get; set; }

        public bool IsActive { get; set; } = true;
        public int? CreatedById { get; set; }
        public int? UpdatedById { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        [ForeignKey("ProjectId")]
        public virtual Project? Project { get; set; }

        [ForeignKey("TeamId")]
        public virtual Team? Team { get; set; }

        [ForeignKey("CreatedById")]
        public virtual User? CreatedBy { get; set; }

        [ForeignKey("UpdatedById")]
        public virtual User? UpdatedBy { get; set; }

        public virtual ICollection<Sprint>? Sprints { get; set; }

        public virtual ICollection<Epic>? Epics { get; set; }

        public virtual ICollection<Issue>? Issues { get; set; }


    }
}
