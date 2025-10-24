using AutoMapper;
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
            // Implementation for populating the database with the provided project data
            foreach (var project in projects)
            {
                Project p1 = _mapper.Map<Project>(project.Project);


                List<Board> boards = new List<Board>();

                foreach (var board in project.Boards)
                {
                    var b = _mapper.Map<Board>(board.BoardInfo);

                    b.Epics = _mapper.Map<List<Epic>>(board.Epics);
                    List<Issue> issues = new List<Issue>();

                    foreach (var issue in board.Issues)
                    {
                        var c = _mapper.Map<Issue>(issue);
                        c.IssueComments = _mapper.Map<List<IssueComment>>(issue.Comment);
                        issues.Add(c);
                    }

                    b.Issues = issues;
                    b.Sprints = _mapper.Map<List<Sprint>>(board.Sprints);

                    boards.Add(b);
                }



                p1.Boards = boards;
                _context.Projects.Add(p1);
                await _context.SaveChangesAsync();
                // Further mapping and database population logic goes here
            }
        }
    }
}
