using MediatR;
using PmtAdmin.Application.Dto;
using System.Collections.Generic;

namespace RoleManagement.Application.Queries
{
    public class GetRolesQuery : IRequest<List<RoleDto>>
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}