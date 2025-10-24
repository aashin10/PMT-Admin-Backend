using static PmtAdmin.Infrastructure.Models.JiraImportModels;

namespace PmtAdmin.Infrastructure.Services.Jira
{
    public interface IJiraService
    {
        Task<JiraProject> ImportProjectByIdAsync(string baseUrl, string token, string projectIdOrKey);
        Task<List<JiraBoard>> GetBoardsByProjectIdAsync(string baseUrl, string token, string projectId);
        Task<List<JiraSprint>> GetSprintsByBoardIdAsync(string baseUrl, string token, int boardId);
        Task<List<JiraIssue>> GetIssuesByBoardIdAsync(string baseUrl, string token, string boardId);
        Task<List<JiraEpic>> GetEpicsByBoardIdAsync(string baseUrl, string token, string boardId);
        Task<List<JiraUser>> GetUsersByProjectAsync(string baseUrl, string token, string projectIdOrKey);
        Task<Dictionary<string, List<JiraUser>>> GetRoleMembersByProjectAsync(string baseUrl, string token, string projectIdOrKey);
        Task<JiraProjectData> GetFullProjectDataAsync(string baseUrl, string token, string projectIdOrKey);
    }
}
