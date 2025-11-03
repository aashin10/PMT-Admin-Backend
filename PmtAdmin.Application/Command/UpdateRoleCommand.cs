using MediatR;
using System.Collections.Generic;
using PmtAdmin.Application.Wrappers;
using PmtAdmin.Application.Dto;

namespace PmtAdmin.Application.Command
{
    public class UpdateRoleCommand : IRequest<ApiResponse<RoleDto>>
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Metadata { get; set; }
        public List<int>? PermissionIds { get; set; }
    }
}