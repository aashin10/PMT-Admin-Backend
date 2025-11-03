using AutoMapper;
using MediatR;
using PmtAdmin.Application.Command.Settings;
using PmtAdmin.Application.Dto.SettingsDTO;
using PmtAdmin.Application.Wrappers;
using PmtAdmin.Domain.Persistance.Settings;
using PmtAdmin.Application.CustomException;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PmtAdmin.Application.Handlers.Settings
{
    public class UpdateSuperAdminCommandHandler : IRequestHandler<UpdateSuperAdminCommand, ApiResponse<SuperAdminDto>>
    {
        private readonly ISuperAdminRepository _superAdminRepository;
        private readonly IMapper _mapper;

        public UpdateSuperAdminCommandHandler(ISuperAdminRepository superAdminRepository, IMapper mapper)
        {
            _superAdminRepository = superAdminRepository;
            _mapper = mapper;
        }

        public async Task<ApiResponse<SuperAdminDto>> Handle(UpdateSuperAdminCommand request, CancellationToken cancellationToken)
        {
            // Get existing user
            var existingUser = await _superAdminRepository.GetByIdAsync(request.Id);

            if (existingUser == null)
                return ApiResponse<SuperAdminDto>.NotFound("SuperAdmin not found");

            // Check if the user is actually a super admin
            if (!existingUser.IsSuperAdmin)
                return ApiResponse<SuperAdminDto>.Fail("User is not a SuperAdmin");

            // Check if user is deleted
            if (existingUser.IsDeleted)
                return ApiResponse<SuperAdminDto>.Fail("Cannot update deleted SuperAdmin");

            // Update fields if provided
            if (!string.IsNullOrEmpty(request.Name))
                existingUser.Name = request.Name;

            if (!string.IsNullOrEmpty(request.Email))
                existingUser.Email = request.Email;

            if (request.IsActive.HasValue)
                existingUser.IsActive = request.IsActive.Value;

            // Set updated metadata
            existingUser.UpdatedAt = DateTime.UtcNow;
            existingUser.UpdatedBy = request.UpdatedBy; // If you want to track who updated it

            // Save changes
            try
            {
                var updatedUser = await _superAdminRepository.UpdateAsync(existingUser);

                // Map to DTO
                var dto = _mapper.Map<SuperAdminDto>(updatedUser);

                return ApiResponse<SuperAdminDto>.Success(dto, "SuperAdmin updated successfully");
            }
            catch (DuplicateEntryException dex)
            {
                return ApiResponse<SuperAdminDto>.Fail(dex.Message);
            }
        }
    }
}
