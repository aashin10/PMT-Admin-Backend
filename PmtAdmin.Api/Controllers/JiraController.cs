using Microsoft.AspNetCore.Mvc;
using PmtAdmin.Infrastructure.Services.Jira;
using System.Net;
using static PmtAdmin.Infrastructure.Models.JiraImportModels;

namespace PmtAdmin.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JiraController : ControllerBase
    {
        private readonly IJiraService _jiraService;
        private readonly IJiraDatabaseService _jiraDatabaseService;
        public JiraController(IJiraService jiraService, IJiraDatabaseService jiraDatabaseService)
        {
            _jiraService = jiraService;
            _jiraDatabaseService = jiraDatabaseService;
        }


        [HttpGet("import/{baseUrl}")]
        public async Task<IActionResult> ImportProjects(string baseUrl, [FromQuery] string projectIds)
        {
            string decodedUrl = WebUtility.UrlDecode(baseUrl);
            string jiraToken = Request.Headers["Jira-Access-Token"].ToString();

            var ids = projectIds.Split(','); // Split comma-separated IDs
            var results = new List<JiraProjectData>();

            foreach (var id in ids)
            {
                var project = await _jiraService.GetFullProjectDataAsync(decodedUrl, jiraToken, id.Trim());
                results.Add(project);
            }
            await _jiraDatabaseService.PopulateDataBase(results);
            return Ok(results);
        }
    }
}
