using AutoMapper;
using MediatR;
using PmtAdmin.Application.Dto;
using PmtAdmin.Application.Query;
using PmtAdmin.Application.Wrappers;
using PmtAdmin.Domain.Persistance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PmtAdmin.Application.Handlers.Users
{
    public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, ApiResponse<List<UserDto>>>
    {
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;

        public GetAllUsersQueryHandler(IMapper mapper, IUserRepository userRepository)
        {
            _mapper = mapper;
            _userRepository = userRepository;
        }

        public async Task<ApiResponse<List<UserDto>>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
        {
            // Use filtered query if type or status is provided
            IReadOnlyList<Domain.Entities.User> users;

            if (!string.IsNullOrWhiteSpace(request.Type) || !string.IsNullOrWhiteSpace(request.Status))
            {
                users = await _userRepository.GetFilteredUsersAsync(request.Type, request.Status);
            }
            else
            {
                users = await _userRepository.GetAllNonDeletedUsersAsync();
            }

            if (users == null || !users.Any())
            {
                return ApiResponse<List<UserDto>>.Success(new List<UserDto>(), "No users found");
            }

            var userDtos = _mapper.Map<List<UserDto>>(users);

            return ApiResponse<List<UserDto>>.Success(userDtos);
        }
    }
}
