using AutoMapper;
using MediatR;
using PmtAdmin.Application.Command.Settings;
using PmtAdmin.Application.Dto.SettingsDTO;
using PmtAdmin.Application.Wrappers;
using PmtAdmin.Domain.Entities;
using PmtAdmin.Domain.Persistance.Settings;
using PmtAdmin.Application.CustomException;
using PmtAdmin.Application.Interfaces;
using PmtAdmin.Application.Utilities;
using System;
using System.Threading.Tasks;

namespace PmtAdmin.Application.Handlers.Settings
{
    public class CreateSuperAdminCommandHandler : IRequestHandler<CreateSuperAdminCommand, ApiResponse<SuperAdminDto>>
    {
        private readonly ISuperAdminRepository _superAdminRepository;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;

        public CreateSuperAdminCommandHandler(
            ISuperAdminRepository superAdminRepository, 
            IMapper mapper,
            IEmailService emailService)
        {
            _superAdminRepository = superAdminRepository;
            _mapper = mapper;
            _emailService = emailService;
        }

        public async Task<ApiResponse<SuperAdminDto>> Handle(CreateSuperAdminCommand request, CancellationToken cancellationToken)
        {
            // Basic validation
            if (request == null)
                throw new ValidationException("Request cannot be null");

            if (string.IsNullOrWhiteSpace(request.Email))
                throw new ValidationException("Email is required");

            // Generate password and hash
            var (password, passwordHash) = PasswordGenerator.GeneratePasswordFromUserInfo(request.Email, request.Name);

            // Map command to User entity
            var user = _mapper.Map<User>(request);

            // Set additional properties
            user.IsSuperAdmin = true;
            user.IsActive = true;
            user.IsDeleted = false;
            user.CreatedAt = DateTime.UtcNow;
            user.CreatedBy = request.CreatedBy;
            user.PasswordHash = passwordHash;

            // Save entity
            var savedUser = await _superAdminRepository.CreateAsync(user);

            // Send email with credentials
            var emailBody = $@"
                <h2>Welcome to PMT Admin</h2>
                <p>Dear {request.Name},</p>
                <p>Your Super Admin account has been created successfully.</p>
                <p>Here are your login credentials:</p>
                <p>Email: {request.Email}</p>
                <p>Password: {password}</p>
                <p>Please change your password after your first login.</p>
                <p>Best regards,<br>PMT Admin Team</p>";

            await _emailService.SendEmailAsync(
                request.Email,
                "PMT Admin - Your Super Admin Account Credentials",
                emailBody);

            // Map entity to DTO
            var dto = _mapper.Map<SuperAdminDto>(savedUser);

            return ApiResponse<SuperAdminDto>.Created(dto, "SuperAdmin created successfully");
        }
    }
}
