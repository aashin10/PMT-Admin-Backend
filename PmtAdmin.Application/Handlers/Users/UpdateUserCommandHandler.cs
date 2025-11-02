using AutoMapper;
using MediatR;
using PmtAdmin.Application.Command;
using PmtAdmin.Application.Constants;
using PmtAdmin.Application.CustomException;
using PmtAdmin.Application.Dto;
using PmtAdmin.Application.Wrappers;
using PmtAdmin.Domain.Persistance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PmtAdmin.Application.Handlers.Users
{
    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, ApiResponse<UserDto>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UpdateUserCommandHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<ApiResponse<UserDto>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            // Get the existing user
            var existingUser = await _userRepository.GetById(request.Id);
            if (existingUser == null)
            {
                throw new NotFoundException($"User with ID {request.Id} not found");
            }

            // Check if user is deleted
            if (existingUser.IsDeleted)
            {
                throw new NotFoundException($"User with ID {request.Id} has been deleted");
            }

            // Check if Email is being changed and if it already exists
            if (!string.IsNullOrEmpty(request.Email) && request.Email != existingUser.Email)
            {
                var userWithEmail = await _userRepository.GetByEmailAsync(request.Email);
                if (userWithEmail != null && userWithEmail.Id != request.Id)
                {
                    throw new DuplicateEntryException($"Email '{request.Email}' already exists for another user");
                }
            }

            // Check if JiraId is being changed and if it already exists
            if (!string.IsNullOrEmpty(request.JiraId) && request.JiraId != existingUser.JiraId)
            {
                var userWithJiraId = await _userRepository.GetByJiraIdAsync(request.JiraId);
                if (userWithJiraId != null && userWithJiraId.Id != request.Id)
                {
                    throw new DuplicateEntryException($"Jira ID '{request.JiraId}' already exists for another user");
                }
            }

            // Update only the editable fields
            if (request.Email != null)
            {
                existingUser.Email = string.IsNullOrWhiteSpace(request.Email) ? null : request.Email;

                // Automatically set type based on email domain
                if (!string.IsNullOrWhiteSpace(request.Email))
                {
                    existingUser.Type = request.Email.EndsWith("@experionglobal.com", StringComparison.OrdinalIgnoreCase)
                        ? "Internal"
                        : "External";
                }
                else
                {
                    // If email is cleared, set type to null
                    existingUser.Type = null;
                }
            }

            if (request.JiraId != null)
            {
                existingUser.JiraId = string.IsNullOrWhiteSpace(request.JiraId) ? null : request.JiraId;
            }

            if (request.Type != null)
            {
                existingUser.Type = request.Type;
            }

            if (request.IsActive.HasValue)
            {
                existingUser.IsActive = request.IsActive.Value;
            }

            existingUser.UpdatedAt = DateTime.UtcNow;
            existingUser.UpdatedBy = request.UpdatedBy;

            // Update the user
            await _userRepository.UpdateAsync(existingUser);

            // Map to DTO
            var userDto = _mapper.Map<UserDto>(existingUser);

            return ApiResponse<UserDto>.Success(userDto, "User updated successfully");
        }
    }
}
