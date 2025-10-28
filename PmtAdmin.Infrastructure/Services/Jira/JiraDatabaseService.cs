using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PmtAdmin.Domain.Entities;
using PmtAdmin.Infrastructure.Context;
using static PmtAdmin.Infrastructure.Models.JiraImportModels;

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

        public async Task PopulateDataBase(List<JiraProjectData> projects)
        {
            Dictionary<string, int> JiraIdToUserIdMappingScheme = new Dictionary<string, int>();

            foreach (var project in projects)
            {
                //Map JiraProject to Project entity
                Project p = _mapper.Map<Project>(project.Project);


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
                    await _context.SaveChangesAsync();

                }
                p.ProjectManagerId = user.Id;

                _context.Projects.Add(p);

                //TODO: Bug wherein multiple users is created with same JIRA ID

                //For each role
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

                    }
                }

                await _context.SaveChangesAsync();

                List<Board> boards = new List<Board>();

                Dictionary<int, string> EpicEntityJiraEpicModelMappingScheme = new Dictionary<int, string>();
                Dictionary<int, string> IssueEntityJiraIssueModelMappingScheme = new Dictionary<int, string>();
                Dictionary<int, string> SprintEntityJiraSprintModelMappingScheme = new Dictionary<int, string>();




                foreach (var board in project.Boards)
                {


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

                            if (issue.Sprint != null)
                            {
                                i.SprintId = SprintEntityJiraSprintModelMappingScheme.ContainsKey(issue.Sprint.Id)
                                ? Guid.Parse(SprintEntityJiraSprintModelMappingScheme[issue.Sprint.Id]) : (Guid?)null;
                            }

                            if (issue.Epic != null)
                            {
                                i.EpicId = EpicEntityJiraEpicModelMappingScheme.ContainsKey(issue.Epic.Id)
                                ? Guid.Parse(EpicEntityJiraEpicModelMappingScheme[issue.Epic.Id]) : (Guid?)null;
                            }

                            //Temp fix
                            i.Labels = "[\"tag1\", \"tag2\"]";

                            //foreach (var comment in issue.Comment)
                            //{
                            //    var ic = _mapper.Map<IssueComment>(comment);
                            //    ic.IssueId = i.Id;
                            //    ic.AuthorId = JiraIdToUserIdMappingScheme.ContainsKey(comment.Author.AccountId)
                            //        ? JiraIdToUserIdMappingScheme[comment.Author.AccountId] : 0; // Default to 0 if not found
                            //    i.IssueComments.Add(ic);
                            //}
                            issues.Add(i);
                            IssueEntityJiraIssueModelMappingScheme[issue.Id] = i.Id.ToString();
                        }
                    }


                    _context.Sprints.AddRange(sprints);
                    _context.Boards.Add(b);

                    _context.Epics.AddRange(epics);
                    _context.Issues.AddRange(issues);
                    await _context.SaveChangesAsync();
                }
            }
        }
    }
}
