using MediatR;
using PmtAdmin.Application.Dto;
using PmtAdmin.Application.Wrappers;
using System.Collections.Generic;

namespace PmtAdmin.Application.Command
{
    public class ImportUsersFromCsvCommand : IRequest<ApiResponse<List<UserDto>>>
    {
        public List<CsvUserDto> Users { get; set; } = new List<CsvUserDto>();
    }

    public class CsvUserDto
    {
        public string? JiraId { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Status { get; set; }  // "Active", "Inactive", or "Suspended"
        public int? CreatedBy { get; set; }
    }
}
