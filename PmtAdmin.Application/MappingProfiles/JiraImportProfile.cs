using AutoMapper;
using PmtAdmin.Domain.Entities;
using static PmtAdmin.Infrastructure.Models.JiraImportModels;


namespace PmtAdmin.Application.MappingProfiles
{
    public class JiraImportProfile : Profile
    {
        public JiraImportProfile()
        {
            CreateMap<JiraProject, Project>()
                //.ForMember(dest => dest.ProjectManager, opt => opt.MapFrom(src => src.Lead))
                .ForMember(dest => dest.IsImportedFromJira, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()));

            CreateMap<JiraBoard, Board>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());

            CreateMap<JiraIssue, Issue>()
    .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
    .ForMember(dest => dest.ProjectId, opt => opt.Ignore())
    .ForMember(dest => dest.EpicId, opt => opt.Ignore())
    .ForMember(dest => dest.SprintId, opt => opt.Ignore())
    .ForMember(dest => dest.Sprint, opt => opt.Ignore())
    .ForMember(dest => dest.Epic, opt => opt.Ignore())
    .ForMember(dest => dest.Assignee, opt => opt.Ignore())
        .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.Name))
    .ForMember(dest => dest.Reporter, opt => opt.Ignore())
        .ForMember(dest => dest.Epic, opt => opt.Ignore())
    .ForMember(dest => dest.Project, opt => opt.Ignore())
    .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
    .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.Priority, opt => opt.MapFrom(src => src.Priority.Name));

            CreateMap<JiraEpic, Epic>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Summary));

            CreateMap<JiraUser, User>()
                .ForMember(dest => dest.JiraId, opt => opt.MapFrom(src => src.AccountId))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.DisplayName));

            CreateMap<JiraSprint, Sprint>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()));
            CreateMap<JiraComment, IssueComment>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());
            CreateMap<JiraTeam, Team>();


        }
    }
}
