using PmtAdmin.Application.Dto;
using static PmtAdmin.Domain.Models.JiraImportModels;

namespace PmtAdmin.Infrastructure.Services.Jira
{
    public interface IJiraDatabaseService
    {
        public Task<JiraImportDatabaseResult> PopulateDataBase(List<JiraProjectData> projects, int importedby);
    }
}
