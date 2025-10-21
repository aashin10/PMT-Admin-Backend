using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PmtAdmin.Domain.Entities
{
    [Table("users")]
    public class Users
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("email")]
        public string? Email { get; set; }

        [Column("password_hash")]
        public string? Password_Hash { get; set; }

        [Column("name")]
        public string? Name { get; set; }

        [Column("avatar_url")]
        public string? Avatar_Url { get; set; }

        [Column("is_active")]
        public bool Is_Active { get; set; } = true;

        [Column("is_super_admin")]
        public bool Is_Super_Admin { get; set; } = false;

        [Column("last_login")]
        public DateTime? Last_Login { get; set; }

        [Column("created_at")]
        public DateTime Created_At { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime? Updated_At { get; set; }

        [Column("deleted_at")]
        public DateTime? Deleted_At { get; set; }

        [Column("created_by")]
        public int? Created_By { get; set; }

        [Column("updated_by")]
        public int? Updated_By { get; set; }

        [Column("deleted_by")]
        public int? Deleted_By { get; set; }

        [Column("is_deleted")]
        public bool Is_Deleted { get; set; } = false;

        [Column("jira_id")]
        public string? Jira_Id { get; set; }

        [Column("type")]
        public string? Type { get; set; }

        // Navigation properties for foreign keys
        [ForeignKey("Created_By")]
        public Users? CreatedByUser { get; set; }

        [ForeignKey("Updated_By")]
        public Users? UpdatedByUser { get; set; }

        [ForeignKey("Deleted_By")]
        public Users? DeletedByUser { get; set; }
    }
}
