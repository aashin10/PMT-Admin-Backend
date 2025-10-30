using MediatR;
using PmtAdmin.Application.Dto;
using PmtAdmin.Application.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PmtAdmin.Application.Query
{
    public class GetUsersWithPaginationQuery : IRequest<ApiResponse<PaginatedUserResponseDto>>
    {
        // Pagination
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        // Sorting
        public string? SortBy { get; set; } = "name";  // Default sort by name
        public string? SortOrder { get; set; } = "asc";  // "asc" or "desc"

        // Filtering
        public string? Type { get; set; }  // "Internal" or "External"
        public string? Status { get; set; }  // "Active" or "Inactive"
        public string? SearchTerm { get; set; }  // Search by name or email
    }
}
