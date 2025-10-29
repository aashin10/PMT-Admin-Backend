using MediatR;
using PmtAdmin.Application.Dto;
using PmtAdmin.Application.Wrappers;

namespace PmtAdmin.Application.Query.Users
{
    public class GetUsersWithPaginationQuery : IRequest<ApiResponse<PaginatedResponse<UserDto>>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SortBy { get; set; } = "name";  // Default sort by name
        public string? SortOrder { get; set; } = "asc";  // "asc" or "desc"
        public string? Type { get; set; }  // Optional: "Internal" or "External"
        public string? Status { get; set; }  // Optional: "Active", "Inactive", or "Suspended"
    }
}
