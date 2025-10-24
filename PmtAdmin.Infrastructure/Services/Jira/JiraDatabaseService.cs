using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PmtAdmin.Domain.Entities;
using PmtAdmin.Infrastructure.Context;
using System.Text.Json;
using static PmtAdmin.Infrastructure.Models.JiraImportModels;

namespace PmtAdmin.Infrastructure.Services.Jira
{
    public class JiraDatabaseService : IJiraDatabaseService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<JiraDatabaseService> _logger;

        public JiraDatabaseService(
            AppDbContext context,
            IMapper mapper,
            ILogger<JiraDatabaseService> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ImportResult> PopulateDataBase(
            List<JiraProjectData> projects,
            int? importedByUserId = null)
        {
            var result = new ImportResult();
            var importJobId = await CreateImportJob("jira", "jira_cloud", importedByUserId);

            try
            {
                _logger.LogInformation("Starting import of {Count} Jira projects", projects.Count);

                foreach (var projectData in projects)
                {
                    await ImportSingleProject(projectData, result, importedByUserId);
                }

                await UpdateImportJobSuccess(importJobId, result);
                _logger.LogInformation(
                    "Successfully completed import: {Projects} projects, {Issues} issues, {Comments} comments",
                    result.ImportedProjects, result.ImportedIssues, result.ImportedComments);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Critical error during Jira import");
                await UpdateImportJobFailure(importJobId, ex.Message);
                throw;
            }
        }

        private async Task ImportSingleProject(
            JiraProjectData projectData,
            ImportResult result,
            int? importedByUserId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                if (projectData?.Project == null)
                {
                    _logger.LogWarning("Skipping null project data");
                    result.FailedProjects++;
                    return;
                }

                _logger.LogInformation("Importing project: {ProjectName} ({ProjectKey})",
                    projectData.Project.Name, projectData.Project.Key);

                // Check if project already exists
                var existingProject = await _context.Projects
                    .FirstOrDefaultAsync(p => p.Key == projectData.Project.Key);

                Project project;

                if (existingProject != null)
                {
                    _logger.LogWarning("Project {Key} already exists. Updating...", projectData.Project.Key);
                    project = existingProject;
                    UpdateProjectFromJira(project, projectData.Project, importedByUserId);
                    result.UpdatedProjects++;
                }
                else
                {
                    project = CreateProjectFromJira(projectData.Project, importedByUserId);
                    _context.Projects.Add(project);
                    result.ImportedProjects++;
                }

                await _context.SaveChangesAsync();

                // Import users first (needed for relationships)
                var userMapping = await ImportProjectUsers(projectData, project, importedByUserId);

                // Import boards with related entities
                if (projectData.Boards != null && projectData.Boards.Any())
                {
                    await ImportBoards(projectData.Boards, project, userMapping, result, importedByUserId);
                }

                await transaction.CommitAsync();
                _logger.LogInformation("Successfully imported project: {ProjectName}", project.Name);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error importing project: {ProjectName}",
                    projectData?.Project?.Name);
                result.FailedProjects++;
                result.Errors.Add($"Project '{projectData?.Project?.Name}': {ex.Message}");
                throw;
            }
        }

        private Project CreateProjectFromJira(JiraProject jiraProject, int? userId)
        {
            return new Project
            {
                Key = jiraProject.Key,
                Name = jiraProject.Name,
                Description = jiraProject.Description,
                IsImportedFromJira = true,
                CreatedById = userId,
                CreatedAt = DateTime.UtcNow
            };
        }

        private void UpdateProjectFromJira(Project project, JiraProject jiraProject, int? userId)
        {
            project.Name = jiraProject.Name;
            project.Description = jiraProject.Description;
            project.UpdatedById = userId;
            project.UpdatedAt = DateTime.UtcNow;
        }

        private async Task<Dictionary<string, int>> ImportProjectUsers(
            JiraProjectData projectData,
            Project project,
            int? importedByUserId)
        {
            var userMapping = new Dictionary<string, int>(); // Jira AccountId -> DB UserId

            try
            {
                // Collect all unique users from various sources
                var allUsers = new Dictionary<string, JiraUser>();

                // From project lead
                if (projectData.Project?.Lead != null)
                {
                    AddUserToDictionary(allUsers, projectData.Project.Lead);
                }

                // From project members
                if (projectData.ProjectMembers != null)
                {
                    foreach (var user in projectData.ProjectMembers)
                    {
                        AddUserToDictionary(allUsers, user);
                    }
                }

                // From role members
                if (projectData.RoleMembers != null)
                {
                    foreach (var roleUsers in projectData.RoleMembers.Values)
                    {
                        foreach (var user in roleUsers)
                        {
                            AddUserToDictionary(allUsers, user);
                        }
                    }
                }

                // From boards (assignees, reporters, etc.)
                if (projectData.Boards != null)
                {
                    foreach (var board in projectData.Boards)
                    {
                        if (board.Issues != null)
                        {
                            foreach (var issue in board.Issues)
                            {
                                AddUserToDictionary(allUsers, issue.Assignee);
                                AddUserToDictionary(allUsers, issue.Reporter);
                                AddUserToDictionary(allUsers, issue.Creator);

                                if (issue.Comment != null)
                                {
                                    foreach (var comment in issue.Comment)
                                    {
                                        AddUserToDictionary(allUsers, comment.Author);
                                        AddUserToDictionary(allUsers, comment.UpdateAuthor);
                                    }
                                }
                            }
                        }
                    }
                }

                // Import or update users
                foreach (var jiraUser in allUsers.Values)
                {
                    var dbUserId = await ImportUser(jiraUser, importedByUserId);
                    if (dbUserId.HasValue)
                    {
                        userMapping[jiraUser.AccountId] = dbUserId.Value;
                    }
                }

                _logger.LogInformation("Imported {Count} users for project {ProjectKey}",
                    userMapping.Count, project.Key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error importing users for project {ProjectKey}", project.Key);
            }

            return userMapping;
        }

        private void AddUserToDictionary(Dictionary<string, JiraUser> dict, JiraUser user)
        {
            if (user != null && !string.IsNullOrEmpty(user.AccountId) && !dict.ContainsKey(user.AccountId))
            {
                dict[user.AccountId] = user;
            }
        }

        private async Task<int?> ImportUser(JiraUser jiraUser, int? importedByUserId)
        {
            try
            {
                if (string.IsNullOrEmpty(jiraUser.AccountId))
                    return null;

                // Check if user exists
                var existingUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.Jira_Id == jiraUser.AccountId);

                if (existingUser != null)
                {
                    // Update user information
                    existingUser.Name = jiraUser.DisplayName;
                    existingUser.Email = jiraUser.EmailAddress ?? existingUser.Email;
                    //existingUser.IsActive = jiraUser.Active ?? true;
                    //existingUser.UpdatedAt = DateTime.UtcNow;
                    //existingUser.UpdatedById = importedByUserId;

                    return existingUser.Id;
                }

                // Create new user
                var newUser = new User
                {
                    Jira_Id = jiraUser.AccountId,
                    Name = jiraUser.DisplayName,
                    Email = jiraUser.EmailAddress ?? $"{jiraUser.AccountId}@jira.imported",
                    //IsActive = jiraUser.Active ?? true,
                    //AvatarUrl = jiraUser.AvatarUrls?.Size48,
                    //CreatedById = importedByUserId,
                    //CreatedAt = DateTime.UtcNow
                };

                _context.Users.Add(newUser);
                await _context.SaveChangesAsync();

                return newUser.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error importing user {DisplayName}", jiraUser?.DisplayName);
                return null;
            }
        }

        private async Task ImportBoards(
            List<BoardWithDetails> boardsData,
            Project project,
            Dictionary<string, int> userMapping,
            ImportResult result,
            int? importedByUserId)
        {
            foreach (var boardData in boardsData)
            {
                try
                {
                    if (boardData?.BoardInfo == null)
                        continue;

                    _logger.LogInformation("Importing board: {BoardName}", boardData.BoardInfo.Name);

                    var board = new Board
                    {
                        ProjectId = Convert.ToInt32(project.Id), // Note: May need adjustment based on your schema
                        Name = boardData.BoardInfo.Name,
                        Type = boardData.BoardInfo.Type ?? "kanban",
                        //Metadata = JsonSerializer.Serialize(new
                        //{
                        //    JiraId = boardData.BoardInfo.Id,
                        //    boardData.BoardInfo.Location
                        //}),
                        CreatedById = importedByUserId,
                        CreatedAt = DateTime.UtcNow,
                        IsActive = true
                    };

                    _context.Boards.Add(board);
                    await _context.SaveChangesAsync();

                    // Import Sprints
                    if (boardData.Sprints != null)
                    {
                        await ImportSprints(boardData.Sprints, project, result, importedByUserId);
                    }

                    // Import Epics
                    if (boardData.Epics != null)
                    {
                        await ImportEpics(boardData.Epics, project, result, importedByUserId);
                    }

                    // Import Issues
                    if (boardData.Issues != null)
                    {
                        await ImportIssues(boardData.Issues, project, userMapping, result, importedByUserId);
                    }

                    result.ImportedBoards++;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error importing board: {BoardName}",
                        boardData?.BoardInfo?.Name);
                    result.FailedBoards++;
                    result.Errors.Add($"Board '{boardData?.BoardInfo?.Name}': {ex.Message}");
                }
            }
        }

        private async Task ImportSprints(
            List<JiraSprint> sprintsData,
            Project project,
            ImportResult result,
            int? importedByUserId)
        {
            foreach (var jiraSprint in sprintsData)
            {
                try
                {
                    if (string.IsNullOrEmpty(jiraSprint.Name))
                        continue;

                    var sprint = new Sprint
                    {
                        //Id = Guid.NewGuid(),

                        //= Convert.ToInt32(project.Id),
                        Name = jiraSprint.Name,
                        SprintGoal = jiraSprint.Goal,
                        StartDate = jiraSprint.StartDate,
                        DueDate = jiraSprint.EndDate,
                        //Status = MapSprintState(jiraSprint.State),
                        //CreatedById = importedByUserId,
                        //CreatedAt = DateTime.UtcNow,
                        //UpdatedAt = DateTime.UtcNow
                    };

                    _context.Sprints.Add(sprint);
                    result.ImportedSprints++;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error importing sprint: {SprintName}", jiraSprint?.Name);
                    result.FailedSprints++;
                }
            }

            await _context.SaveChangesAsync();
        }

        private async Task ImportEpics(
            List<JiraEpic> epicsData,
            Project project,
            ImportResult result,
            int? importedByUserId)
        {
            foreach (var jiraEpic in epicsData)
            {
                try
                {
                    if (string.IsNullOrEmpty(jiraEpic.Name))
                        continue;

                    var epic = new Epic
                    {
                        //Id = Guid.NewGuid(),
                        //ProjectId = Convert.ToInt32(project.Id),
                        //Title = jiraEpic.Name ?? jiraEpic.Summary,
                        //Description = jiraEpic.Description,
                        //StartDate = jiraEpic.StartDate,
                        //DueDate = jiraEpic.DueDate,
                        //CreatedById = importedByUserId,
                        //CreatedAt = DateTime.UtcNow,
                        //UpdatedAt = DateTime.UtcNow,
                        //Labels = JsonSerializer.Serialize(new { jiraEpic.Key, jiraEpic.Color })
                    };

                    _context.Epics.Add(epic);
                    result.ImportedEpics++;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error importing epic: {EpicName}", jiraEpic?.Name);
                    result.FailedEpics++;
                }
            }

            await _context.SaveChangesAsync();
        }

        private async Task ImportIssues(
            List<JiraIssue> issuesData,
            Project project,
            Dictionary<string, int> userMapping,
            ImportResult result,
            int? importedByUserId)
        {
            // Create mapping for first pass (Jira Key -> GUID)
            var issueMapping = new Dictionary<string, Guid>();

            // First pass: Create all issues
            foreach (var jiraIssue in issuesData)
            {
                try
                {
                    if (string.IsNullOrEmpty(jiraIssue.Key))
                        continue;

                    var issue = new Issue
                    {
                        Id = Guid.NewGuid(),
                        Key = jiraIssue.Key,
                        ProjectId = Convert.ToInt32(project.Id),
                        Summary = jiraIssue.Summary,
                        Title = jiraIssue.Summary,
                        Description = jiraIssue.Description,
                        Type = MapIssueType(jiraIssue.IssueType),
                        Priority = MapPriority(jiraIssue.Priority?.Name),
                        Status = jiraIssue.Status?.Name ?? "TODO",
                        AssigneeId = GetUserId(jiraIssue.Assignee?.AccountId, userMapping),
                        ReporterId = GetUserId(jiraIssue.Reporter?.AccountId, userMapping),
                        StoryPoints = Convert.ToInt32(jiraIssue.StoryPoints ?? 0),
                        Labels = JsonSerializer.Serialize(jiraIssue.Labels ?? new List<string>()),
                        StartDate = jiraIssue.CreatedAt,
                        DueDate = jiraIssue.DueDate,
                        CreatedById = importedByUserId,
                        CreatedAt = jiraIssue.CreatedAt ?? DateTime.UtcNow,
                        UpdatedAt = jiraIssue.UpdatedAt ?? DateTime.UtcNow
                    };

                    // Map to sprint
                    if (!string.IsNullOrEmpty(jiraIssue.SprintName))
                    {
                        //var sprint = await _context.Sprints
                        //    .FirstOrDefaultAsync(s => s.ProjectId == Convert.ToInt32(project.Id)
                        //        && s.Name == jiraIssue.SprintName);
                        //if (sprint != null)
                        //{
                        //    issue.SprintId = Convert.ToInt32(sprint.Id);
                        //}
                    }

                    // Map to epic
                    if (!string.IsNullOrEmpty(jiraIssue.EpicKey))
                    {
                        //var epic = await _context.Epics
                        //    .FirstOrDefaultAsync(e => e.ProjectId == Convert.ToInt32(project.Id)
                        //        && e.Title.Contains(jiraIssue.EpicKey));
                        //if (epic != null)
                        //{
                        //    issue.EpicId = Convert.ToInt32(epic.Id);
                        //}
                    }

                    _context.Issues.Add(issue);
                    issueMapping[jiraIssue.Key] = issue.Id;

                    // Import comments
                    if (jiraIssue.Comment != null && jiraIssue.Comment.Any())
                    {
                        await ImportIssueComments(
                            jiraIssue.Comment,
                            issue,
                            userMapping,
                            result,
                            importedByUserId);
                    }

                    result.ImportedIssues++;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error importing issue: {IssueKey}", jiraIssue?.Key);
                    result.FailedIssues++;
                }
            }

            await _context.SaveChangesAsync();

            // Second pass: Update parent relationships
            await UpdateParentIssueReferences(issuesData, issueMapping);
        }

        private async Task UpdateParentIssueReferences(
            List<JiraIssue> issuesData,
            Dictionary<string, Guid> issueMapping)
        {
            foreach (var jiraIssue in issuesData)
            {
                try
                {
                    if (string.IsNullOrEmpty(jiraIssue.ParentKey) ||
                        string.IsNullOrEmpty(jiraIssue.Key) ||
                        !issueMapping.ContainsKey(jiraIssue.Key) ||
                        !issueMapping.ContainsKey(jiraIssue.ParentKey))
                        continue;

                    var issue = await _context.Issues.FindAsync(issueMapping[jiraIssue.Key]);
                    if (issue != null)
                    {
                        //issue.ParentIssueId = Convert.ToInt32(issueMapping[jiraIssue.ParentKey]);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating parent for issue: {IssueKey}", jiraIssue?.Key);
                }
            }

            await _context.SaveChangesAsync();
        }

        private async Task ImportIssueComments(
            List<JiraComment> commentsData,
            Issue issue,
            Dictionary<string, int> userMapping,
            ImportResult result,
            int? importedByUserId)
        {
            foreach (var jiraComment in commentsData)
            {
                try
                {
                    if (string.IsNullOrEmpty(jiraComment.Body))
                        continue;

                    var comment = new IssueComment
                    {
                        //Id = Guid.NewGuid(),
                        //IssueId = Convert.ToInt32(issue.Id),
                        //AuthorId = GetUserId(jiraComment.Author?.AccountId, userMapping) ?? 0,
                        //MentionId = 0, // TODO: Extract mentions from body
                        //Body = jiraComment.Body,
                        //CreatedById = importedByUserId,
                        //CreatedAt = jiraComment.CreatedAt ?? DateTime.UtcNow,
                        //UpdatedAt = jiraComment.UpdatedAt ?? DateTime.UtcNow
                    };

                    _context.IssueComments.Add(comment);
                    result.ImportedComments++;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error importing comment for issue: {IssueKey}", issue.Key);
                    result.FailedComments++;
                }
            }
        }

        // Helper methods
        private int? GetUserId(string jiraAccountId, Dictionary<string, int> userMapping)
        {
            if (string.IsNullOrEmpty(jiraAccountId))
                return null;

            return userMapping.TryGetValue(jiraAccountId, out var userId) ? userId : null;
        }

        private string MapSprintState(string jiraState)
        {
            return jiraState?.ToLower() switch
            {
                "future" => "PLANNED",
                "active" => "ACTIVE",
                "closed" => "COMPLETED",
                _ => "PLANNED"
            };
        }

        private string MapIssueType(string jiraType)
        {
            return jiraType?.ToLower() switch
            {
                "story" => "STORY",
                "bug" => "BUG",
                "task" => "TASK",
                "subtask" => "SUBTASK",
                "sub-task" => "SUBTASK",
                _ => "TASK"
            };
        }

        private string MapPriority(string jiraPriority)
        {
            return jiraPriority?.ToLower() switch
            {
                "highest" => "CRITICAL",
                "high" => "HIGH",
                "medium" => "MEDIUM",
                "low" => "LOW",
                "lowest" => "LOW",
                _ => "MEDIUM"
            };
        }

        private async Task<int> CreateImportJob(string type, string source, int? startedById)
        {
            var importJob = new ImportJobs
            {
                Type = type,
                Source = source,
                Status = "running",
                //StartedById = startedById,
                StartedAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };

            _context.ImportJobs.Add(importJob);
            await _context.SaveChangesAsync();

            return importJob.Id;
        }

        private async Task UpdateImportJobSuccess(int jobId, ImportResult result)
        {
            var job = await _context.ImportJobs.FindAsync(jobId);
            if (job != null)
            {
                job.Status = "completed";
                job.FinishedAt = DateTime.UtcNow;
                //job.Summary = JsonSerializer.Serialize(new
                //{
                //    result.ImportedProjects,
                //    result.UpdatedProjects,
                //    result.ImportedBoards,
                //    result.ImportedSprints,
                //    result.ImportedEpics,
                //    result.ImportedIssues,
                //    result.ImportedComments,
                //    result.FailedProjects,
                //    result.FailedBoards,
                //    result.FailedSprints,
                //    result.FailedEpics,
                //    result.FailedIssues,
                //    result.FailedComments,
                //    TotalErrors = result.Errors.Count
                //});

                //if (result.Errors.Any())
                //{
                //    job.Details = JsonSerializer.Serialize(new { Errors = result.Errors });
                //}

                await _context.SaveChangesAsync();
            }
        }

        private async Task UpdateImportJobFailure(int jobId, string errorMessage)
        {
            var job = await _context.ImportJobs.FindAsync(jobId);
            if (job != null)
            {
                job.Status = "failed";
                job.FinishedAt = DateTime.UtcNow;
                //job.Details = JsonSerializer.Serialize(new { Error = errorMessage });
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<Project>> GetImportedProjects()
        {
            //return await _context.Projects
            //    .Where(p => p.IsImportedFromJira)
            //    .Include(p => p.Boards)
            //    //.Include(p => p.Sprints)
            //    //.Include(p => p.Epics)
            //    //.Include(p => p.Issues)
            //    //.ToListAsync();
            throw new NotImplementedException();

        }

        public async Task<ImportJobs> GetImportJobStatus(int jobId)
        {
            return await _context.ImportJobs
                .Include(j => j.StartedBy)
                .FirstOrDefaultAsync(j => j.Id == jobId);
        }
    }

    // Result tracking model
    public class ImportResult
    {
        public int ImportedProjects { get; set; }
        public int UpdatedProjects { get; set; }
        public int FailedProjects { get; set; }
        public int ImportedBoards { get; set; }
        public int FailedBoards { get; set; }
        public int ImportedSprints { get; set; }
        public int FailedSprints { get; set; }
        public int ImportedEpics { get; set; }
        public int FailedEpics { get; set; }
        public int ImportedIssues { get; set; }
        public int FailedIssues { get; set; }
        public int ImportedComments { get; set; }
        public int FailedComments { get; set; }
        public List<string> Errors { get; set; } = new List<string>();

        public bool IsSuccess => FailedProjects == 0 && FailedBoards == 0 &&
                                FailedSprints == 0 && FailedEpics == 0 &&
                                FailedIssues == 0 && FailedComments == 0;

        public int TotalImported => ImportedProjects + ImportedBoards +
                                   ImportedSprints + ImportedEpics +
                                   ImportedIssues + ImportedComments;

        public int TotalFailed => FailedProjects + FailedBoards +
                                 FailedSprints + FailedEpics +
                                 FailedIssues + FailedComments;
    }
}