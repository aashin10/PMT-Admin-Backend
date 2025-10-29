using AutoMapper;
using MediatR;
using PmtAdmin.Application.Dto;
using PmtAdmin.Application.Query.Users;
using PmtAdmin.Application.Wrappers;
using PmtAdmin.Domain.Persistance;
using System.Threading;
using System.Threading.Tasks;

namespace PmtAdmin.Application.Handlers.Users
{
    public class GetUsersWithPaginationQueryHandler : IRequestHandler<GetUsersWithPaginationQuery, ApiResponse<PaginatedResponse<UserDto>>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public GetUsersWithPaginationQueryHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<ApiResponse<PaginatedResponse<UserDto>>> Handle(GetUsersWithPaginationQuery request, CancellationToken cancellationToken)
        {
            // Validate pagination parameters
            var pageNumber = request.PageNumber < 1 ? 1 : request.PageNumber;
            var pageSize = request.PageSize < 1 ? 10 : (request.PageSize > 100 ? 100 : request.PageSize); // Max 100 per page

            var sortBy = string.IsNullOrWhiteSpace(request.SortBy) ? "name" : request.SortBy.ToLower();
            var sortOrder = string.IsNullOrWhiteSpace(request.SortOrder) ? "asc" : request.SortOrder.ToLower();

            // Get paginated users from repository
            var (users, totalCount) = await _userRepository.GetUsersWithPaginationAsync(
                pageNumber,
                pageSize,
                sortBy,
                sortOrder,
                request.Type,
                request.Status
            );

            // Map to DTOs
            var userDtos = _mapper.Map<System.Collections.Generic.List<UserDto>>(users);

            // Create paginated response
            var paginatedResponse = new PaginatedResponse<UserDto>(
                userDtos,
                pageNumber,
                pageSize,
                totalCount
            );

            return ApiResponse<PaginatedResponse<UserDto>>.Success(paginatedResponse, "Users retrieved successfully");
        }
    }
}
