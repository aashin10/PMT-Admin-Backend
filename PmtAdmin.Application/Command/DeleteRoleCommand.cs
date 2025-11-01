using MediatR;
using PmtAdmin.Application.Wrappers;
namespace RoleManagement.Application.Commands
{
    public class DeleteRoleCommand : IRequest<ApiResponse<string>>
    {
        public int Id { get; set; }
    }
}