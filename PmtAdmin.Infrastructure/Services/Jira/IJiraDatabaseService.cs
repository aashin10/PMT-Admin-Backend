using static PmtAdmin.Infrastructure.Models.JiraImportModels;

namespace PmtAdmin.Infrastructure.Services.Jira
{
    public interface IJiraDatabaseService
    {
        public Task PopulateDataBase(List<JiraProjectData> projects);
    }
}
