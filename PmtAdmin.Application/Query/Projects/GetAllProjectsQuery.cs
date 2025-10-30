using MediatR;
using PmtAdmin.Application.Dto;
using PmtAdmin.Application.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PmtAdmin.Application.Query.Projects
{
    public class GetAllProjectsQuery : IRequest<ApiResponse<PaginatedResponse<ProjectTableDTO>>>
    {
        // Pagination
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        // Search
        public string? SearchTerm { get; set; }

        // Multi-select Filters (changed from int? to List<int>)
        public List<int>? StatusIds { get; set; }
        public List<int>? DeliveryUnitIds { get; set; }
        public List<int>? ProjectManagerIds { get; set; }
    }
}
