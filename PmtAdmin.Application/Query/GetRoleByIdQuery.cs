using MediatR;
using PmtAdmin.Application.Dto;

namespace RoleManagement.Application.Queries
{
    public class GetRoleByIdQuery : IRequest<RoleDto>
    {
        public int Id { get; set; }
    }
}