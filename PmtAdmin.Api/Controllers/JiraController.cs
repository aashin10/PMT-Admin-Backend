using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PmtAdmin.Application.Command;
using PmtAdmin.Application.Dto;
using PmtAdmin.Application.Wrappers;
using System.Net;
using static PmtAdmin.Domain.Models.JiraImportModels;


namespace PmtAdmin.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class JiraController : ControllerBase
    {
        private readonly IMediator _mediator;

        public JiraController(IMediator mediator)
        {
            _mediator = mediator;
        }


        [HttpGet("import/{baseUrl}")]
        public async Task<ApiResponse<JiraImportResult>> ImportProjects(string baseUrl, [FromQuery] string projectIds)
        {

            string decodedUrl = WebUtility.UrlDecode(baseUrl);
            string jiraToken = Request.Headers["Jira-Access-Token"].ToString();

            var ids = projectIds.Split(','); // Split comma-separated IDs
            var results = new List<JiraProjectData>();

            var users = await _mediator.Send(new ImportFromJiraCommand
            {
                BaseUrl = decodedUrl,
                ProjectIds = ids,
                JiraAccessToken = jiraToken
            });

            return users;
        }
    }
}
