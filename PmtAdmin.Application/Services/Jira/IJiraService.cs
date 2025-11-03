using static PmtAdmin.Domain.Models.JiraImportModels;


namespace PmtAdmin.Infrastructure.Services.Jira
{
    public interface IJiraService
    {
        public Task<JiraProject> ImportProjectByIdAsync(string baseUrl, string token, string projectIdOrKey);

        public Task<List<JiraBoard>> GetBoardsByProjectIdAsync(string baseUrl, string token, string projectId);

        public Task<List<JiraSprint>> GetSprintsByBoardIdAsync(string baseUrl, string token, int boardId);

        public Task<List<JiraIssue>> GetIssuesByBoardIdAsync(string baseUrl, string token, string boardId);

        public Task<List<JiraEpic>> GetEpicsByBoardIdAsync(string baseUrl, string token, string boardId);

        public Task<List<JiraUser>> GetUsersByRoleAsync(string roleUrl, string token);

        public Task<JiraProjectData> GetFullProjectDataAsync(string baseUrl, string token, string projectIdOrKey);
    }
}
