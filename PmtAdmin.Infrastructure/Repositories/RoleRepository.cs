using Microsoft.EntityFrameworkCore;
using PmtAdmin.Application.Wrappers;
using PmtAdmin.Domain.Entities;
using PmtAdmin.Domain.Persistance;
using PmtAdmin.Infrastructure.Context;
using RoleManagement.Application.Queries;

namespace PmtAdmin.Infrastructure.Repositories
{
    public class RoleRepository : GenericRepository<Role>, IRoleRepository
    {
        private readonly AppDbContext _context;

        public RoleRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<Role>> GetAllAsync()
        {
            return await _context.Set<Role>()
                .Include(r => r.RolePermissions!)
                    .ThenInclude(rp => rp.Permission)
                .Include(r => r.ProjectMembers)
                .ToListAsync();
        }

        //public override async Task<Role?> GetById(int id)
        //{
        //    return await _context.Set<Role>()
        //        .Include(r => r.RolePermissions!)
        //            .ThenInclude(rp => rp.Permission)
        //        .Include(r => r.ProjectMembers)
        //        .FirstOrDefaultAsync(r => r.Id == id);
        //}

        // Better async version
        public async Task<ApiResponse<RoleDto>> GetRoleDtoByIdAsync(int roleId)
        {
            var role = await GetById(roleId);
            if (role == null)
            {
                return ApiResponse<RoleDto>.Fail("Role not found");
            }

            var roleDto = new RoleDto
            {
                Id = role.Id,
                Name = role.Name,
                Description = role.Description,
                Metadata = role.Metadata,
                CreatedAt = role.CreatedAt.ToString("o"),
                UserCount = role.ProjectMembers?.Count ?? 0,
                Permissions = role.RolePermissions?.Select(rp => new PermissionDto
                {
                    Id = rp.Permission!.Id,
                    Name = rp.Permission.Name,
                    Description = rp.Permission.Description
                }).ToList() ?? new List<PermissionDto>()
            };
            Console.WriteLine($"Fetched RoleDto: {roleDto.Name} with {roleDto.UserCount} users.");
            return ApiResponse<RoleDto>.Success(roleDto);
        }

        // Change the method signature to 'new' instead of 'override' since the base method is not virtual
        public new async Task<Role> UpdateAsync(Role entity)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.Set<Role>().Update(entity);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return entity;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task UpdateRolePermissionsAsync(Role role, IEnumerable<int> newPermissionIds)
        {
            // Load current RolePermissions from the database
            var currentRolePermissions = _context.Set<RolePermission>()
                .Where(rp => rp.RoleId == role.Id);

            _context.Set<RolePermission>().RemoveRange(currentRolePermissions);

            // Add new RolePermission entries
            foreach (var permissionId in newPermissionIds)
            {
                _context.Set<RolePermission>().Add(new RolePermission
                {
                    RoleId = role.Id,
                    PermissionId = permissionId,
                    CreatedAt = DateTime.UtcNow
                });
            }

            await _context.SaveChangesAsync();
        }
    }
}
