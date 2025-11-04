using BACKEND_CQRS.Application.Dto;

using MediatR;
using PmtAdmin.Application.Wrappers;

namespace BACKEND_CQRS.Application.Command
{
    public class LoginCommand : IRequest<ApiResponse<LoginResponseDto>>
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
