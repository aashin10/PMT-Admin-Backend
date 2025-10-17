using static PmtAdmin.Infrastructure.Models.JiraImportModels;

namespace PmtAdmin.Infrastructure.Services
{
    public interface IJiraService
    {
        public Task<JiraProject> ImportProjectByIdAsync(string baseUrl, string token, string projectIdOrKey);

        public Task<List<JiraBoard>> GetBoardsByProjectIdAsync(string baseUrl, string token, string projectId);

        public Task<List<JiraSprint>> GetSprintsByBoardIdAsync(string baseUrl, string token, int boardId);
    }
}
