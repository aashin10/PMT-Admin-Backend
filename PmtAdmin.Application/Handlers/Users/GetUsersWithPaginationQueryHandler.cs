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
    public class GetUsersWithPaginationQueryHandler : IRequestHandler<GetUsersWithPaginationQuery, ApiResponse<PaginatedUserResponseDto>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public GetUsersWithPaginationQueryHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<ApiResponse<PaginatedUserResponseDto>> Handle(GetUsersWithPaginationQuery request, CancellationToken cancellationToken)
        {
            // Validate page and pageSize
            if (request.Page < 1)
                request.Page = 1;

            if (request.PageSize < 1)
                request.PageSize = 10;

            if (request.PageSize > 100)
                request.PageSize = 100;  // Max page size limit

            // Get paginated users from repository
            var (users, totalCount) = await _userRepository.GetUsersWithPaginationAsync(
                request.Page,
                request.PageSize,
                request.SortBy,
                request.SortOrder,
                request.Type,
                request.Status,
                request.SearchTerm
            );

            // Map to DTOs
            var userDtos = _mapper.Map<List<UserDto>>(users);

            // Calculate pagination metadata
            var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);

            var response = new PaginatedUserResponseDto
            {
                Users = userDtos,
                TotalCount = totalCount,
                Page = request.Page,
                PageSize = request.PageSize,
                TotalPages = totalPages,
                HasNextPage = request.Page < totalPages,
                HasPreviousPage = request.Page > 1,
                Metadata = new PaginationMetadata
                {
                    SortBy = request.SortBy,
                    SortOrder = request.SortOrder,
                    Type = request.Type,
                    Status = request.Status,
                    SearchTerm = request.SearchTerm
                }
            };

            return ApiResponse<PaginatedUserResponseDto>.Success(response, $"Retrieved {userDtos.Count} users (Page {request.Page} of {totalPages})");
        }
    }
}
