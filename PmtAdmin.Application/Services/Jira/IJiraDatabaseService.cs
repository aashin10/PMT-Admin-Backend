using PmtAdmin.Domain.Entities;
using static PmtAdmin.Domain.Models.JiraImportModels;

namespace PmtAdmin.Infrastructure.Services.Jira
{
    public interface IJiraDatabaseService
    {
        public Task<List<User>> PopulateDataBase(List<JiraProjectData> projects);
    }
}
