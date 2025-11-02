using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace PmtAdmin.Domain.Entities
{
    [Table("issues")]
    public class Issue
    {
        [Key]
        [Column("id")]
        [Required]
        public Guid Id { get; set; }

        [Column("key")]
        public string? Key { get; set; }

        [Required]
        [Column("project_id")]
        public Guid? ProjectId { get; set; }

        [Column("epic_id")]
        public Guid? EpicId { get; set; }

        [Column("sprint_id")]
        public Guid? SprintId { get; set; }

        [Column("parent_issue_id")]
        public Guid? ParentIssueId { get; set; }

        [Column("attachment_url")]
        public string? AttachmentUrl { get; set; }

        [Column("title")]
        [Required]
        public string Title { get; set; }

        [Column("description")]
        public string? Description { get; set; }

        [Column("type")]
        [Required]
        public string Type { get; set; }

        [Column("priority")]
        public string? Priority { get; set; }

        [Column("status")]
        public int? StatusId { get; set; }

        [Column("assignee_id")]
        public int? AssigneeId { get; set; }

        [Column("reporter_id")]
        [Required]
        public int ReporterId { get; set; }

        [Column("story_points")]
        public int? StoryPoints { get; set; }

        [Column("labels", TypeName = "jsonb")]
        public string? Labels { get; set; }

        [Column("start_date")]
        public DateTimeOffset? StartDate { get; set; }

        [Column("due_date")]
        public DateTimeOffset? DueDate { get; set; }

        [Column("created_by")]
        public int? CreatedBy { get; set; }

        [Column("updated_by")]
        public int? UpdatedBy { get; set; }

        [Column("created_at")]
        public DateTimeOffset? CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTimeOffset? UpdatedAt { get; set; }

        // Navigation properties
        [ForeignKey("StatusId")]
        public Status? Status { get; set; }

        [ForeignKey("ProjectId")]
        public Project? Project { get; set; }

        [ForeignKey("EpicId")]
        public Epic? Epic { get; set; }

        [ForeignKey("SprintId")]
        public Sprint? Sprint { get; set; }

        [ForeignKey("ParentIssueId")]
        public Issue? ParentIssue { get; set; }

        [ForeignKey("AssigneeId")]
        public User? Assignee { get; set; }

        [ForeignKey("ReporterId")]
        public User Reporter { get; set; }

        [ForeignKey("CreatedBy")]
        public User? Creator { get; set; }

        [ForeignKey("UpdatedBy")]
        public User? Updater { get; set; }
        public ICollection<Issue> ChildIssues { get; set; }
        public ICollection<IssueComment> IssueComments { get; set; }
    }
}

