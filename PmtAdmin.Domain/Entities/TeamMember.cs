using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PmtAdmin.Domain.Entities
{
    [Table("team_members")]
    public class TeamMember
    {
        [Column("team_member_id")]
        public long TeamMemberId { get; set; }

        [Column("team_id")]
        public int TeamId { get; set; }

        [Column("project_member_id")]
        public int ProjectMemberId { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; }

        // No 'virtual' keyword — plain references
        [ForeignKey(nameof(TeamId))]
        public Team? Team { get; set; }
        [ForeignKey(nameof(ProjectMemberId))]
        public ProjectMember? ProjectMember { get; set; }
    }
}
