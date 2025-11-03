using BACKEND_CQRS.Application.Wrapper;
using MediatR;
using PmtAdmin.Application.Wrappers;

namespace BACKEND_CQRS.Application.Command
{
    public class LogoutCommand : IRequest<ApiResponse<bool>>
    {
        public int UserId { get; set; }
    }
}
