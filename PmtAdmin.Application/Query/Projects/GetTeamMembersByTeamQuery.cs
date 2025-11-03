using MediatR;
using PmtAdmin.Application.Dto;
using PmtAdmin.Application.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PmtAdmin.Application.Query.Projects
{
    /// <summary>
    /// Query to fetch all members of a specific team within a project
    /// Includes member count and detailed member information
    /// </summary>
    public class GetTeamMembersByTeamQuery : IRequest<ApiResponse<TeamMembersResponseDto>>
    {
        /// <summary>
        /// The ID of the team
        /// </summary>
        public int TeamId { get; set; }

        /// <summary>
        /// The ID of the project containing the team
        /// </summary>
        public Guid ProjectId { get; set; }
    }
}
