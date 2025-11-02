using MediatR;
using PmtAdmin.Application.Dto;
using PmtAdmin.Application.Wrappers;
using System.Collections.Generic;

namespace PmtAdmin.Application.Command
{
    public class CreateRoleCommand : IRequest<ApiResponse<RoleDto>>
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Metadata { get; set; }
        public List<int>? PermissionIds { get; set; }
    }
}
