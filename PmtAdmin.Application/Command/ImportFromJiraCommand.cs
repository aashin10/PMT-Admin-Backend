using MediatR;
using PmtAdmin.Application.Dto;
using PmtAdmin.Application.Wrappers;

namespace PmtAdmin.Application.Command
{
    public class ImportFromJiraCommand : IRequest<ApiResponse<List<UserDto>>>
    {
        public string BaseUrl { get; set; } = string.Empty;

        public string[] ProjectIds { get; set; }

        public String JiraAccessToken { get; set; } = string.Empty;
    }
}
