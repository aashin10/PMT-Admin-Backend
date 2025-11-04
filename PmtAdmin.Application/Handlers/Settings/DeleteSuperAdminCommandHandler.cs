using AutoMapper;
using MediatR;
using PmtAdmin.Application.Command.Settings;
using PmtAdmin.Application.CustomException;
using PmtAdmin.Application.Dto.SettingsDTO;
using PmtAdmin.Application.Wrappers;
using PmtAdmin.Domain.Persistance.Settings;
using System;
using System.Threading;
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
            var user = await _superAdminRepository.GetByIdAsync(request.Id)
                ?? throw new NotFoundException($"Super admin with ID {request.Id} not found");

            // Set deletion tracking fields
            user.DeletedAt = DateTime.UtcNow;
            user.DeletedBy = request.DeletedBy;
            user.IsDeleted = true;

            var deletedUser = await _superAdminRepository.SoftDeleteAsync(user);
            var superAdminDto = _mapper.Map<SuperAdminDto>(deletedUser);

            return ApiResponse<SuperAdminDto>.Success(superAdminDto, "Super admin deleted successfully");
        }
    }
}
