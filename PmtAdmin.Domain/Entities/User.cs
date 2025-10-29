using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace PmtAdmin.Domain.Entities
{

    // ============================================
    // USERS
    // ============================================
    [Table("users")]
    public class User
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        [Column("email")]
        public string Email { get; set; }

        [MaxLength(1024)]
        [Column("password_hash")]
        public string? PasswordHash { get; set; }

        [MaxLength(150)]
        [Column("name")]
        public string? Name { get; set; }

        [MaxLength(1000)]
        [Column("avatar_url")]
        public string? AvatarUrl { get; set; }

        [Column("is_active")]
        public bool IsActive { get; set; } = true;

        [MaxLength(50)]
        [Column("status")]
        public string? Status { get; set; } = "Active";  // "Active", "Inactive", or "Suspended"

        [Column("is_super_admin")]
        public bool IsSuperAdmin { get; set; } = false;

        [Column("last_login")]
        public DateTime? LastLogin { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; }

        [Column("deleted_at")]
        public DateTime? DeletedAt { get; set; }

        [MaxLength(1024)]
        [Column("jira_id")]
        public string? JiraId { get; set; }

        [Column("type")]
        public string? Type { get; set; }

        [Column("created_by")]
        public int? CreatedBy { get; set; }

        [Column("deleted_by")]
        public int? DeletedBy { get; set; }

        [Column("updated_by")]
        public int? UpdatedBy { get; set; }

        [Column("is_deleted")]
        public bool IsDeleted { get; set; } = false;

        // Navigation properties
        [ForeignKey("CreatedBy")]
        public User? CreatedByUser { get; set; }

        [ForeignKey("DeletedBy")]
        public User? DeletedByUser { get; set; }

        [ForeignKey("UpdatedBy")]
        public User? UpdatedByUser { get; set; }

        public ICollection<ProjectMember> ProjectMembers { get; set; }
        public ICollection<Project> ManagedProjects { get; set; }
        public ICollection<Team> LeadTeams { get; set; }
        public ICollection<DeliveryUnit> ManagedDeliveryUnits { get; set; }


        //[Table("users")]
        //public class Users
        //{
        //    [Column("id")]
        //    public int Id { get; set; }

        //    [Column("email")]
        //    public string? Email { get; set; }

        //    [Column("password_hash")]
        //    public string? Password_Hash { get; set; }

        //    [Column("name")]
        //    public string? Name { get; set; }

        //    [Column("avatar_url")]
        //    public string? Avatar_Url { get; set; }

        //    [Column("is_active")]
        //    public bool Is_Active { get; set; } = true;

        //    [Column("is_super_admin")]
        //    public bool Is_Super_Admin { get; set; } = false;

        //    [Column("last_login")]
        //    public DateTime? Last_Login { get; set; }

        //    [Column("created_at")]
        //    public DateTime Created_At { get; set; } = DateTime.UtcNow;

        //    [Column("updated_at")]
        //    public DateTime? Updated_At { get; set; }

        //    [Column("deleted_at")]
        //    public DateTime? Deleted_At { get; set; }

        //    [Column("created_by")]
        //    public int? Created_By { get; set; }

        //    [Column("updated_by")]
        //    public int? Updated_By { get; set; }

        //    [Column("deleted_by")]
        //    public int? Deleted_By { get; set; }

        //    [Column("is_deleted")]
        //    public bool Is_Deleted { get; set; } = false;

        //    [Column("jira_id")]
        //    public string? Jira_Id { get; set; }

        //    [Column("type")]
        //    public string? Type { get; set; }

        //    // Navigation properties for foreign keys
        //    [ForeignKey("Created_By")]
        //    public Users? CreatedByUser { get; set; }

        //    [ForeignKey("Updated_By")]
        //    public Users? UpdatedByUser { get; set; }

        //    [ForeignKey("Deleted_By")]
        //    public Users? DeletedByUser { get; set; }
        //}
    }
}