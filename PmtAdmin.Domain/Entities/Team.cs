using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace PmtAdmin.Domain.Entities
{
    // ============================================
    // TEAMS
    // ============================================
    [Table("teams")]
    public class Team
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("project_id")]
        public Guid? ProjectId { get; set; }

        [Required]
        [MaxLength(150)]
        [Column("name")]
        public string Name { get; set; }

        [Column("description")]
        public string? Description { get; set; }

        [Column("lead_id")]
        public int? LeadId { get; set; }

        [Column("is_active")]
        public bool IsActive { get; set; } = true;

        [Column("created_by")]
        public int? CreatedBy { get; set; }

        [Column("updated_by")]
        public int? UpdatedBy { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        [ForeignKey("ProjectId")]
        public Project Project { get; set; }

        [ForeignKey("LeadId")]
        public User? Lead { get; set; }

        [ForeignKey("CreatedBy")]
        public User? Creator { get; set; }

        [ForeignKey("UpdatedBy")]
        public User? Updater { get; set; }

        public ICollection<Board> Boards { get; set; }
        public ICollection<ProjectMember> ProjectMembers { get; set; }
        public ICollection<Channel> Channels { get; set; }
    }
}
