using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PmtAdmin.Domain.Entities
{
    [Table("sprints")]
    public class Sprint
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Required]
        [Column("project_id")]
        public Guid? ProjectId { get; set; }

        [Column("name")]
        public string? Name { get; set; }

        [Column("sprint_goal")]
        public string? SprintGoal { get; set; }

        [Column("start_date")]
        public DateTime? StartDate { get; set; }

        [Column("due_date")]
        public DateTime? DueDate { get; set; }

        [MaxLength(50)]
        [Column("status")]
        public string Status { get; set; } = "PLANNED";

        [Column("story_point")]
        public decimal? StoryPoint { get; set; }

        [Column("created_by")]
        public int? CreatedBy { get; set; }

        [Column("updated_by")]
        public int? UpdatedBy { get; set; }

        [Column("created_at")]
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

        [Column("updated_at")]
        public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;

        // Navigation properties
        [ForeignKey("ProjectId")]
        public Project Project { get; set; }

        [ForeignKey("CreatedBy")]
        public User? Creator { get; set; }

        [ForeignKey("UpdatedBy")]
        public User? Updater { get; set; }

        public ICollection<Issue> Issues { get; set; }
    }

}
