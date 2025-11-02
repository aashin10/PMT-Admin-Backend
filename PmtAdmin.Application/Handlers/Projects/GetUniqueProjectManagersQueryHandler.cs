using MediatR;
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
    public class GetUniqueProjectManagersQueryHandler : IRequestHandler<GetUniqueProjectManagersQuery, ApiResponse<IReadOnlyList<ProjectManagerInfo>>>
    {
        private readonly IProjectRepository _projectRepository;

        public GetUniqueProjectManagersQueryHandler(IProjectRepository projectRepository)
        {
            _projectRepository = projectRepository;
        }

        public async Task<ApiResponse<IReadOnlyList<ProjectManagerInfo>>> Handle(GetUniqueProjectManagersQuery request, CancellationToken cancellationToken)
        {
            var projectManagers = await _projectRepository.GetUniqueProjectManagersAsync();

            if (projectManagers == null || !projectManagers.Any())
            {
                return ApiResponse<IReadOnlyList<ProjectManagerInfo>>.Success(
                    new List<ProjectManagerInfo>(),
                    "No project managers found"
                );
            }

            return ApiResponse<IReadOnlyList<ProjectManagerInfo>>.Success(
                projectManagers,
                $"Retrieved {projectManagers.Count} unique project managers"
            );
        }
    }
}