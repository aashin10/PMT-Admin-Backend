using AutoMapper;
using MediatR;
using PmtAdmin.Application.Command;
using PmtAdmin.Application.Dto;
using PmtAdmin.Application.Wrappers;
using PmtAdmin.Domain.Entities;
using PmtAdmin.Domain.Persistance;

namespace PmtAdmin.Application.Handlers.Users
{
    public class JiraCsvImportUsersCommandHandler : IRequestHandler<JiraCsvImportUsersCommand, ApiResponse<JiraCsvImportResultDto>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public JiraCsvImportUsersCommandHandler(IUserRepository userRepository, IMapper mapper)
        {
            _mapper = mapper;
            _userRepository = userRepository;
        }
        public async Task<ApiResponse<JiraCsvImportResultDto>> Handle(JiraCsvImportUsersCommand request, CancellationToken cancellationToken)
        {
            var result = new JiraCsvImportResultDto
            {
                CreatedUsers = new List<UserDto>(),
            };

            foreach (var user in request.Users)
            {
                try
                {
                    var searchedUser = await _userRepository.GetByJiraIdAsync(user.JiraId);

                    if (searchedUser == null)
                    {
                        var newUser = _mapper.Map<User>(user);
                        await _userRepository.CreateAsync(newUser);
                        result.CreatedUsers.Add(_mapper.Map<UserDto>(newUser));
                    }
                    else
                    {
                        searchedUser.Name = user.Name;
                        searchedUser.Email = user.Email;
                        await _userRepository.UpdateAsync(searchedUser);
                    }

                }
                catch (Exception ex)
                {
                    return ApiResponse<JiraCsvImportResultDto>.Fail($"Error importing user with Jira ID {user.JiraId}: {ex.Message}");
                }
            }

            return ApiResponse<JiraCsvImportResultDto>.Success(result, "Jira users imported successfully");
        }
    }
}
