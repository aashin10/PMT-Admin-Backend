using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PmtAdmin.Domain.Entities
{
    public class Issue
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public string? Key { get; set; }

        [Required]
        public int ProjectId { get; set; }

        //public int? EpicId { get; set; }
        public int? SprintId { get; set; }

        [Required]
        public string? Summary { get; set; }

        public string? Title { get; set; }
        public string? Description { get; set; }

        [MaxLength(50)]
        public string Type { get; set; } = "STORY";

        [MaxLength(50)]
        public string Priority { get; set; } = "MEDIUM";

        [MaxLength(50)]
        public string Status { get; set; } = "TODO";

        public int? AssigneeId { get; set; }
        public int? ReporterId { get; set; }
        public int StoryPoints { get; set; } = 0;
        public string Labels { get; set; } = "[]";
        public DateTime? StartDate { get; set; }
        public DateTime? DueDate { get; set; }
        public int? CreatedById { get; set; }
        public int? UpdatedById { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("ProjectId")]
        public virtual Project? Project { get; set; }

        //[ForeignKey("EpicId")]
        //public virtual Epic Epic { get; set; }

        [ForeignKey("SprintId")]
        public virtual Sprint? Sprint { get; set; }

        [ForeignKey("AssigneeId")]
        public virtual User? Assignee { get; set; }

        [ForeignKey("ReporterId")]
        public virtual User? Reporter { get; set; }

        [ForeignKey("CreatedById")]
        public virtual User? CreatedBy { get; set; }

        [ForeignKey("UpdatedById")]
        public virtual User? UpdatedBy { get; set; }

        public virtual ICollection<IssueComment> Comments { get; set; }
    }
}
