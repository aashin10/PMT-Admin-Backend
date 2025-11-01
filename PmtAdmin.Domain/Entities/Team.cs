using System.ComponentModel.DataAnnotations.Schema;

namespace PmtAdmin.Domain.Entities
{


        [Table("teams")]
        public class Team
        {

                [Key]
                [Column("id")]
                [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

                public int Id { get; set; }

                [Column("project_id")]
                public Guid ProjectId { get; set; }

                [Column("name")]
                public string Name { get; set; } = string.Empty;

                [Column("description")]
                public string? Description { get; set; }

                // 🔹 LeadId now references ProjectMembers.Id
                [Column("lead_id")]
                public int? LeadId { get; set; }

                [Column("is_active")]
                public bool? IsActive { get; set; } = true;

                [Column("created_by")]
                public int? CreatedBy { get; set; }

                [Column("updated_by")]
                public int? UpdatedBy { get; set; }

                [Column("created_at")]
                public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;

                [Column("updated_at")]
                public DateTime? UpdatedAt { get; set; }

                [Column("labels", TypeName = "text[]")]
                public List<string>? Label { get; set; } = new();



                // Navigation properties
                [ForeignKey("ProjectId")]
                public Project Project { get; set; }

                // 🔹 Lead is now a ProjectMember, not a User
                [ForeignKey("LeadId")]
                //public User? Lead { get; set; }
                public ProjectMember? Lead { get; set; }

                // ✅ These remain linked to the Users table
                [ForeignKey("CreatedBy")]
                //public User? Creator { get; set; }
                public ProjectMember? Creator { get; set; }

                [ForeignKey("UpdatedBy")]
                //public User? Updater { get; set; }
                public ProjectMember? Updater { get; set; }

                [NotMapped]
                public int MemberCount { get; set; }

                [NotMapped]
                public int ActiveSprintCount { get; set; }

                public ICollection<TeamMember> TeamMembers { get; set; } = new List<TeamMember>();

                public ICollection<Board> Boards { get; set; }
                //public ICollection<ProjectMember> ProjectMembers { get; set; }
                public ICollection<Channel> Channels { get; set; }
                public ICollection<Sprint>? Sprints { get; set; }

        }
}



//public User? Lead { get; set; }
//public User? Creator { get; set; }
//public User? Updater { get; set; }