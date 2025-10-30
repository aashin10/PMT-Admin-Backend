using AutoMapper;
using Microsoft.EntityFrameworkCore;
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

        public async Task<List<User>> PopulateDataBase(List<JiraProjectData> projects)
        {
            Dictionary<string, int> JiraIdToUserIdMappingScheme = new Dictionary<string, int>();
            List<User> returnUsers = new List<User>();
            List<string> issueStatuses = new List<string>
            {
             "TODO",
             "INPROGRESS",
             "DONE",
            };

            // Get existing status names from the database
            var existingStatuses = _context.Statuses
                .Where(s => issueStatuses.Contains(s.StatusName))
                .Select(s => s.StatusName)
                .ToList();

            // Filter out statuses that already exist
            var newStatuses = issueStatuses
                .Except(existingStatuses)
                .Select(status => new Status { StatusName = status })
                .ToList();

            // Add only new statuses
            if (newStatuses.Any())
            {
                _context.Statuses.AddRange(newStatuses);
                _context.SaveChanges();
            }

            var allStatuses = _context.Statuses.ToList();

            foreach (var project in projects)
            {
                //Map JiraProject to Project entity
                Project p = _mapper.Map<Project>(project.Project);

                p.Key = "JIRA" + p.Key;


                //Check if Project Manager exists in the database
                if (project.Project.Lead.AccountId == null)
                {
                    throw new Exception("No Project Manager Assigned in Jira");
                }

                var user = await _context.User
                    .FirstOrDefaultAsync<User>(u => u.JiraId == project.Project.Lead.AccountId);

                if (user == null)
                {
                    // Create a user for entity if user with JiraId does not exists in the database
                    user = _mapper.Map<User>(project.Project.Lead);
                    await _context.User.AddAsync(user);
                    returnUsers.Add(user);
                    await _context.SaveChangesAsync();

                }
                p.ProjectManagerId = user.Id;
                p.ProjectManagerRoleId = 1; //1=>Admin

                _context.Projects.Add(p);
                await _context.SaveChangesAsync();

                //For each role
                //If not an admin, the roles wont be available/imported
                foreach (var role in project.UsersByRole.Keys)
                {
                    //For each user in the role
                    foreach (var u in project.UsersByRole[role])
                    {
                        //If already gone through this user, skip   
                        if (!JiraIdToUserIdMappingScheme.ContainsKey(u.AccountId))
                        {
                            //Check if user exists in the database
                            var existingUser = await _context.User
                                .FirstOrDefaultAsync(usr => usr.JiraId == u.AccountId);

                            //If not, create new user
                            if (existingUser == null)
                            {
                                // Create a new User entity if not found
                                existingUser = _mapper.Map<User>(u);
                                await _context.User.AddAsync(existingUser);
                                await _context.SaveChangesAsync();
                                returnUsers.Add(existingUser);
                            }

                            //Add to mapping scheme
                            JiraIdToUserIdMappingScheme[u.AccountId] = existingUser.Id;
                        }

                        //Create ProjectMember entity
                        var projectMember = new ProjectMember
                        {
                            ProjectId = p.Id,
                            UserId = JiraIdToUserIdMappingScheme[u.AccountId],
                            RoleId = 1, // Default RoleId, adjust as necessary
                        };

                        _context.ProjectMembers.Add(projectMember);
                        await _context.SaveChangesAsync();

                    }
                }

                await _context.SaveChangesAsync();

                List<Board> boards = new List<Board>();

                Dictionary<int, string> EpicEntityJiraEpicModelMappingScheme = new Dictionary<int, string>();
                Dictionary<int, string> IssueEntityJiraIssueModelMappingScheme = new Dictionary<int, string>();
                Dictionary<int, string> SprintEntityJiraSprintModelMappingScheme = new Dictionary<int, string>();
                Dictionary<int, string> IssueEntityStatusJiraIssueStatusMappingScheme = new Dictionary<int, string>();


                foreach (var board in project.Boards)
                {

                    int BoardColumnCount = 0;
                    List<Issue> issues = new List<Issue>();
                    List<Sprint> sprints = new List<Sprint>();
                    List<Epic> epics = new List<Epic>();
                    //Mapping Board
                    var b = _mapper.Map<Board>(board.BoardInfo);

                    //Creating a Team for the Board
                    var t = new Team
                    {
                        Name = b.Name + " Team",
                        ProjectId = p.Id
                    };

                    _context.Teams.Add(t);
                    await _context.SaveChangesAsync();

                    b.TeamId = t.Id;
                    b.ProjectId = p.Id;
                    _context.Boards.Add(b);
                    await _context.SaveChangesAsync();

                    HashSet<string> TeamMembersIds = new HashSet<string>();

                    foreach (var sprint in board.Sprints)
                    {
                        if (!SprintEntityJiraSprintModelMappingScheme.ContainsKey(sprint.Id))
                        {
                            var sp = _mapper.Map<Sprint>(sprint);
                            sp.ProjectId = p.Id;
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
                            i.ProjectId = p.Id;
                            i.Title = issue.Summary;

                            if (issue.Sprint != null)
                            {
                                i.SprintId = SprintEntityJiraSprintModelMappingScheme.ContainsKey(issue.Sprint.Id)
                                ? Guid.Parse(SprintEntityJiraSprintModelMappingScheme[issue.Sprint.Id]) : (Guid?)null;
                            }

                            if (!IssueEntityStatusJiraIssueStatusMappingScheme.ContainsKey(issue.Status.Id))
                            {
                                //Create a board
                                var boardcolumn = new BoardColumn
                                {
                                    BoardColumnName = issue.Status.Name,
                                    BoardColor = "#FFFFFF",
                                    Position = BoardColumnCount
                                };

                                BoardColumnCount++;
                                _context.BoardColumns.Add(boardcolumn);
                                await _context.SaveChangesAsync();

                                // Replace lines 215-235 with:
                                var tempStatusName = issue.Status.Name.ToUpper().Replace(" ", "");
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
                                _context.BoardColumnMappings.Add(boardColumnMapping);
                                IssueEntityStatusJiraIssueStatusMappingScheme[issue.Status.Id] = statusToUse.Id.ToString();

                                await _context.SaveChangesAsync();
                            }

                            //i.StatusId = IssueEntityStatusJiraIssueStatusMappingScheme.ContainsKey(issue.Status.Id) ? Guid.Parse(SprintEntityJiraSprintModelMappingScheme[issue.Status.Id]) : (Guid?)null;

                            if (issue.Epic != null)
                            {
                                i.EpicId = EpicEntityJiraEpicModelMappingScheme.ContainsKey(issue.Epic.Id)
                                ? Guid.Parse(EpicEntityJiraEpicModelMappingScheme[issue.Epic.Id]) : (Guid?)null;
                            }

                            // Assignee
                            if (issue.Assignee != null)
                            {
                                if (!JiraIdToUserIdMappingScheme.ContainsKey(issue.Assignee.AccountId))
                                {
                                    var existingUser = await _context.User.FirstOrDefaultAsync(u => u.JiraId == issue.Assignee.AccountId);
                                    if (existingUser == null)
                                    {

                                        existingUser = _mapper.Map<User>(issue.Assignee);
                                        await _context.User.AddAsync(existingUser);
                                        await _context.SaveChangesAsync();
                                    }
                                    JiraIdToUserIdMappingScheme[issue.Assignee.AccountId] = existingUser.Id;
                                }
                                TeamMembersIds.Add(issue.Assignee.AccountId);
                                i.AssigneeId = JiraIdToUserIdMappingScheme[issue.Assignee.AccountId];
                            }
                            else
                            {
                                i.AssigneeId = null;
                            }

                            // Reporter
                            if (issue.Reporter != null)
                            {
                                if (!JiraIdToUserIdMappingScheme.ContainsKey(issue.Reporter.AccountId))
                                {
                                    var existingUser = await _context.User.FirstOrDefaultAsync(u => u.JiraId == issue.Reporter.AccountId);
                                    if (existingUser == null)
                                    {
                                        existingUser = _mapper.Map<User>(issue.Reporter);
                                        await _context.User.AddAsync(existingUser);
                                        await _context.SaveChangesAsync();
                                    }
                                    JiraIdToUserIdMappingScheme[issue.Reporter.AccountId] = existingUser.Id;
                                }
                                TeamMembersIds.Add(issue.Assignee.AccountId);
                                i.ReporterId = JiraIdToUserIdMappingScheme[issue.Reporter.AccountId];
                            }

                            //Temp fix
                            i.Labels = "[\"tag1\", \"tag2\"]";

                            //foreach (var comment in issue.Comment)
                            //{
                            //    var ic = _mapper.Map<IssueComment>(comment);
                            //    ic.IssueId = i.Id;
                            //    ic.AuthorId = JiraIdToUserIdMappingScheme.ContainsKey(comment.Author.AccountId)
                            //        ? JiraIdToUserIdMappingScheme[comment.Author.AccountId] : 0;
                            //    i.IssueComments.Add(ic);
                            //}
                            i.Status = null; // To avoid EF Core tracking issues
                            issues.Add(i);
                            IssueEntityJiraIssueModelMappingScheme[issue.Id] = i.Id.ToString();
                        }
                    }


                    _context.Sprints.AddRange(sprints);


                    _context.Epics.AddRange(epics);
                    _context.Issues.AddRange(issues);
                    await _context.SaveChangesAsync();
                }
            }

            return returnUsers;
        }
    }
}
