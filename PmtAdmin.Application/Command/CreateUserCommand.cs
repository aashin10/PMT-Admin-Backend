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
    public class CreateUserCommand : IRequest<ApiResponse<List<UserDto>>>
    {
        public List<CreateUserDto> Users { get; set; } = new List<CreateUserDto>();
    }

    public class CreateUserDto
    {
        public string? Email { get; set; }
        public string? Name { get; set; }
        public string? JiraId { get; set; }
        public string? Type { get; set; }  // "Internal" or "External"
        public string? Status { get; set; }  // "Active", "Inactive", or "Suspended"
        public int? CreatedBy { get; set; }
    }
}
