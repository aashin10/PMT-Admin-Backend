using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PmtAdmin.Application.Dto
{
    public class TeamMemberDTO
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Role { get; set; }
        public string? Email { get; set; }
        public int? UserId { get; set; }
        public bool? IsOwner { get; set; }
        public DateTimeOffset? AddedAt { get; set; }
    }
}