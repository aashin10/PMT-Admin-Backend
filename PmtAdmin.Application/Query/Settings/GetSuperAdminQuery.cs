using MediatR;
using PmtAdmin.Application.Dto.SettingsDTO;
using PmtAdmin.Application.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PmtAdmin.Application.Query.Settings
{
    public class GetSuperAdminQuery : IRequest<ApiResponse<List<SuperAdminDto>>>
    {
        // Pagination
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        // Sorting
        public string? SortBy { get; set; } = "Name";
        public string SortOrder { get; set; } = "asc";

        // Search
        public string? SearchQuery { get; set; }
    }
}
