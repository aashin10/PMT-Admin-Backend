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
    public class UpdateUserCommand : IRequest<ApiResponse<UserDto>>
    {
        public int Id { get; set; }
        public string? Email { get; set; }
        public string? JiraId { get; set; }
        public string? Type { get; set; }
        public bool? IsActive { get; set; }
        public int? UpdatedBy { get; set; }
    }
}
