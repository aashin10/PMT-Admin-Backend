using AutoMapper;
using MediatR;
using PmtAdmin.Application.Command;
using PmtAdmin.Application.Dto;
using PmtAdmin.Application.Services;
using PmtAdmin.Application.Wrappers;
using PmtAdmin.Domain.Entities;
using PmtAdmin.Domain.Persistance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PmtAdmin.Application.Handlers.Users
{
    public class ImportUsersFromCsvCommandHandler : IRequestHandler<ImportUsersFromCsvCommand, ApiResponse<List<UserDto>>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IPasswordHashingService _passwordHashingService;

        public ImportUsersFromCsvCommandHandler(IUserRepository userRepository, IMapper mapper, IPasswordHashingService passwordHashingService)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _passwordHashingService = passwordHashingService;
        }

        public async Task<ApiResponse<List<UserDto>>> Handle(ImportUsersFromCsvCommand request, CancellationToken cancellationToken)
        {
            var createdUsers = new List<User>();
            var errors = new List<string>();

            foreach (var csvUser in request.Users)
            {
                // Validate required fields
                if (string.IsNullOrWhiteSpace(csvUser.Name))
                {
                    errors.Add($"Name is required for user with email: {csvUser.Email ?? "unknown"}");
                    continue;
                }

                if (string.IsNullOrWhiteSpace(csvUser.Email))
                {
                    errors.Add($"Email is required for user: {csvUser.Name}");
                    continue;
                }

                // Validate email format
                if (!IsValidEmail(csvUser.Email))
                {
                    errors.Add($"Invalid email format: {csvUser.Email}");
                    continue;
                }

                // Check if user with this email already exists
                var existingUserByEmail = await _userRepository.GetByEmailAsync(csvUser.Email);
                if (existingUserByEmail != null)
                {
                    errors.Add($"Email already exists: {csvUser.Email}");
                    continue;
                }

                // Check if Jira ID already exists (if provided)
                if (!string.IsNullOrWhiteSpace(csvUser.JiraId))
                {
                    var existingUserByJiraId = await _userRepository.GetByJiraIdAsync(csvUser.JiraId);
                    if (existingUserByJiraId != null)
                    {
                        errors.Add($"Jira ID already exists: {csvUser.JiraId}");
                        continue;
                    }
                }

                // Infer Type from email domain
                var type = InferTypeFromEmail(csvUser.Email);

                // Normalize Status to capitalize first letter (default to "Active" if not provided)
                var normalizedStatus = string.IsNullOrWhiteSpace(csvUser.Status)
                    ? "Active"
                    : NormalizeEnum(csvUser.Status);

                // Map Status to IsActive: "Active" -> true, "Inactive"/"Suspended" -> false
                var isActive = normalizedStatus.Equals("Active", StringComparison.OrdinalIgnoreCase);

                // Extract name parts
                var (firstName, lastName) = ExtractNameParts(csvUser.Name);

                // Generate avatar URL
                var avatarUrl = GenerateAvatarUrl(firstName, lastName);

                // Generate password: [lastname]@experionglobal.123
                var password = $"{lastName}@experionglobal.123";
                var passwordHash = _passwordHashingService.HashPassword(password);

                // Create user entity
                var user = new User
                {
                    Email = csvUser.Email,
                    Name = csvUser.Name,
                    PasswordHash = passwordHash,
                    AvatarUrl = avatarUrl,
                    IsActive = isActive,
                    Status = normalizedStatus,  // Set the Status field
                    IsSuperAdmin = false,
                    JiraId = csvUser.JiraId,
                    Type = type,
                    CreatedBy = csvUser.CreatedBy,
                    CreatedAt = DateTime.UtcNow,
                    IsDeleted = false
                };

                var savedUser = await _userRepository.CreateAsync(user);
                createdUsers.Add(savedUser);
            }

            // If there are errors and no users were created, return error
            if (errors.Any() && !createdUsers.Any())
            {
                return ApiResponse<List<UserDto>>.Fail(string.Join("; ", errors));
            }

            var userDtos = _mapper.Map<List<UserDto>>(createdUsers);

            // Return success with warnings if some failed
            var message = createdUsers.Count == request.Users.Count
                ? $"All {createdUsers.Count} users imported successfully from CSV"
                : $"{createdUsers.Count} of {request.Users.Count} users imported. Errors: {string.Join("; ", errors)}";

            return ApiResponse<List<UserDto>>.Created(userDtos, message);
        }

        /// <summary>
        /// Infer user type from email domain.
        /// Type is "Internal" if email domain is experionglobal.com, else "External"
        /// </summary>
        private string InferTypeFromEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return "External";

            var domain = email.Split('@').LastOrDefault()?.Trim().ToLower();

            return domain == "experionglobal.com" ? "Internal" : "External";
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
    }
}
