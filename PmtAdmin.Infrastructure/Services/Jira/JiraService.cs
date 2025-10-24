using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Polly;
using Polly.Retry;
using RestSharp;
using System.Net;
using static PmtAdmin.Infrastructure.Models.JiraImportModels;

namespace PmtAdmin.Infrastructure.Services.Jira
{
    public class JiraService : IJiraService
    {
        private readonly ILogger<JiraService> _logger;
        private readonly AsyncRetryPolicy _retryPolicy;
        private const int MaxRetries = 3;
        private const int MaxResultsPerPage = 50;

        public JiraService(ILogger<JiraService> logger)
        {
            _logger = logger;

            _retryPolicy = Policy
                .Handle<HttpRequestException>()
                .Or<TimeoutException>()
                .WaitAndRetryAsync(
                    MaxRetries,
                    retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    onRetry: (exception, timeSpan, retryCount, context) =>
                    {
                        _logger.LogWarning(
                            "Retry {RetryCount} after {Delay}s due to: {Exception}",
                            retryCount, timeSpan.TotalSeconds, exception.Message);
                    });
        }

        public async Task<JiraProject> ImportProjectByIdAsync(
            string baseUrl,
            string token,
            string projectIdOrKey)
        {
            return await _retryPolicy.ExecuteAsync(async () =>
            {
                try
                {
                    var client = JiraServiceFactory.CreateClient(baseUrl, token);
                    var request = new RestRequest($"/rest/api/3/project/{projectIdOrKey}", Method.Get);
                    request.AddHeader("Accept", "application/json");
                    request.AddParameter("expand", "description,lead,url,projectKeys");

                    _logger.LogInformation("Fetching project: {ProjectKey}", projectIdOrKey);
                    var response = await client.ExecuteAsync(request);

                    ValidateResponse(response, $"fetch project {projectIdOrKey}");

                    var project = JsonConvert.DeserializeObject<JiraProject>(response.Content);
                    _logger.LogInformation("Successfully fetched project: {ProjectName}", project?.Name);

                    return project;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error importing project {ProjectKey}", projectIdOrKey);
                    throw new JiraImportException($"Failed to import project {projectIdOrKey}", ex);
                }
            });
        }

        public async Task<List<JiraBoard>> GetBoardsByProjectIdAsync(
            string baseUrl,
            string token,
            string projectId)
        {
            return await _retryPolicy.ExecuteAsync(async () =>
            {
                try
                {
                    var allBoards = new List<JiraBoard>();
                    var startAt = 0;
                    var maxResults = MaxResultsPerPage;
                    var isLast = false;

                    _logger.LogInformation("Fetching boards for project: {ProjectId}", projectId);

                    while (!isLast)
                    {
                        var client = JiraServiceFactory.CreateClient(baseUrl, token);
                        var request = new RestRequest("/rest/agile/1.0/board", Method.Get);
                        request.AddHeader("Accept", "application/json");

                        if (!string.IsNullOrEmpty(projectId))
                        {
                            request.AddParameter("projectKeyOrId", projectId);
                        }

                        request.AddParameter("startAt", startAt);
                        request.AddParameter("maxResults", maxResults);

                        var response = await client.ExecuteAsync(request);
                        ValidateResponse(response, $"fetch boards for project {projectId}");

                        if (string.IsNullOrEmpty(response.Content))
                            break;

                        var json = JObject.Parse(response.Content);
                        var boards = json["values"]?.ToObject<List<JiraBoard>>() ?? new List<JiraBoard>();

                        allBoards.AddRange(boards);

                        isLast = json["isLast"]?.ToObject<bool>() ?? true;
                        startAt += maxResults;

                        _logger.LogDebug("Fetched {Count} boards (total: {Total})", boards.Count, allBoards.Count);
                    }

                    _logger.LogInformation("Successfully fetched {Count} boards for project {ProjectId}",
                        allBoards.Count, projectId);

                    return allBoards;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error fetching boards for project {ProjectId}", projectId);
                    throw new JiraImportException($"Failed to fetch boards for project {projectId}", ex);
                }
            });
        }

        public async Task<List<JiraSprint>> GetSprintsByBoardIdAsync(
            string baseUrl,
            string token,
            int boardId)
        {
            return await _retryPolicy.ExecuteAsync(async () =>
            {
                try
                {
                    var allSprints = new List<JiraSprint>();
                    var startAt = 0;
                    var maxResults = MaxResultsPerPage;
                    var isLast = false;

                    _logger.LogInformation("Fetching sprints for board: {BoardId}", boardId);

                    while (!isLast)
                    {
                        var client = JiraServiceFactory.CreateClient(baseUrl, token);
                        var request = new RestRequest($"/rest/agile/1.0/board/{boardId}/sprint", Method.Get);
                        request.AddHeader("Accept", "application/json");
                        request.AddParameter("startAt", startAt);
                        request.AddParameter("maxResults", maxResults);

                        var response = await client.ExecuteAsync(request);
                        ValidateResponse(response, $"fetch sprints for board {boardId}");

                        if (string.IsNullOrEmpty(response.Content))
                            break;

                        var json = JObject.Parse(response.Content);
                        var sprints = json["values"]?.ToObject<List<JiraSprint>>() ?? new List<JiraSprint>();

                        allSprints.AddRange(sprints);

                        isLast = json["isLast"]?.ToObject<bool>() ?? true;
                        startAt += maxResults;

                        _logger.LogDebug("Fetched {Count} sprints (total: {Total})", sprints.Count, allSprints.Count);
                    }

                    _logger.LogInformation("Successfully fetched {Count} sprints for board {BoardId}",
                        allSprints.Count, boardId);

                    return allSprints;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error fetching sprints for board {BoardId}", boardId);
                    throw new JiraImportException($"Failed to fetch sprints for board {boardId}", ex);
                }
            });
        }

        public async Task<List<JiraIssue>> GetIssuesByBoardIdAsync(
            string baseUrl,
            string token,
            string boardId)
        {
            return await _retryPolicy.ExecuteAsync(async () =>
            {
                try
                {
                    var allIssues = new List<JiraIssue>();
                    var startAt = 0;
                    var maxResults = MaxResultsPerPage;
                    var total = 0;

                    _logger.LogInformation("Fetching issues for board: {BoardId}", boardId);

                    do
                    {
                        var client = JiraServiceFactory.CreateClient(baseUrl, token);
                        var request = new RestRequest($"/rest/agile/1.0/board/{boardId}/issue", Method.Get);
                        request.AddHeader("Accept", "application/json");
                        request.AddParameter("startAt", startAt);
                        request.AddParameter("maxResults", maxResults);
                        request.AddParameter("fields", "summary,assignee,labels,reporter,creator,comment,customfield_10001,updated,status,priority,issuetype,description,created,duedate,parent,resolutiondate,timetracking,components,attachment,worklog,resolution,sprint,customfield_10014,customfield_10016");
                        request.AddParameter("expand", "names,schema,operations,editmeta,changelog,renderedFields");

                        var response = await client.ExecuteAsync(request);
                        ValidateResponse(response, $"fetch issues for board {boardId}");

                        if (string.IsNullOrEmpty(response.Content))
                            break;

                        var json = JObject.Parse(response.Content);
                        var issuesArray = json["issues"];
                        total = json["total"]?.ToObject<int>() ?? 0;

                        if (issuesArray != null)
                        {
                            foreach (var item in issuesArray)
                            {
                                var issue = ParseJiraIssue(item);
                                if (issue != null)
                                {
                                    allIssues.Add(issue);
                                }
                            }
                        }

                        startAt += maxResults;
                        _logger.LogDebug("Fetched {Count} issues (total: {Total}/{Expected})",
                            allIssues.Count, allIssues.Count, total);

                    } while (startAt < total);

                    _logger.LogInformation("Successfully fetched {Count} issues for board {BoardId}",
                        allIssues.Count, boardId);

                    return allIssues;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error fetching issues for board {BoardId}", boardId);
                    throw new JiraImportException($"Failed to fetch issues for board {boardId}", ex);
                }
            });
        }

        public async Task<List<JiraEpic>> GetEpicsByBoardIdAsync(
            string baseUrl,
            string token,
            string boardId)
        {
            return await _retryPolicy.ExecuteAsync(async () =>
            {
                try
                {
                    var allEpics = new List<JiraEpic>();
                    var startAt = 0;
                    var maxResults = MaxResultsPerPage;
                    var isLast = false;

                    _logger.LogInformation("Fetching epics for board: {BoardId}", boardId);

                    while (!isLast)
                    {
                        var client = JiraServiceFactory.CreateClient(baseUrl, token);
                        var request = new RestRequest($"/rest/agile/1.0/board/{boardId}/epic", Method.Get);
                        request.AddHeader("Accept", "application/json");
                        request.AddParameter("startAt", startAt);
                        request.AddParameter("maxResults", maxResults);

                        var response = await client.ExecuteAsync(request);
                        ValidateResponse(response, $"fetch epics for board {boardId}");

                        if (string.IsNullOrEmpty(response.Content))
                            break;

                        var json = JObject.Parse(response.Content);
                        var epics = json["values"]?.ToObject<List<JiraEpic>>() ?? new List<JiraEpic>();

                        allEpics.AddRange(epics);

                        isLast = json["isLast"]?.ToObject<bool>() ?? true;
                        startAt += maxResults;

                        _logger.LogDebug("Fetched {Count} epics (total: {Total})", epics.Count, allEpics.Count);
                    }

                    _logger.LogInformation("Successfully fetched {Count} epics for board {BoardId}",
                        allEpics.Count, boardId);

                    return allEpics;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error fetching epics for board {BoardId}", boardId);
                    throw new JiraImportException($"Failed to fetch epics for board {boardId}", ex);
                }
            });
        }

        public async Task<List<JiraUser>> GetUsersByProjectAsync(
            string baseUrl,
            string token,
            string projectIdOrKey)
        {
            return await _retryPolicy.ExecuteAsync(async () =>
            {
                try
                {
                    var allUsers = new List<JiraUser>();
                    var startAt = 0;
                    var maxResults = MaxResultsPerPage;
                    var total = 0;

                    _logger.LogInformation("Fetching users for project: {ProjectKey}", projectIdOrKey);

                    do
                    {
                        var client = JiraServiceFactory.CreateClient(baseUrl, token);
                        var request = new RestRequest($"/rest/api/3/user/assignable/search", Method.Get);
                        request.AddHeader("Accept", "application/json");
                        request.AddParameter("project", projectIdOrKey);
                        request.AddParameter("startAt", startAt);
                        request.AddParameter("maxResults", maxResults);

                        var response = await client.ExecuteAsync(request);
                        ValidateResponse(response, $"fetch users for project {projectIdOrKey}");

                        if (string.IsNullOrEmpty(response.Content))
                            break;

                        var users = JsonConvert.DeserializeObject<List<JiraUser>>(response.Content);
                        if (users == null || !users.Any())
                            break;

                        allUsers.AddRange(users);
                        total = users.Count;
                        startAt += maxResults;

                        _logger.LogDebug("Fetched {Count} users (total: {Total})", users.Count, allUsers.Count);

                        // Break if we got fewer results than requested
                        if (users.Count < maxResults)
                            break;

                    } while (total >= maxResults);

                    _logger.LogInformation("Successfully fetched {Count} users for project {ProjectKey}",
                        allUsers.Count, projectIdOrKey);

                    return allUsers;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error fetching users for project {ProjectKey}", projectIdOrKey);
                    throw new JiraImportException($"Failed to fetch users for project {projectIdOrKey}", ex);
                }
            });
        }

        public async Task<Dictionary<string, List<JiraUser>>> GetRoleMembersByProjectAsync(
            string baseUrl,
            string token,
            string projectIdOrKey)
        {
            return await _retryPolicy.ExecuteAsync(async () =>
            {
                try
                {
                    _logger.LogInformation("Fetching role members for project: {ProjectKey}", projectIdOrKey);

                    // First, get project to fetch roles
                    var project = await ImportProjectByIdAsync(baseUrl, token, projectIdOrKey);
                    if (project?.Roles == null || !project.Roles.Any())
                    {
                        _logger.LogWarning("No roles found for project {ProjectKey}", projectIdOrKey);
                        return new Dictionary<string, List<JiraUser>>();
                    }

                    var roleMembers = new Dictionary<string, List<JiraUser>>();

                    foreach (var role in project.Roles)
                    {
                        try
                        {
                            var client = JiraServiceFactory.CreateClient(baseUrl, token);
                            var request = new RestRequest(role.Value, Method.Get);
                            request.AddHeader("Accept", "application/json");

                            var response = await client.ExecuteAsync(request);

                            if (!response.IsSuccessful || string.IsNullOrEmpty(response.Content))
                                continue;

                            var json = JObject.Parse(response.Content);
                            var actors = json["actors"];

                            var users = new List<JiraUser>();
                            if (actors != null)
                            {
                                foreach (var actor in actors)
                                {
                                    var actorUser = actor["actorUser"];
                                    if (actorUser != null)
                                    {
                                        var user = actorUser.ToObject<JiraUser>();
                                        if (user != null)
                                        {
                                            users.Add(user);
                                        }
                                    }
                                }
                            }

                            if (users.Any())
                            {
                                roleMembers[role.Key] = users;
                                _logger.LogDebug("Found {Count} users for role {RoleName}", users.Count, role.Key);
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning(ex, "Error fetching members for role {RoleName}", role.Key);
                        }
                    }

                    _logger.LogInformation("Successfully fetched role members for {Count} roles in project {ProjectKey}",
                        roleMembers.Count, projectIdOrKey);

                    return roleMembers;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error fetching role members for project {ProjectKey}", projectIdOrKey);
                    throw new JiraImportException($"Failed to fetch role members for project {projectIdOrKey}", ex);
                }
            });
        }

        public async Task<JiraProjectData> GetFullProjectDataAsync(
            string baseUrl,
            string token,
            string projectIdOrKey)
        {
            try
            {
                _logger.LogInformation("Starting full project import for: {ProjectKey}", projectIdOrKey);

                // Fetch project details
                var project = await ImportProjectByIdAsync(baseUrl, token, projectIdOrKey);

                if (project == null)
                {
                    throw new JiraImportException($"Project {projectIdOrKey} not found");
                }

                // Fetch project members and role members
                var projectMembersTask = GetUsersByProjectAsync(baseUrl, token, projectIdOrKey);
                var roleMembersTask = GetRoleMembersByProjectAsync(baseUrl, token, projectIdOrKey);

                await Task.WhenAll(projectMembersTask, roleMembersTask);

                var projectMembers = await projectMembersTask;
                var roleMembers = await roleMembersTask;

                // Fetch all boards
                var boards = await GetBoardsByProjectIdAsync(baseUrl, token, projectIdOrKey);

                if (!boards.Any())
                {
                    _logger.LogWarning("No boards found for project {ProjectKey}", projectIdOrKey);
                }

                var boardDetailsList = new List<BoardWithDetails>();

                // Fetch details for each board with parallel processing
                var boardTasks = boards.Select(async board =>
                {
                    try
                    {
                        _logger.LogInformation("Processing board: {BoardName} (ID: {BoardId})",
                            board.Name, board.Id);

                        var sprintsTask = GetSprintsByBoardIdAsync(baseUrl, token, board.Id);
                        var issuesTask = GetIssuesByBoardIdAsync(baseUrl, token, board.Id.ToString());
                        var epicsTask = GetEpicsByBoardIdAsync(baseUrl, token, board.Id.ToString());

                        await Task.WhenAll(sprintsTask, issuesTask, epicsTask);

                        return new BoardWithDetails
                        {
                            BoardInfo = board,
                            Sprints = await sprintsTask,
                            Issues = await issuesTask,
                            Epics = await epicsTask
                        };
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error processing board {BoardId}", board.Id);
                        // Return partial data instead of failing completely
                        return new BoardWithDetails
                        {
                            BoardInfo = board,
                            Sprints = new List<JiraSprint>(),
                            Issues = new List<JiraIssue>(),
                            Epics = new List<JiraEpic>()
                        };
                    }
                });

                // Wait for all board processing to complete
                var boardDetails = await Task.WhenAll(boardTasks);
                boardDetailsList.AddRange(boardDetails);

                _logger.LogInformation(
                    "Successfully imported project {ProjectKey}: {BoardCount} boards, " +
                    "{IssueCount} issues, {SprintCount} sprints, {EpicCount} epics, {UserCount} users",
                    projectIdOrKey,
                    boardDetailsList.Count,
                    boardDetailsList.Sum(b => b.TotalIssues),
                    boardDetailsList.Sum(b => b.TotalSprints),
                    boardDetailsList.Sum(b => b.TotalEpics),
                    projectMembers.Count);

                return new JiraProjectData
                {
                    Project = project,
                    Boards = boardDetailsList,
                    ProjectMembers = projectMembers,
                    RoleMembers = roleMembers,
                    ImportedAt = DateTime.UtcNow,
                    SourceBaseUrl = baseUrl
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error importing full project data for {ProjectKey}", projectIdOrKey);
                throw new JiraImportException($"Failed to import full project data for {projectIdOrKey}", ex);
            }
        }

        // ========================================
        // HELPER METHODS
        // ========================================

        private JiraIssue ParseJiraIssue(JToken item)
        {
            try
            {
                var fields = item["fields"];

                // Extract sprint information from custom field
                var sprintField = fields?["customfield_10020"] ?? fields?["customfield_10014"];
                string sprintName = null;

                if (sprintField != null && sprintField.Type == JTokenType.Array && sprintField.Any())
                {
                    var sprintData = sprintField.Last;
                    if (sprintData != null)
                    {
                        sprintName = sprintData["name"]?.ToString();
                    }
                }

                // Extract epic key and name
                var epicKey = fields?["customfield_10014"]?.ToString() ??
                             fields?["customfield_10008"]?.ToString() ??
                             fields?["parent"]?["key"]?.ToString();

                var epicName = fields?["customfield_10011"]?.ToString();

                // Extract parent key for subtasks
                var parentKey = fields?["parent"]?["key"]?.ToString();

                var issue = new JiraIssue
                {
                    Id = item["id"]?.ToString(),
                    Key = item["key"]?.ToString(),
                    Self = item["self"]?.ToString(),
                    Summary = fields?["summary"]?.ToString(),
                    Description = fields?["description"]?.ToString(),
                    IssueType = fields?["issuetype"]?["name"]?.ToString(),
                    Assignee = SafeDeserialize<JiraUser>(fields?["assignee"]),
                    Labels = fields?["labels"]?.ToObject<List<string>>() ?? new List<string>(),
                    Reporter = SafeDeserialize<JiraUser>(fields?["reporter"]),
                    Creator = SafeDeserialize<JiraUser>(fields?["creator"]),
                    Comment = fields?["comment"]?["comments"]?.ToObject<List<JiraComment>>() ?? new List<JiraComment>(),
                    Team = SafeDeserialize<JiraTeam>(fields?["customfield_10001"]),
                    Status = SafeDeserialize<JiraStatus>(fields?["status"]),
                    Priority = SafeDeserialize<JiraPriority>(fields?["priority"]),
                    Resolution = SafeDeserialize<JiraResolution>(fields?["resolution"]),
                    Parent = SafeDeserialize<JiraIssueParent>(fields?["parent"]),
                    Components = fields?["components"]?.ToObject<List<JiraComponent>>() ?? new List<JiraComponent>(),
                    Attachments = fields?["attachment"]?.ToObject<List<JiraAttachment>>() ?? new List<JiraAttachment>(),
                    Worklogs = fields?["worklog"]?["worklogs"]?.ToObject<List<JiraWorklog>>() ?? new List<JiraWorklog>(),
                    CreatedAt = ParseDateTime(fields?["created"]),
                    UpdatedAt = ParseDateTime(fields?["updated"]),
                    DueDate = ParseDateTime(fields?["duedate"]),
                    ResolutionDate = ParseDateTime(fields?["resolutiondate"]),
                    SprintName = sprintName,
                    EpicKey = epicKey,
                    EpicName = epicName,
                    ParentKey = parentKey
                };

                // Extract story points (can be in different custom fields)
                var storyPoints = fields?["customfield_10016"] ??
                                 fields?["customfield_10026"] ??
                                 fields?["customfield_10002"];
                if (storyPoints != null && decimal.TryParse(storyPoints.ToString(), out var points))
                {
                    issue.StoryPoints = points;
                }

                // Extract time tracking
                var timeTracking = fields?["timetracking"];
                if (timeTracking != null)
                {
                    issue.TimeEstimate = timeTracking["originalEstimateSeconds"]?.ToObject<int>() ?? 0;
                    issue.TimeSpent = timeTracking["timeSpentSeconds"]?.ToObject<int>() ?? 0;
                    issue.TimeRemaining = timeTracking["remainingEstimateSeconds"]?.ToObject<int>() ?? 0;
                }

                return issue;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error parsing issue {IssueKey}", item["key"]?.ToString());
                return null;
            }
        }

        private T SafeDeserialize<T>(JToken token) where T : class
        {
            try
            {
                return token?.ToObject<T>();
            }
            catch
            {
                return null;
            }
        }

        private DateTime? ParseDateTime(JToken token)
        {
            try
            {
                return token != null ? DateTime.Parse(token.ToString()) : (DateTime?)null;
            }
            catch
            {
                return null;
            }
        }

        private void ValidateResponse(RestResponse response, string operation)
        {
            if (!response.IsSuccessful)
            {
                var errorMessage = $"Failed to {operation}. Status: {response.StatusCode}";

                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    throw new JiraAuthenticationException("Invalid Jira credentials or token expired");
                }
                else if (response.StatusCode == HttpStatusCode.Forbidden)
                {
                    throw new JiraAuthorizationException("Insufficient permissions to access this resource");
                }
                else if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    throw new JiraNotFoundException($"Resource not found while trying to {operation}");
                }
                else if (response.StatusCode == HttpStatusCode.TooManyRequests)
                {
                    throw new JiraRateLimitException("Jira API rate limit exceeded. Please try again later.");
                }

                _logger.LogError("{ErrorMessage}. Content: {Content}", errorMessage, response.Content);
                throw new JiraImportException(errorMessage);
            }

            if (string.IsNullOrEmpty(response.Content))
            {
                _logger.LogWarning("Empty response received for operation: {Operation}", operation);
            }
        }
    }

    // ========================================
    // CUSTOM EXCEPTIONS
    // ========================================
    public class JiraImportException : Exception
    {
        public JiraImportException(string message) : base(message) { }
        public JiraImportException(string message, Exception innerException) : base(message, innerException) { }
    }

    public class JiraAuthenticationException : JiraImportException
    {
        public JiraAuthenticationException(string message) : base(message) { }
    }

    public class JiraAuthorizationException : JiraImportException
    {
        public JiraAuthorizationException(string message) : base(message) { }
    }

    public class JiraNotFoundException : JiraImportException
    {
        public JiraNotFoundException(string message) : base(message) { }
    }

    public class JiraRateLimitException : JiraImportException
    {
        public JiraRateLimitException(string message) : base(message) { }
    }
}