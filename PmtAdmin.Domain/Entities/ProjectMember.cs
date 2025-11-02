using PmtAdmin.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PmtAdmin.Domain.Entities
{
    // ============================================
    // PROJECT MEMBERS
    // ============================================
    [Table("project_members")]
    public class ProjectMember
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("project_id")]
        public Guid ProjectId { get; set; }

        [Column("user_id")]
        public int? UserId { get; set; }

        [Column("role_id")]
        public int? RoleId { get; set; }

        [Column("is_owner")]
        public bool? IsOwner { get; set; }

        [Column("added_at")]
        public DateTimeOffset? AddedAt { get; set; }


        [Column("added_by")]
        public int? AddedBy { get; set; }

        // Navigation properties
        [ForeignKey("UserId")]
        public User? User { get; set; }

        [ForeignKey("ProjectId")]
        public Project? Project { get; set; }

        [ForeignKey("RoleId")]
        public Role? Role { get; set; }

        [ForeignKey("AddedBy")]
        public User? Users { get; set; }



    }

}
