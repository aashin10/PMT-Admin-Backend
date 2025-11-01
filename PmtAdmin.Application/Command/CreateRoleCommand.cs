using MediatR;
using PmtAdmin.Application.Wrappers;
using RoleManagement.Application.Queries;
using System.Collections.Generic;

namespace RoleManagement.Application.Commands
{
    public class CreateRoleCommand : IRequest<ApiResponse<RoleDto>>
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Metadata { get; set; }
        public List<int>? PermissionIds { get; set; }
    }
}
