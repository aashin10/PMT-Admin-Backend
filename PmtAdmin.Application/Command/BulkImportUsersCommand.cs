using MediatR;
using PmtAdmin.Application.Dto;
using PmtAdmin.Application.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PmtAdmin.Application.Command
{
    public class BulkImportUsersCommand : IRequest<ApiResponse<BulkImportResultDto>>
    {
        public List<BulkImportUserDto> Users { get; set; } = new List<BulkImportUserDto>();
        public int? CreatedBy { get; set; }
    }

    public class BulkImportUserDto
    {
        public string? JiraId { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Status { get; set; }  // "Active", "Inactive", or "Suspended"
    }
}
