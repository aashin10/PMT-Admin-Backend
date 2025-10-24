using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PmtAdmin.Domain.Entities;
using PmtAdmin.Infrastructure.Context;
using PmtAdmin.Infrastructure.Logging;
using static PmtAdmin.Infrastructure.Models.JiraImportModels;

namespace PmtAdmin.Infrastructure.Services.Jira
{
    public class JiraDatabaseService : IJiraDatabaseService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly IAppLogger<JiraDatabaseService> _logger;


        public JiraDatabaseService(AppDbContext context, IMapper mapper, IAppLogger<JiraDatabaseService> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task PopulateDataBase(List<JiraProjectData> projects)
        {
            foreach (var project in projects)
            {
                _logger.LogInformation("Importing Project: {ProjectName}", project.Project.Name);
                //Map JiraProject to Project entity
                Project p = _mapper.Map<Project>(project.Project);

                //Check if Project Manager exists in the database
                if (project.Project.Lead.AccountId == null)
                {
                    _logger.LogError("Project Lead AccountId is null for project: {ProjectName}", project.Project.Name);
                    throw new Exception();
                }
                var user = await _context.Users
                    .FirstOrDefaultAsync<User>(u => u.JiraId == project.Project.Lead.AccountId);
                if (user == null)
                {
                    // Create a user for entity if user with JiraId does not exists in the database
                    user = _mapper.Map<User>(project.Project.Lead);
                    await _context.Users.AddAsync(user);
                    await _context.SaveChangesAsync();

                }
                _logger.LogInformation("Team Lead User Id: {UserId}", user.Id);
                p.ProjectManagerId = user.Id;

                await _context.SaveChangesAsync();

                List<Board> boards = new List<Board>();

                Dictionary<int, string> EpicEntityJiraEpicModelMappingScheme = new Dictionary<int, string>();
                Dictionary<int, int> IssueEntityJiraIssueModelMappingScheme = new Dictionary<int, int>();
                Dictionary<int, string> SprintEntityJiraSprintModelMappingScheme = new Dictionary<int, string>();

                List<Epic> epics = new List<Epic>();
                List<Issue> issues = new List<Issue>();
                List<Sprint> sprints = new List<Sprint>();
                _logger.LogInformation("Importing Boards for Project: {ProjectName}", project.Project.Name);
                foreach (var board in project.Boards)
                {

                    var b = _mapper.Map<Board>(board.BoardInfo);

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
                            issues.Add(i);
                            i.SprintId = SprintEntityJiraSprintModelMappingScheme.ContainsKey(issue.Sprint.Id)
                                ? Guid.Parse(SprintEntityJiraSprintModelMappingScheme[issue.Sprint.Id]) : (Guid?)null;

                            i.EpicId = EpicEntityJiraEpicModelMappingScheme.ContainsKey(issue.Epic.Id)
                                ? Guid.Parse(EpicEntityJiraEpicModelMappingScheme[issue.Epic.Id]) : (Guid?)null;

                            i.IssueComments = _mapper.Map<List<IssueComment>>(issue.Comment);

                            IssueEntityJiraIssueModelMappingScheme[issue.Id] = i.Id;
                        }
                    }
                }
            }
        }
    }
}
