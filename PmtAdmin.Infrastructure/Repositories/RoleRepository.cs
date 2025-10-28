using PmtAdmin.Application.Constants;
using PmtAdmin.Application.Dto;
using PmtAdmin.Application.Wrappers;
using PmtAdmin.Domain.Entities;
using PmtAdmin.Domain.Persistance;
using PmtAdmin.Infrastructure.Context;
using RoleManagement.Application.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PmtAdmin.Infrastructure.Repositories
{
    public class RoleRepository : GenericRepository<Role>, IRoleRepository
    {
        public RoleRepository(AppDbContext context) : base(context)
        {
        }

        public ApiResponse<RoleDto> GetRoleById(int roleId)
        {
            var role = GetById(roleId);
            if (role == null)
            {
                return ApiResponse<RoleDto>.Fail("Role not found", StatusCode.NotFound);
            }

            var roleDto = new RoleDto
            {
                // Map properties from role to roleDto
            };

            return ApiResponse<RoleDto>.Success(roleDto);
        }
    }
}
