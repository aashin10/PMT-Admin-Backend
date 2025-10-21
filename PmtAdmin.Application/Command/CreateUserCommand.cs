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
        public string? Password_Hash { get; set; }
        public string? Name { get; set; }
        public string? Avatar_Url { get; set; }
        public bool Is_Active { get; set; } = true;
        public bool Is_Super_Admin { get; set; } = false;
        public string? Jira_Id { get; set; }
        public string? Type { get; set; }
        public int? Created_By { get; set; }
    }
}
