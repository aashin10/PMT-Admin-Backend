using MediatR;
using PmtAdmin.Application.Dto;
using PmtAdmin.Application.Wrappers;

namespace PmtAdmin.Application.Command
{
    public class JiraCsvImportUsersCommand : IRequest<ApiResponse<JiraCsvImportResultDto>>
    {
        public List<JiraCsvUserDto> Users { get; set; } = new List<JiraCsvUserDto>();
    }

    public class JiraCsvUserDto
    {
        public string? JiraId { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Status { get; set; }

    }
}
