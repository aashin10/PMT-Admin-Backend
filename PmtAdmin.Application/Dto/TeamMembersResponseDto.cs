using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PmtAdmin.Application.Dto
{
    /// <summary>
    /// Response DTO for team members query
    /// Contains team information and all members with their details and count
    /// </summary>
    public class TeamMembersResponseDto
    {
        /// <summary>
        /// The ID of the team
        /// </summary>
        public int TeamId { get; set; }

        /// <summary>
        /// The name of the team
        /// </summary>
        public string? TeamName { get; set; }

        /// <summary>
        /// The description of the team
        /// </summary>
        public string? TeamDescription { get; set; }

        /// <summary>
        /// Total count of members in the team
        /// </summary>
        public int MemberCount { get; set; }

        /// <summary>
        /// List of all team members with their details
        /// </summary>
        public List<TeamMemberDTO> Members { get; set; } = new();

        /// <summary>
        /// Name of the team lead
        /// </summary>
        public string? LeadName { get; set; }

        /// <summary>
        /// Whether the team is currently active
        /// </summary>
        public bool IsActive { get; set; }
    }
}
