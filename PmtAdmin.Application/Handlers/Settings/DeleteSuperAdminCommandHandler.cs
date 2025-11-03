using AutoMapper;
using MediatR;
using PmtAdmin.Application.Command.Settings;
using PmtAdmin.Application.Dto.SettingsDTO;
using PmtAdmin.Application.Wrappers;
using PmtAdmin.Domain.Persistance.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PmtAdmin.Application.Handlers.Settings
{
    public class DeleteSuperAdminCommandHandler : IRequestHandler<DeleteSuperAdminCommand, ApiResponse<SuperAdminDto>>
    {
        private readonly ISuperAdminRepository _superAdminRepository;
        private readonly IMapper _mapper;

        public DeleteSuperAdminCommandHandler(ISuperAdminRepository superAdminRepository, IMapper mapper)
        {
            _superAdminRepository = superAdminRepository;
            _mapper = mapper;
        }

        public async Task<ApiResponse<SuperAdminDto>> Handle(DeleteSuperAdminCommand request, CancellationToken cancellationToken)
        {
            // Get the user by ID
            var existingUser = await _superAdminRepository.GetByIdAsync(request.Id);

            if (existingUser == null)
                return ApiResponse<SuperAdminDto>.Fail("SuperAdmin not found");

            // Check if the user is actually a super admin
            if (!existingUser.IsSuperAdmin)
                return ApiResponse<SuperAdminDto>.Fail("User is not a SuperAdmin");

            // Check if already deleted
            if (existingUser.IsDeleted)
                return ApiResponse<SuperAdminDto>.Fail("SuperAdmin already deleted");

            // Soft delete: Set IsDeleted to true and DeletedAt timestamp
            existingUser.IsDeleted = true;
            existingUser.DeletedAt = DateTime.UtcNow;
            existingUser.DeletedBy = request.DeletedBy; // If you want to track who deleted it

            // Update the entity
            await _superAdminRepository.UpdateAsync(existingUser);

            // Map to DTO
            var dto = _mapper.Map<SuperAdminDto>(existingUser);

            return ApiResponse<SuperAdminDto>.Success(dto, "SuperAdmin deleted successfully");
        }
    }
}
