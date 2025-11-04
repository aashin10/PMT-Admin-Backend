using AutoMapper;
using MediatR;
using PmtAdmin.Application.Command.Settings;
using PmtAdmin.Application.CustomException;
using PmtAdmin.Application.Dto.SettingsDTO;
using PmtAdmin.Application.Wrappers;
using PmtAdmin.Domain.Persistance.Settings;

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
            var existingUser = await _superAdminRepository.GetByIdAsync(request.Id)
                ?? throw new NotFoundException($"Super admin with ID {request.Id} not found");

            // Update fields if they are provided
            if (!string.IsNullOrWhiteSpace(request.Name))
                existingUser.Name = request.Name;
            
            if (!string.IsNullOrWhiteSpace(request.Email))
                existingUser.Email = request.Email;
            
            if (request.IsActive.HasValue)
                existingUser.IsActive = request.IsActive.Value;

            // Add tracking fields
            existingUser.UpdatedAt = DateTime.UtcNow;
            existingUser.UpdatedBy = request.UpdatedBy;

            var updatedUser = await _superAdminRepository.UpdateAsync(existingUser);
            var superAdminDto = _mapper.Map<SuperAdminDto>(updatedUser);
            
            return ApiResponse<SuperAdminDto>.Success(superAdminDto, "Super admin updated successfully");
        }
    }
}
