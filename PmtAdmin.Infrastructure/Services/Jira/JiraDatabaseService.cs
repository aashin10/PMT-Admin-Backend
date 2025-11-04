using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PmtAdmin.Application.Dto;
using PmtAdmin.Domain.Entities;
using PmtAdmin.Infrastructure.Context;
using static PmtAdmin.Domain.Models.JiraImportModels;

namespace PmtAdmin.Infrastructure.Services.Jira
{
    public class JiraDatabaseService : IJiraDatabaseService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public JiraDatabaseService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<JiraImportDatabaseResult> PopulateDataBase(List<JiraProjectData> projects, int importedBy)
        {
            List<OperationResult> operationResults = new List<OperationResult>();
            var response = new JiraImportDatabaseResult();
            var JiraIdToUserIdMappingScheme = new Dictionary<string, int>();
            var returnUsers = new List<User>();
            var returnProjects = new List<Project>();

            var issueStatuses = new List<string> { "TO_DO", "IN_PROGRESS", "DONE" };

            var existingStatuses = _context.Statuses
                .Where(s => issueStatuses.Contains(s.StatusName))
                .Select(s => s.StatusName)
                .ToList();

            var JiraSprintStateToProjectStatusMappingScheme = new Dictionary<string, string>
            {
                { "active", "ACTIVE" },
                { "closed", "COMPLETED" },
                { "future", "PLANNED" }
            };


            HashSet<int> ProjectMembers = new HashSet<int>();

            var newStatuses = issueStatuses
                .Except(existingStatuses)
                .Select(x => new Status { StatusName = x })
                .ToList();

            int PmRoleId = _context.Roles.FirstOrDefault(r => r.Name == "Project Manager")?.Id ?? 1;
            int MemberRoleId = _context.Roles.FirstOrDefault(r => r.Name == "Developer")?.Id ?? 2;

            int projectActiveStatusId = _context.ProjectStatuses.FirstOrDefault(s => s.Name == "Active")?.Id ?? 1;



            if (newStatuses.Any())
            {
                _context.Statuses.AddRange(newStatuses);
                await _context.SaveChangesAsync();
            }

            var allStatuses = _context.Statuses.ToList();

            foreach (var project in projects)
            {
                await using var transaction = await _context.Database.BeginTransactionAsync();
                List<User> tempUsers = new List<User>();
                try
                {
                    var p = _mapper.Map<Project>(project.Project);
                    p.Key = "JIRA - " + p.Key;

                    if (project.Project.Lead.AccountId == null)
                        throw new Exception("No Project Manager Assigned");

                    if (_context.Projects.Any(prj => prj.Key == p.Key))
                        throw new Exception("Project with the same Key already exists.");

                    p.IsImportedFromJira = true;
                    p.StatusId = projectActiveStatusId;

                    var user = await _context.User
                        .FirstOrDefaultAsync(u => u.JiraId == project.Project.Lead.AccountId);

                    if (user == null)
                    {
                        user = _mapper.Map<User>(project.Project.Lead);
                        await _context.User.AddAsync(user);
                        await _context.SaveChangesAsync();
                        returnUsers.Add(user);
                    }

                    p.ProjectManagerId = user.Id;
                    p.ProjectManagerRoleId = PmRoleId;   // 2 for PM Role

                    _context.Projects.Add(p);
                    await _context.SaveChangesAsync();

                    // ensure PM exists in project members only once
                    bool pmExists = await _context.ProjectMembers
                        .AnyAsync(pm => pm.ProjectId == p.Id && pm.UserId == user.Id);

                    int pm_id = 0;

                    if (!pmExists)
                    {
                        var pm = new ProjectMember
                        {
                            ProjectId = p.Id,
                            UserId = user.Id,
                            RoleId = PmRoleId //2 for PM Role
                        };
                        ProjectMembers.Add(user.Id);
                        await _context.ProjectMembers.AddAsync(pm);
                        await _context.SaveChangesAsync();
                        pm_id = pm.Id;
                    }



                    bool noUsers = true;

                    foreach (var role in project.UsersByRole.Keys)
                    {
                        foreach (var u in project.UsersByRole[role])
                        {
                            if (noUsers)
                                noUsers = false;

                            if (!JiraIdToUserIdMappingScheme.ContainsKey(u.AccountId))
                            {
                                var existingUser = await _context.User
                                    .FirstOrDefaultAsync(usr => usr.JiraId == u.AccountId);

                                if (existingUser == null)
                                {
                                    existingUser = _mapper.Map<User>(u);
                                    await _context.User.AddAsync(existingUser);
                                    await _context.SaveChangesAsync();
                                    returnUsers.Add(existingUser);
                                }

                                JiraIdToUserIdMappingScheme[u.AccountId] = existingUser.Id;

                                if (!ProjectMembers.Contains(JiraIdToUserIdMappingScheme[u.AccountId]))
                                {
                                    var projectMember = new ProjectMember
                                    {
                                        ProjectId = p.Id,
                                        UserId = JiraIdToUserIdMappingScheme[u.AccountId],
                                        RoleId = MemberRoleId  //2 for Member Role
                                    };

                                    _context.ProjectMembers.Add(projectMember);
                                    ProjectMembers.Add(projectMember.Id);
                                    await _context.SaveChangesAsync();
                                }
                            }



                        }
                    }

                    if (noUsers)
                    {
                        throw new Exception("Insufficient Jira permissions.");
                    }

                    await _context.SaveChangesAsync();

                    var boards = new List<Board>();
                    var EpicEntityJiraEpicModelMappingScheme = new Dictionary<int, string>();
                    var IssueEntityJiraIssueModelMappingScheme = new Dictionary<int, string>();
                    var SprintEntityJiraSprintModelMappingScheme = new Dictionary<int, string>();
                    var IssueEntityStatusJiraIssueStatusMappingScheme = new Dictionary<int, int>();

                    foreach (var board in project.Boards)
                    {
                        int BoardColumnCount = 0;
                        var issues = new List<Issue>();
                        var sprints = new List<Sprint>();
                        var epics = new List<Epic>();

                        var b = _mapper.Map<Board>(board.BoardInfo);

                        var t = new Team
                        {
                            Name = b.Name + " Team",
                            ProjectId = p.Id,
                            LeadId = pm_id,
                            CreatedBy = importedBy
                        };

                        _context.Teams.Add(t);
                        await _context.SaveChangesAsync();

                        b.TeamId = t.Id;
                        b.ProjectId = p.Id;
                        b.CreatedBy = importedBy;

                        boards.Add(b);
                        _context.Boards.Add(b);
                        await _context.SaveChangesAsync();

                        var TeamMembersIds = new HashSet<string>();

                        foreach (var sprint in board.Sprints)
                        {
                            if (!SprintEntityJiraSprintModelMappingScheme.ContainsKey(sprint.Id))
                            {
                                var sp = _mapper.Map<Sprint>(sprint);
                                sp.ProjectId = p.Id;
                                sp.TeamId = t.Id;
                                sp.Status = JiraSprintStateToProjectStatusMappingScheme.ContainsKey(sprint.State)
                                    ? JiraSprintStateToProjectStatusMappingScheme[sprint.State]
                                    : "PLANNED";
                                sprints.Add(sp);
                                SprintEntityJiraSprintModelMappingScheme[sprint.Id] = sp.Id.ToString();
                            }
                        }

                        foreach (var epic in board.Epics)
                        {
                            if (!EpicEntityJiraEpicModelMappingScheme.ContainsKey(epic.Id))
                            {
                                var e = _mapper.Map<Epic>(epic);
                                e.ProjectId = p.Id;
                                epics.Add(e);
                                EpicEntityJiraEpicModelMappingScheme[epic.Id] = e.Id.ToString();
                            }
                        }

                        foreach (var issue in board.Issues)
                        {
                            if (!IssueEntityJiraIssueModelMappingScheme.ContainsKey(issue.Id))
                            {
                                var i = _mapper.Map<Issue>(issue);

                                if (string.Equals(i.Type, "subtask", StringComparison.OrdinalIgnoreCase))
                                    continue;

                                i.ProjectId = p.Id;
                                i.Title = issue.Summary;
                                i.Type = issue.IssueType.Name.ToUpper();

                                if (issue.Sprint != null)
                                    i.SprintId = SprintEntityJiraSprintModelMappingScheme.ContainsKey(issue.Sprint.Id)
                                        ? Guid.Parse(SprintEntityJiraSprintModelMappingScheme[issue.Sprint.Id])
                                        : (Guid?)null;

                                if (!IssueEntityStatusJiraIssueStatusMappingScheme.ContainsKey(issue.Status.Id))
                                {
                                    var boardcolumn = new BoardColumn
                                    {
                                        BoardColumnName = issue.Status.Name,
                                        BoardColor = "#FFFFFF",
                                        Position = BoardColumnCount
                                    };

                                    BoardColumnCount++;
                                    _context.BoardColumns.Add(boardcolumn);
                                    await _context.SaveChangesAsync();

                                    var tempStatusName = issue.Status.Name.ToUpper().Replace(" ", "_");
                                    var searchedStatus = allStatuses.Find(s => s.StatusName == tempStatusName);

                                    Status statusToUse;
                                    if (searchedStatus == null)
                                    {
                                        statusToUse = new Status { StatusName = tempStatusName };
                                        _context.Statuses.Add(statusToUse);
                                        await _context.SaveChangesAsync();
                                        allStatuses.Add(statusToUse);
                                    }
                                    else
                                    {
                                        statusToUse = searchedStatus;
                                    }

                                    boardcolumn.StatusId = statusToUse.Id;

                                    await _context.SaveChangesAsync();

                                    var boardColumnMapping = new BoardBoardColumnMap
                                    {
                                        BoardId = b.Id,
                                        BoardColumnId = boardcolumn.Id
                                    };

                                    _context.BoardBoardColumnMaps.Add(boardColumnMapping);
                                    IssueEntityStatusJiraIssueStatusMappingScheme[issue.Status.Id] = statusToUse.Id;
                                    await _context.SaveChangesAsync();
                                }
                                else
                                {
                                    i.StatusId = IssueEntityStatusJiraIssueStatusMappingScheme[issue.Status.Id];
                                }

                                if (issue.Epic != null)
                                    i.EpicId = EpicEntityJiraEpicModelMappingScheme.ContainsKey(issue.Epic.Id)
                                        ? Guid.Parse(EpicEntityJiraEpicModelMappingScheme[issue.Epic.Id])
                                        : (Guid?)null;

                                if (issue.Assignee != null)
                                {
                                    if (!JiraIdToUserIdMappingScheme.ContainsKey(issue.Assignee.AccountId))
                                    {
                                        var existingUser = await _context.User
                                            .FirstOrDefaultAsync(u => u.JiraId == issue.Assignee.AccountId);

                                        if (existingUser == null)
                                        {
                                            existingUser = _mapper.Map<User>(issue.Assignee);
                                            await _context.User.AddAsync(existingUser);
                                            await _context.SaveChangesAsync();

                                        }

                                        if (!ProjectMembers.Contains(existingUser.Id))
                                        {
                                            var projectMember = new ProjectMember
                                            {
                                                ProjectId = p.Id,
                                                UserId = existingUser.Id,
                                                RoleId = MemberRoleId  //2 for Member Role
                                            };

                                            await _context.ProjectMembers.AddAsync(projectMember);
                                            await _context.SaveChangesAsync();
                                            ProjectMembers.Add(existingUser.Id);
                                        }


                                        tempUsers.Add(existingUser);

                                        JiraIdToUserIdMappingScheme[issue.Assignee.AccountId] = existingUser.Id;
                                    }

                                    TeamMembersIds.Add(issue.Assignee.AccountId);
                                    i.AssigneeId = JiraIdToUserIdMappingScheme[issue.Assignee.AccountId];
                                }
                                else
                                {
                                    i.AssigneeId = null;
                                }

                                if (issue.Reporter != null)
                                {
                                    if (!JiraIdToUserIdMappingScheme.ContainsKey(issue.Reporter.AccountId))
                                    {
                                        var existingUser = await _context.User
                                            .FirstOrDefaultAsync(u => u.JiraId == issue.Reporter.AccountId);

                                        if (existingUser == null)
                                        {
                                            existingUser = _mapper.Map<User>(issue.Reporter);
                                            await _context.User.AddAsync(existingUser);
                                            await _context.SaveChangesAsync();

                                        }

                                        tempUsers.Add(existingUser);

                                        if (!ProjectMembers.Contains(existingUser.Id))
                                        {
                                            var projectMember = new ProjectMember
                                            {
                                                ProjectId = p.Id,
                                                UserId = existingUser.Id,
                                                RoleId = MemberRoleId  //2 for Member Role
                                            };

                                            await _context.ProjectMembers.AddAsync(projectMember);
                                            await _context.SaveChangesAsync();
                                            ProjectMembers.Add(existingUser.Id);
                                        }


                                        JiraIdToUserIdMappingScheme[issue.Reporter.AccountId] = existingUser.Id;
                                    }

                                    TeamMembersIds.Add(issue.Reporter.AccountId);
                                    i.ReporterId = JiraIdToUserIdMappingScheme[issue.Reporter.AccountId];
                                }

                                i.Labels = JsonConvert.SerializeObject(issue.Labels);
                                i.Status = null;

                                i.StartDate = issue.StartDate.HasValue
                                    ? new DateTimeOffset(issue.StartDate.Value.ToUniversalTime(), TimeSpan.Zero)
                                    : null;

                                i.DueDate = issue.DueDate.HasValue
                                    ? new DateTimeOffset(issue.DueDate.Value.ToUniversalTime(), TimeSpan.Zero)
                                    : null;

                                issues.Add(i);
                                IssueEntityJiraIssueModelMappingScheme[issue.Id] = i.Id.ToString();
                            }



                            await _context.SaveChangesAsync();
                        }

                        foreach (var id in TeamMembersIds)
                        {
                            var pmid = _context.ProjectMembers
                                .FirstOrDefault(pm => pm.User.JiraId == id && pm.ProjectId == p.Id).Id;

                            var newMember = new TeamMember
                            {
                                TeamId = t.Id,
                                ProjectMemberId = pmid
                            };

                            _context.TeamMembers.Add(newMember);
                        }

                        _context.Sprints.AddRange(sprints);
                        _context.Epics.AddRange(epics);
                        _context.Issues.AddRange(issues);
                        await _context.SaveChangesAsync();
                    }
                    operationResults.Add(new OperationResult
                    {
                        Success = true,
                        Message = $"Success: {p.Name} imported successfully."
                    });

                    //returnUsers.Add(user);
                    returnUsers.AddRange(tempUsers);
                    returnProjects.Add(p);
                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    operationResults.Add(new OperationResult
                    {
                        Success = false,
                        Message = $"Failed: {project.Project.Name} - {ex.Message}"
                    });
                    await transaction.RollbackAsync();
                }
            }

            response.Users = returnUsers;
            response.Projects = returnProjects;
            response.Results = operationResults;
            return response;
        }
    }
}
