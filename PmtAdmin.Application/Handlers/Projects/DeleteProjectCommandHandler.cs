using MediatR;
using PmtAdmin.Application.Command.Projects;
using PmtAdmin.Application.Wrappers;
using PmtAdmin.Domain.Persistance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PmtAdmin.Application.Handlers.Projects
{
    public class DeleteProjectCommandHandler : IRequestHandler<DeleteProjectCommand, ApiResponse<string>>
    {
        private readonly IProjectRepository _projectRepository;

        public DeleteProjectCommandHandler(IProjectRepository projectRepository)
        {
            _projectRepository = projectRepository;
        }

        public async Task<ApiResponse<string>> Handle(DeleteProjectCommand request, CancellationToken cancellationToken)
        {
            var deleted = await _projectRepository.SoftDeleteProjectAsync(request.Id);

            if (!deleted)
            {
                return ApiResponse<string>.NotFound("Project not found");
            }

            return ApiResponse<string>.Success("Project deleted successfully");
        }
    }
}
