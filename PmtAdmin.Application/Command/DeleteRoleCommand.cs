using MediatR;
using PmtAdmin.Application.Wrappers;

namespace PmtAdmin.Application.Command
{
    public class DeleteRoleCommand : IRequest<ApiResponse<string>>
    {
        public int Id { get; set; }
    }
}