using AutoMapper;
using MediatR;
using PmtAdmin.Application.Command;
using PmtAdmin.Application.Dto;
using PmtAdmin.Application.Interfaces;
using PmtAdmin.Application.Services;
using PmtAdmin.Application.Wrappers;
using PmtAdmin.Domain.Entities;
using PmtAdmin.Domain.Persistance;

namespace PmtAdmin.Application.Handlers.Users
{
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, ApiResponse<List<UserDto>>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IPasswordHashingService _passwordHashingService;
        private readonly IEmailService _emailService;

        public CreateUserCommandHandler(
            IUserRepository userRepository,
            IMapper mapper,
            IPasswordHashingService passwordHashingService,
            IEmailService emailService)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _passwordHashingService = passwordHashingService;
            _emailService = emailService;
        }

        public async Task<ApiResponse<List<UserDto>>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            var createdUsers = new List<User>();
            var errors = new List<string>();
            var emailErrors = new List<string>();

            foreach (var userDto in request.Users)
            {
                // Validate required fields
                if (string.IsNullOrWhiteSpace(userDto.Name))
                {
                    errors.Add($"Name is required for user with email: {userDto.Email ?? "unknown"}");
                    continue;
                }

                // Validate email format if provided
                if (!string.IsNullOrWhiteSpace(userDto.Email))
                {
                    if (!IsValidEmail(userDto.Email))
                    {
                        errors.Add($"Invalid email format: {userDto.Email}");
                        continue;
                    }

                    // Check if user with this email already exists
                    var existingUserByEmail = await _userRepository.GetByEmailAsync(userDto.Email);
                    if (existingUserByEmail != null)
                    {
                        errors.Add($"Email already exists: {userDto.Email}");
                        continue;
                    }
                }

                // Check if Jira ID already exists (if provided)
                if (!string.IsNullOrWhiteSpace(userDto.JiraId))
                {
                    var existingUserByJiraId = await _userRepository.GetByJiraIdAsync(userDto.JiraId);
                    if (existingUserByJiraId != null)
                    {
                        errors.Add($"Jira ID already exists: {userDto.JiraId}");
                        continue;
                    }
                }

                // Extract name parts
                var (firstName, lastName) = ExtractNameParts(userDto.Name);

                // Generate avatar URL
                var avatarUrl = GenerateAvatarUrl(firstName, lastName);

                // Generate password: [lastname]@experionglobal.123
                var password = $"{lastName}@experionglobal.123";
                var passwordHash = _passwordHashingService.HashPassword(password);

                // Normalize Type to capitalize first letter
                var normalizedType = NormalizeEnum(userDto.Type);

                // If Type is not provided, infer from email domain (null if no email)
                if (string.IsNullOrWhiteSpace(normalizedType))
                {
                    normalizedType = string.IsNullOrEmpty(userDto.Email) ? null : InferTypeFromEmail(userDto.Email);
                }

                // Map Status to IsActive boolean
                bool isActive = MapStatusToIsActive(userDto.Status);

                // Create user entity
                var user = new User
                {
                    Email = string.IsNullOrWhiteSpace(userDto.Email) ? null : userDto.Email,
                    Name = userDto.Name,
                    PasswordHash = passwordHash,
                    AvatarUrl = avatarUrl,
                    IsActive = isActive,
                    IsSuperAdmin = false,
                    JiraId = string.IsNullOrWhiteSpace(userDto.JiraId) ? null : userDto.JiraId,
                    Type = normalizedType,
                    //CreatedBy = userDto.CreatedBy,
                    CreatedAt = DateTime.UtcNow,
                    IsDeleted = false
                };

                var savedUser = await _userRepository.CreateAsync(user);
                createdUsers.Add(savedUser);

                // Send email with credentials (only if email exists)
                if (!string.IsNullOrWhiteSpace(savedUser.Email))
                {
                    try
                    {
                        var emailBody = $@"
                            <h2>Welcome to PMT </h2>
                            <p>Dear {savedUser.Name},</p>
                            <p>Your account has been created successfully.</p>
                            <p>Here are your login credentials:</p>
                            <p><strong>Email:</strong> {savedUser.Email}</p>
                            <p><strong>Password:</strong> {password}</p>
                            <p><strong>User Type:</strong> {savedUser.Type ?? "Not specified"}</p>
                            <p>Please change your password after your first login.</p>
                            <p>Best regards,<br>PMT Admin Team</p>";

                        await _emailService.SendEmailAsync(
                            savedUser.Email,
                            "PMT Admin - Your Account Credentials",
                            emailBody);
                    }
                    catch (Exception ex)
                    {
                        // Don't fail user creation if email fails, just track it
                        emailErrors.Add($"Failed to send email to {savedUser.Email}: {ex.Message}");
                    }
                }
            }

            // If there are errors and no users were created, return error
            if (errors.Any() && !createdUsers.Any())
            {
                return ApiResponse<List<UserDto>>.Fail(string.Join("; ", errors));
            }

            var userDtos = _mapper.Map<List<UserDto>>(createdUsers);

            // Return success with warnings if some failed
            var message = createdUsers.Count == request.Users.Count
                ? "All users created successfully"
                : $"{createdUsers.Count} of {request.Users.Count} users created. Errors: {string.Join("; ", errors)}";

            // Add email warnings to message if any
            if (emailErrors.Any())
            {
                message += $". Email warnings: {string.Join("; ", emailErrors)}";
            }

            return ApiResponse<List<UserDto>>.Created(userDtos, message);
        }

        private (string firstName, string lastName) ExtractNameParts(string fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName))
                return (string.Empty, string.Empty);

            var parts = fullName.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            var firstName = parts.Length > 0 ? parts[0] : string.Empty;
            var lastName = parts.Length > 1 ? parts[1] : firstName; // Use first name if no last name

            return (firstName, lastName);
        }

        private string GenerateAvatarUrl(string firstName, string lastName)
        {
            var username = string.IsNullOrWhiteSpace(lastName) || firstName == lastName
                ? firstName
                : $"{firstName}+{lastName}";

            return $"https://avatar.iran.liara.run/username?username={username}";
        }

        private string NormalizeEnum(string? value)
        {
            if (string.IsNullOrWhiteSpace(value)) return value ?? string.Empty;

            // Capitalize first letter, lowercase rest
            var trimmed = value.Trim();
            return char.ToUpper(trimmed[0]) + trimmed.Substring(1).ToLower();
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private string InferTypeFromEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return "External";

            // Check if email domain is experionglobal.com
            return email.EndsWith("@experionglobal.com", StringComparison.OrdinalIgnoreCase)
                ? "Internal"
                : "External";
        }

        private bool MapStatusToIsActive(string? status)
        {
            if (string.IsNullOrWhiteSpace(status))
                return true; // Default to Active

            var normalizedStatus = NormalizeEnum(status);

            // "Active" -> true
            // "Inactive" -> false
            // "Suspended" -> false (convert to Inactive)
            return normalizedStatus.Equals("Active", StringComparison.OrdinalIgnoreCase);
        }
    }
}










//using AutoMapper;
//using MediatR;
//using PmtAdmin.Application.Command;
//using PmtAdmin.Application.Dto;
//using PmtAdmin.Application.Services;
//using PmtAdmin.Application.Wrappers;
//using PmtAdmin.Domain.Entities;
//using PmtAdmin.Domain.Persistance;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace PmtAdmin.Application.Handlers.Users
//{
//    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, ApiResponse<List<UserDto>>>
//    {
//        private readonly IUserRepository _userRepository;
//        private readonly IMapper _mapper;
//        private readonly IPasswordHashingService _passwordHashingService;

//        public CreateUserCommandHandler(IUserRepository userRepository, IMapper mapper, IPasswordHashingService passwordHashingService)
//        {
//            _userRepository = userRepository;
//            _mapper = mapper;
//            _passwordHashingService = passwordHashingService;
//        }

//        public async Task<ApiResponse<List<UserDto>>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
//        {
//            var createdUsers = new List<User>();
//            var errors = new List<string>();

//            foreach (var userDto in request.Users)
//            {
//                // Validate required fields
//                if (string.IsNullOrWhiteSpace(userDto.Name))
//                {
//                    errors.Add($"Name is required for user with email: {userDto.Email ?? "unknown"}");
//                    continue;
//                }

//                // Validate email format if provided
//                if (!string.IsNullOrWhiteSpace(userDto.Email))
//                {
//                    if (!IsValidEmail(userDto.Email))
//                    {
//                        errors.Add($"Invalid email format: {userDto.Email}");
//                        continue;
//                    }

//                    // Check if user with this email already exists
//                    var existingUserByEmail = await _userRepository.GetByEmailAsync(userDto.Email);
//                    if (existingUserByEmail != null)
//                    {
//                        errors.Add($"Email already exists: {userDto.Email}");
//                        continue;
//                    }
//                }

//                // Check if Jira ID already exists (if provided)
//                if (!string.IsNullOrWhiteSpace(userDto.JiraId))
//                {
//                    var existingUserByJiraId = await _userRepository.GetByJiraIdAsync(userDto.JiraId);
//                    if (existingUserByJiraId != null)
//                    {
//                        errors.Add($"Jira ID already exists: {userDto.JiraId}");
//                        continue;
//                    }
//                }

//                // Extract name parts
//                var (firstName, lastName) = ExtractNameParts(userDto.Name);

//                // Generate avatar URL
//                var avatarUrl = GenerateAvatarUrl(firstName, lastName);

//                // Generate password: [lastname]@experionglobal.123
//                var password = $"{lastName}@experionglobal.123";
//                var passwordHash = _passwordHashingService.HashPassword(password);

//                // Normalize Type to capitalize first letter
//                var normalizedType = NormalizeEnum(userDto.Type);

//                // If Type is not provided, infer from email domain (null if no email)
//                if (string.IsNullOrWhiteSpace(normalizedType))
//                {
//                    normalizedType = string.IsNullOrEmpty(userDto.Email) ? null : InferTypeFromEmail(userDto.Email);
//                }

//                // Map Status to IsActive boolean
//                bool isActive = MapStatusToIsActive(userDto.Status);

//                // Create user entity
//                var user = new User
//                {
//                    Email = string.IsNullOrWhiteSpace(userDto.Email) ? null : userDto.Email,
//                    Name = userDto.Name,
//                    PasswordHash = passwordHash,
//                    AvatarUrl = avatarUrl,
//                    IsActive = isActive,
//                    IsSuperAdmin = false,
//                    JiraId = string.IsNullOrWhiteSpace(userDto.JiraId) ? null : userDto.JiraId,
//                    Type = normalizedType,
//                    CreatedBy = userDto.CreatedBy,
//                    CreatedAt = DateTime.UtcNow,
//                    IsDeleted = false
//                };

//                var savedUser = await _userRepository.CreateAsync(user);
//                createdUsers.Add(savedUser);
//            }

//            // If there are errors and no users were created, return error
//            if (errors.Any() && !createdUsers.Any())
//            {
//                return ApiResponse<List<UserDto>>.Fail(string.Join("; ", errors));
//            }

//            var userDtos = _mapper.Map<List<UserDto>>(createdUsers);

//            // Return success with warnings if some failed
//            var message = createdUsers.Count == request.Users.Count
//                ? "All users created successfully"
//                : $"{createdUsers.Count} of {request.Users.Count} users created. Errors: {string.Join("; ", errors)}";

//            return ApiResponse<List<UserDto>>.Created(userDtos, message);
//        }

//        private (string firstName, string lastName) ExtractNameParts(string fullName)
//        {
//            if (string.IsNullOrWhiteSpace(fullName))
//                return (string.Empty, string.Empty);

//            var parts = fullName.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

//            var firstName = parts.Length > 0 ? parts[0] : string.Empty;
//            var lastName = parts.Length > 1 ? parts[1] : firstName; // Use first name if no last name

//            return (firstName, lastName);
//        }

//        private string GenerateAvatarUrl(string firstName, string lastName)
//        {
//            var username = string.IsNullOrWhiteSpace(lastName) || firstName == lastName
//                ? firstName
//                : $"{firstName}+{lastName}";

//            return $"https://avatar.iran.liara.run/username?username={username}";
//        }

//        private string NormalizeEnum(string? value)
//        {
//            if (string.IsNullOrWhiteSpace(value)) return value ?? string.Empty;

//            // Capitalize first letter, lowercase rest
//            var trimmed = value.Trim();
//            return char.ToUpper(trimmed[0]) + trimmed.Substring(1).ToLower();
//        }

//        private bool IsValidEmail(string email)
//        {
//            try
//            {
//                var addr = new System.Net.Mail.MailAddress(email);
//                return addr.Address == email;
//            }
//            catch
//            {
//                return false;
//            }
//        }

//        private string InferTypeFromEmail(string email)
//        {
//            if (string.IsNullOrWhiteSpace(email))
//                return "External";

//            // Check if email domain is experionglobal.com
//            return email.EndsWith("@experionglobal.com", StringComparison.OrdinalIgnoreCase)
//                ? "Internal"
//                : "External";
//        }

//        private bool MapStatusToIsActive(string? status)
//        {
//            if (string.IsNullOrWhiteSpace(status))
//                return true; // Default to Active

//            var normalizedStatus = NormalizeEnum(status);

//            // "Active" -> true
//            // "Inactive" -> false
//            // "Suspended" -> false (convert to Inactive)
//            return normalizedStatus.Equals("Active", StringComparison.OrdinalIgnoreCase);
//        }
//    }
//}
