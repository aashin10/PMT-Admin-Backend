using AutoMapper;
using MediatR;
using PmtAdmin.Application.Command;
using PmtAdmin.Application.Dto;
using PmtAdmin.Application.Wrappers;
using PmtAdmin.Infrastructure.Services.Jira;
using static PmtAdmin.Domain.Models.JiraImportModels;

namespace PmtAdmin.Application.Handlers.Jira
{
    public class ImportFromJiraCommandHandler : IRequestHandler<ImportFromJiraCommand, ApiResponse<JiraImportResult>>
    {
        private readonly IJiraService _jiraService;
        private readonly IJiraDatabaseService _jiraDatabaseService;
        private readonly IMapper _mapper;
        public ImportFromJiraCommandHandler(IJiraService jiraService, IJiraDatabaseService jiraDatabaseService, IMapper mapper)
        {
            _jiraService = jiraService;
            _jiraDatabaseService = jiraDatabaseService;
            _mapper = mapper;
        }

        public async Task<ApiResponse<JiraImportResult>> Handle(ImportFromJiraCommand request, CancellationToken cancellationToken)
        {
            var results = new List<JiraProjectData>();
            foreach (var id in request.ProjectIds)
            {
                var project = await _jiraService.GetFullProjectDataAsync(request.BaseUrl, request.JiraAccessToken, id.Trim());
                results.Add(project);
            }
            var users = await _jiraDatabaseService.PopulateDataBase(results);

            var usersDto = _mapper.Map<List<UserDto>>(users.Users);
            var projectsDto = _mapper.Map<List<ProjectDTO>>(users.Projects);

            var response = new JiraImportResult();
            response.ImportFailed = false;
            response.Users = usersDto;
            response.Projects = projectsDto;

            return ApiResponse<JiraImportResult>.Success(response);
        }
    }
}
