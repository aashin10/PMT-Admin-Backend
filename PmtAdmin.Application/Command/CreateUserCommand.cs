using MediatR;
using PmtAdmin.Application.Dto;
using PmtAdmin.Application.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PmtAdmin.Application.Command
{
    public class CreateUserCommand : IRequest<ApiResponse<UserDto>>
    {
        public string? Email { get; set; }
        public string? PasswordHash { get; set; }
        public string? Name { get; set; }
        public string? AvatarUrl { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsSuperAdmin { get; set; } = false;
        public string? JiraId { get; set; }
        public string? Type { get; set; }
        public int? CreatedBy { get; set; }
    }
}
