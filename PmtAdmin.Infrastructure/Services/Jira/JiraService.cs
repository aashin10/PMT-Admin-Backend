using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using static PmtAdmin.Infrastructure.Models.JiraImportModels;

namespace PmtAdmin.Infrastructure.Services.Jira
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



        public async Task<List<JiraIssue>> GetIssuesByBoardIdAsync(string baseUrl, string token, string boardId)
        {
            var client = JiraServiceFactory.CreateClient(baseUrl, token);

            var request = new RestRequest($"/rest/agile/1.0/board/{boardId}/issue?jql=issuetype != Epic", Method.Get);
            request.AddHeader("Accept", "application/json");

            var response = await client.ExecuteAsync(request);

            if (string.IsNullOrEmpty(response.Content))
                return new List<JiraIssue>();

            var json = JObject.Parse(response.Content);
            var issuesArray = json["issues"];

            var issues = new List<JiraIssue>();

            if (issuesArray != null)
            {
                foreach (var item in issuesArray)
                {
                    var fields = item["fields"];

                    var issue = new JiraIssue
                    {
                        Key = item["key"]?.ToString(),
                        Summary = fields?["summary"]?.ToString(),
                        Assignee = fields?["assignee"].ToObject<JiraUser>(),
                        Labels = fields?["labels"]?.ToObject<List<string>>(),
                        Reporter = fields?["reporter"].ToObject<JiraUser>(),
                        Creator = fields?["creator"].ToObject<JiraUser>(),
                        Comment = fields?["comment"]?["comments"]?.ToObject<List<JiraComment>>(),
                        Team = fields?["customfield_10001"]?.ToObject<JiraTeam>(),
                        UpdatedAt = fields?["updated"] != null ? DateTime.Parse(fields["updated"].ToString()) : DateTime.MinValue,
                        Status = fields?["status"]?["statusCategory"]?.ToObject<JiraStatus>(),
                        Priority = fields?["priority"]?.ToObject<JiraPriority>(),
                        Sprint = fields?["sprint"]?.ToObject<JiraSprint>(),
                        Epic = fields?["parent"]?.ToObject<JiraEpic>()
                    };

                    issues.Add(issue);
                }
            }

            return issues;
        }

        public async Task<List<JiraEpic>> GetEpicsByBoardIdAsync(string baseUrl, string token, string boardId)
        {
            var client = JiraServiceFactory.CreateClient(baseUrl, token);

            var request = new RestRequest($"/rest/agile/1.0/board/{boardId}/epic", Method.Get);
            request.AddHeader("Accept", "application/json");

            var response = await client.ExecuteAsync(request);

            if (string.IsNullOrEmpty(response.Content))
                return new List<JiraEpic>();

            var json = JObject.Parse(response.Content);
            var epics = json["values"]?.ToObject<List<JiraEpic>>() ?? new List<JiraEpic>();

            return epics;
        }

        public async Task<List<JiraUser>> GetUsersByRoleAsync(string roleUrl, string token)
        {
            var client = JiraServiceFactory.CreateClient(baseUrl: "", token);
            var request = new RestRequest(roleUrl, Method.Get);
            request.AddHeader("Accept", "application/json");

            var response = await client.ExecuteAsync(request);

            if (string.IsNullOrEmpty(response.Content))
                return new List<JiraUser>();

            var json = JObject.Parse(response.Content);


            var users = json["actors"]?
                .Select(a => new JiraUser
                {
                    AccountId = a["actorUser"]?["accountId"]?.ToString(),
                    DisplayName = a["displayName"]?.ToString()
                })
                .Where(u => u.AccountId != null)
                .ToList() ?? new List<JiraUser>();


            return users;
        }

        public async Task<JiraProjectData> GetFullProjectDataAsync(string baseUrl, string token, string projectIdOrKey)
        {
            var project = await ImportProjectByIdAsync(baseUrl, token, projectIdOrKey);
            var boards = await GetBoardsByProjectIdAsync(baseUrl, token, projectIdOrKey);

            Dictionary<string, List<JiraUser>> boardRolesDict = new Dictionary<string, List<JiraUser>>();

            //Import roles and user associated with each role

            //Ignore certain roles for apps and bots
            HashSet<string> IgnoreRoles = new HashSet<string>() { "atlassian-addons-project-access" };

            foreach (var roleEntry in project.Roles)
            {
                if (IgnoreRoles.Contains(roleEntry.Key))
                    continue;

                var roleUrl = roleEntry.Value;
                var usersInRole = await GetUsersByRoleAsync(roleUrl, token);
                boardRolesDict[roleEntry.Key] = usersInRole;
            }


            var boardDetailsList = new List<BoardWithDetails>();

            foreach (var board in boards)
            {
                var sprints = await GetSprintsByBoardIdAsync(baseUrl, token, board.Id);
                var issues = await GetIssuesByBoardIdAsync(baseUrl, token, board.Id.ToString());
                var epics = await GetEpicsByBoardIdAsync(baseUrl, token, board.Id.ToString());

                boardDetailsList.Add(new BoardWithDetails
                {
                    BoardInfo = board,
                    Sprints = sprints,
                    Issues = issues,
                    Epics = epics
                });
            }

            return new JiraProjectData
            {
                Project = project,
                Boards = boardDetailsList,
                UsersByRole = boardRolesDict
            };
        }

    }
}
