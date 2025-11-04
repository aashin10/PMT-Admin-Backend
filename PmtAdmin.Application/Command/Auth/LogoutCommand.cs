using MediatR;
using PmtAdmin.Application.Wrappers;

namespace BACKEND_CQRS.Application.Command
{
    public class LogoutCommand : IRequest<ApiResponse<object>>
    {
        public int UserId { get; set; }
    }
}
