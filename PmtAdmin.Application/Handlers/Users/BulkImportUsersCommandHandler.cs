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
using System.Text;
using System.Threading.Tasks;

namespace PmtAdmin.Application.Handlers.Users
{
    public class BulkImportUsersCommandHandler : IRequestHandler<BulkImportUsersCommand, ApiResponse<BulkImportResultDto>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IPasswordHashingService _passwordHashingService;

        public BulkImportUsersCommandHandler(IUserRepository userRepository, IMapper mapper, IPasswordHashingService passwordHashingService)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _passwordHashingService = passwordHashingService;
        }

        public async Task<ApiResponse<BulkImportResultDto>> Handle(BulkImportUsersCommand request, CancellationToken cancellationToken)
        {
            var result = new BulkImportResultDto
            {
                TotalProcessed = request.Users.Count
            };

            var createdUsers = new List<User>();

            foreach (var userDto in request.Users)
            {
                // Skip users with "Suspended" status
                if (!string.IsNullOrWhiteSpace(userDto.Status) &&
                    userDto.Status.Trim().Equals("Suspended", StringComparison.OrdinalIgnoreCase))
                {
                    result.Skipped.Add($"Suspended user skipped: {userDto.Name} ({userDto.Email})");
                    result.SkippedCount++;
                    continue;
                }

                // Validate required fields
                if (string.IsNullOrWhiteSpace(userDto.Name))
                {
                    result.Errors.Add($"Name is required for user with email: {userDto.Email ?? "unknown"}");
                    result.ErrorCount++;
                    continue;
                }

                // Validate email format if provided
                if (!string.IsNullOrWhiteSpace(userDto.Email))
                {
                    if (!IsValidEmail(userDto.Email))
                    {
                        result.Errors.Add($"Invalid email format: {userDto.Email}");
                        result.ErrorCount++;
                        continue;
                    }

                    // Check if user with this email already exists
                    var existingUserByEmail = await _userRepository.GetByEmailAsync(userDto.Email);
                    if (existingUserByEmail != null)
                    {
                        result.Duplicates.Add($"Email already exists: {userDto.Email}");
                        result.DuplicateCount++;
                        continue;
                    }
                }

                // Check if Jira ID already exists (if provided)
                if (!string.IsNullOrWhiteSpace(userDto.JiraId))
                {
                    var existingUserByJiraId = await _userRepository.GetByJiraIdAsync(userDto.JiraId);
                    if (existingUserByJiraId != null)
                    {
                        result.Duplicates.Add($"Jira ID already exists: {userDto.JiraId} for user: {userDto.Name}");
                        result.DuplicateCount++;
                        continue;
                    }
                }

                // Infer type from email domain (null if no email)
                var type = string.IsNullOrEmpty(userDto.Email) ? null : InferTypeFromEmail(userDto.Email);

                // Map Status to IsActive boolean
                // "Active" -> true
                // "Inactive" -> false
                // "Suspended" users are already skipped above
                bool isActive = MapStatusToIsActive(userDto.Status);

                // Extract name parts
                var (firstName, lastName) = ExtractNameParts(userDto.Name);

                // Generate avatar URL
                var avatarUrl = GenerateAvatarUrl(firstName, lastName);

                // Generate password: [lastname]@experionglobal.123
                var password = $"{lastName}@experionglobal.123";
                var passwordHash = _passwordHashingService.HashPassword(password);

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
                    Type = type,
                    CreatedBy = request.CreatedBy,
                    CreatedAt = DateTime.UtcNow,
                    IsDeleted = false
                };

                try
                {
                    var savedUser = await _userRepository.CreateAsync(user);
                    createdUsers.Add(savedUser);
                    result.SuccessCount++;
                }
                catch (Exception ex)
                {
                    result.Errors.Add($"Error creating user {userDto.Name}: {ex.Message}");
                    result.ErrorCount++;
                }
            }

            // Map created users to DTOs
            result.CreatedUsers = _mapper.Map<List<UserDto>>(createdUsers);

            // Build response message
            var message = BuildResultMessage(result);

            return ApiResponse<BulkImportResultDto>.Success(result, message);
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

            var normalizedStatus = status.Trim();

            // "Active" -> true
            // "Inactive" -> false
            // Note: "Suspended" users are skipped before reaching this method
            return normalizedStatus.Equals("Active", StringComparison.OrdinalIgnoreCase);
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

        private string BuildResultMessage(BulkImportResultDto result)
        {
            var messageParts = new List<string>
            {
                $"Processed {result.TotalProcessed} users",
                $"Successfully imported: {result.SuccessCount}"
            };

            if (result.SkippedCount > 0)
            {
                messageParts.Add($"Suspended users skipped: {result.SkippedCount}");
            }

            if (result.DuplicateCount > 0)
            {
                messageParts.Add($"Duplicates skipped: {result.DuplicateCount}");
            }

            if (result.ErrorCount > 0)
            {
                messageParts.Add($"Errors: {result.ErrorCount}");
            }

            return string.Join(". ", messageParts);
        }
    }
}
