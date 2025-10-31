using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PmtAdmin.Application.Dto
{
    public class PaginatedUserResponseDto
    {
        public List<UserDto> Users { get; set; } = new List<UserDto>();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public bool HasNextPage { get; set; }
        public bool HasPreviousPage { get; set; }
        public PaginationMetadata Metadata { get; set; } = new PaginationMetadata();
    }

    public class PaginationMetadata
    {
        public string? SortBy { get; set; }
        public string? SortOrder { get; set; }
        public string? Type { get; set; }
        public string? Status { get; set; }
        public string? SearchTerm { get; set; }
    }
}
