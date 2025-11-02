using MediatR;
using PmtAdmin.Application.Dto;
using PmtAdmin.Application.Query.Projects;
using PmtAdmin.Application.Wrappers;
using PmtAdmin.Domain.Persistance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PmtAdmin.Application.Handlers.Projects
{
    /// <summary>
    /// Handler for GetTeamMembersByTeamQuery
    /// Retrieves all members of a specific team within a project with their details
    /// </summary>
    public class GetTeamMembersByTeamQueryHandler : IRequestHandler<GetTeamMembersByTeamQuery, ApiResponse<TeamMembersResponseDto>>
    {
        private readonly IProjectRepository _projectRepository;

        public GetTeamMembersByTeamQueryHandler(IProjectRepository projectRepository)
        {
            _projectRepository = projectRepository;
        }

        public async Task<ApiResponse<TeamMembersResponseDto>> Handle(GetTeamMembersByTeamQuery request, CancellationToken cancellationToken)
        {
            // Validate input
            if (request.TeamId <= 0)
            {
                return ApiResponse<TeamMembersResponseDto>.Fail("Invalid team ID");
            }

            if (request.ProjectId == Guid.Empty)
            {
                return ApiResponse<TeamMembersResponseDto>.Fail("Invalid project ID");
            }

            // Get the team with members from repository
            var team = await _projectRepository.GetTeamWithMembersAsync(request.TeamId, request.ProjectId);

            if (team == null)
            {
                return ApiResponse<TeamMembersResponseDto>.NotFound("Team not found in the specified project");
            }

            // Map the team to response DTO
            var response = new TeamMembersResponseDto
            {
                TeamId = team.Id,
                TeamName = team.Name,
                TeamDescription = team.Description,
                IsActive = team.IsActive,
                LeadName = team.Lead?.User?.Name,
                MemberCount = team.TeamMembers?.Count ?? 0,
                Members = team.TeamMembers?.Select(tm => new TeamMemberDTO
                {
                    Id = tm.ProjectMember?.Id ?? 0,
                    Name = tm.ProjectMember?.User?.Name,
                    Role = tm.ProjectMember?.Role?.Name,
                    Email = tm.ProjectMember?.User?.Email,
                    UserId = tm.ProjectMember?.UserId,
                    IsOwner = tm.ProjectMember?.IsOwner,
                    AddedAt = tm.ProjectMember?.AddedAt
                }).ToList() ?? new List<TeamMemberDTO>()
            };

            return ApiResponse<TeamMembersResponseDto>.Success(response, "Team members retrieved successfully");
        }
    }
}
