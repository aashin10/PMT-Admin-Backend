using PmtAdmin.Domain.Entities;
using static PmtAdmin.Infrastructure.Models.JiraImportModels;

namespace PmtAdmin.Infrastructure.Services.Jira
{
    /// <summary>
    /// Service interface for importing Jira data into the database
    /// </summary>
    public interface IJiraDatabaseService
    {
        /// <summary>
        /// Imports multiple Jira projects with all related data into the database
        /// </summary>
        /// <param name="projects">List of Jira project data to import</param>
        /// <param name="importedByUserId">ID of the user performing the import</param>
        /// <returns>Import result with statistics and errors</returns>
        Task<ImportResult> PopulateDataBase(
            List<JiraProjectData> projects,
            int? importedByUserId = null);

        /// <summary>
        /// Retrieves all projects that were imported from Jira
        /// </summary>
        /// <returns>List of projects with related entities</returns>
        Task<List<Project>> GetImportedProjects();

        /// <summary>
        /// Gets the status and details of a specific import job
        /// </summary>
        /// <param name="jobId">Import job ID</param>
        /// <returns>Import job entity with details</returns>
        Task<ImportJobs> GetImportJobStatus(int jobId);
    }

    /// <summary>
    /// Result model for tracking import operations
    /// </summary>

}
