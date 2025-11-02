using MediatR;
using PmtAdmin.Application.Dto;

namespace PmtAdmin.Application.Query
{
    public class GetRoleByIdQuery : IRequest<RoleDto>
    {
        public int Id { get; set; }
    }
}