using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using static PmtAdmin.Infrastructure.Models.JiraImportModels;

namespace PmtAdmin.Infrastructure.Services
{
    public class JiraService : IJiraService
    {
        public async Task<JiraProject> ImportProjectByIdAsync(string baseUrl, string token, string projectIdOrKey)
        {
            var client = JiraServiceFactory.CreateClient(baseUrl, token);
            var request = new RestRequest($"/rest/api/3/project/{projectIdOrKey}", Method.Get);
            request.AddHeader("Accept", "application/json");

            var response = await client.ExecuteAsync(request);
            var project = JsonConvert.DeserializeObject<JiraProject>(response.Content);

            return project;
        }

        public async Task<List<JiraBoard>> GetBoardsByProjectIdAsync(string baseUrl, string token, string projectId)
        {
            var client = JiraServiceFactory.CreateClient(baseUrl, token);
            var request = new RestRequest("/rest/agile/1.0/board", Method.Get);
            request.AddHeader("Accept", "application/json");

            if (!string.IsNullOrEmpty(projectId))
            {
                request.AddParameter("projectKeyOrId", projectId);
            }

            var response = await client.ExecuteAsync(request);

            if (string.IsNullOrEmpty(response.Content))
                return new List<JiraBoard>();

            var json = JObject.Parse(response.Content);
            var boards = json["values"]?.ToObject<List<JiraBoard>>() ?? new List<JiraBoard>();

            return boards;

        }

        public async Task<List<JiraSprint>> GetSprintsByBoardIdAsync(string baseUrl, string token, int boardId)
        {
            var client = JiraServiceFactory.CreateClient(baseUrl, token);
            var request = new RestRequest($"/rest/agile/1.0/board/{boardId}/sprint", Method.Get);
            request.AddHeader("Accept", "application/json");

            var response = await client.ExecuteAsync(request);

            if (string.IsNullOrEmpty(response.Content))
                return new List<JiraSprint>();

            var json = JObject.Parse(response.Content);

            var sprints = json["values"]?.ToObject<List<JiraSprint>>() ?? new List<JiraSprint>();

            return sprints;
        }
    }
}
