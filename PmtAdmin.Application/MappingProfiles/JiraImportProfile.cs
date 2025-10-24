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
                .ForMember(dest => dest.IsImportedFromJira, opt => opt.MapFrom(src => true));

            CreateMap<JiraBoard, Board>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());


            CreateMap<JiraIssue, Issue>();

            CreateMap<JiraEpic, Epic>();
            CreateMap<JiraUser, User>()
                .ForMember(dest => dest.JiraId, opt => opt.MapFrom(src => src.AccountId))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.DisplayName));

            CreateMap<JiraSprint, Sprint>();
            //CreateMap<JiraComment, Comment>();
            CreateMap<JiraTeam, Team>();


        }
    }
}
