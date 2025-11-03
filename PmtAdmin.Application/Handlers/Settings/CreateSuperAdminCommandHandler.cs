using AutoMapper;
using MediatR;
using PmtAdmin.Application.Command.Settings;
using PmtAdmin.Application.Dto.SettingsDTO;
using PmtAdmin.Application.Wrappers;
using PmtAdmin.Domain.Entities;
using PmtAdmin.Domain.Persistance.Settings;
using PmtAdmin.Application.CustomException;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PmtAdmin.Application.Handlers.Settings
{
    public class CreateSuperAdminCommandHandler : IRequestHandler<CreateSuperAdminCommand, ApiResponse<SuperAdminDto>>
    {
        private readonly ISuperAdminRepository _superAdminRepository;
        private readonly IMapper _mapper;

        public CreateSuperAdminCommandHandler(ISuperAdminRepository superAdminRepository, IMapper mapper)
        {
            _superAdminRepository = superAdminRepository;
            _mapper = mapper;
        }

        public async Task<ApiResponse<SuperAdminDto>> Handle(CreateSuperAdminCommand request, CancellationToken cancellationToken)
        {
            // Basic validation
            if (request == null)
                throw new ValidationException("Request cannot be null");

            if (string.IsNullOrWhiteSpace(request.Email))
                throw new ValidationException("Email is required");

            // Map command to User entity
            var user = _mapper.Map<User>(request);

            // Set IsSuperAdmin to true
            user.IsSuperAdmin = true;
            user.IsActive = true;
            user.IsDeleted = false;
            user.CreatedAt = DateTime.UtcNow;

            // Add the CreatedBy field
            user.CreatedBy = request.CreatedBy;

            // Save entity
            var savedUser = await _superAdminRepository.CreateAsync(user);

            // Map entity to DTO
            var dto = _mapper.Map<SuperAdminDto>(savedUser);

            return ApiResponse<SuperAdminDto>.Created(dto, "SuperAdmin created successfully");
        }
    }
}
