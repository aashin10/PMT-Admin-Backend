using AutoMapper;
using MediatR;
using PmtAdmin.Application.Command;
using PmtAdmin.Application.Dto;
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
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, ApiResponse<UserDto>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public CreateUserCommandHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<ApiResponse<UserDto>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            // Validate required fields
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return ApiResponse<UserDto>.Fail("Name is required");
            }

            if (string.IsNullOrWhiteSpace(request.Email))
            {
                return ApiResponse<UserDto>.Fail("Email is required");
            }

            // Validate email format
            if (!IsValidEmail(request.Email))
            {
                return ApiResponse<UserDto>.Fail("Invalid email format");
            }

            // Check if user with this email already exists
            var existingUserByEmail = await _userRepository.GetByEmailAsync(request.Email);
            if (existingUserByEmail != null)
            {
                return ApiResponse<UserDto>.Fail("Email already exists");
            }

            // Check if Jira ID already exists (if provided)
            if (!string.IsNullOrWhiteSpace(request.Jira_Id))
            {
                var existingUserByJiraId = await _userRepository.GetByJiraIdAsync(request.Jira_Id);
                if (existingUserByJiraId != null)
                {
                    return ApiResponse<UserDto>.Fail("Jira ID already exists");
                }
            }

            var user = _mapper.Map<Domain.Entities.Users>(request);
            user.Created_At = DateTime.UtcNow;

            var savedUser = await _userRepository.CreateAsync(user);

            var dto = _mapper.Map<UserDto>(savedUser);
            return ApiResponse<UserDto>.Created(dto, "User created successfully");
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
