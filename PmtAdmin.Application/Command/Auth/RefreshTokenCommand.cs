using BACKEND_CQRS.Application.Dto;
using MediatR;
using PmtAdmin.Application.Wrappers;

namespace BACKEND_CQRS.Application.Command
{
    public class RefreshTokenCommand : IRequest<ApiResponse<LoginResponseDto>>
    {
        public string RefreshToken { get; set; }
    }
}
