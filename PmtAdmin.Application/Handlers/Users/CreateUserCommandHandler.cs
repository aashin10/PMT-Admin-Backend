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
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, ApiResponse<List<UserDto>>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IPasswordHashingService _passwordHashingService;

        public CreateUserCommandHandler(IUserRepository userRepository, IMapper mapper, IPasswordHashingService passwordHashingService)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _passwordHashingService = passwordHashingService;
        }

        public async Task<ApiResponse<List<UserDto>>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            var createdUsers = new List<User>();
            var errors = new List<string>();

            foreach (var userDto in request.Users)
            {
                // Validate required fields
                if (string.IsNullOrWhiteSpace(userDto.Name))
                {
                    errors.Add($"Name is required for user with email: {userDto.Email ?? "unknown"}");
                    continue;
                }

                if (string.IsNullOrWhiteSpace(userDto.Email))
                {
                    errors.Add($"Email is required for user: {userDto.Name}");
                    continue;
                }

                // Validate email format
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

                // Create user entity
                var user = new User
                {
                    Email = userDto.Email,
                    Name = userDto.Name,
                    PasswordHash = passwordHash,
                    AvatarUrl = avatarUrl,
                    IsActive = true,
                    IsSuperAdmin = false,
                    JiraId = userDto.JiraId,
                    Type = normalizedType,
                    CreatedBy = userDto.CreatedBy,
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
                ? "All users created successfully"
                : $"{createdUsers.Count} of {request.Users.Count} users created. Errors: {string.Join("; ", errors)}";

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
    }
}
