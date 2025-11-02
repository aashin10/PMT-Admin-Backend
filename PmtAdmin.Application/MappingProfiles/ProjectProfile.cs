using AutoMapper;
using PmtAdmin.Application.Dto;
using PmtAdmin.Domain.Entities;
using System.Linq;

namespace PmtAdmin.Application.MappingProfiles
{
    public class ProjectProfile : Profile
    {
        public ProjectProfile()
        {
            CreateMap<Project, ProjectDTO>()
                .ForMember(dest => dest.ProjectManagerName, opt => opt.MapFrom(src => src.ProjectManager != null ? src.ProjectManager.Name : null))
                .ForMember(dest => dest.StatusName, opt => opt.MapFrom(src => src.Status != null ? src.Status.Name : null))
                .ForMember(dest => dest.DeliveryUnitName, opt => opt.MapFrom(src => src.DeliveryUnit != null ? src.DeliveryUnit.Name : null))
                .ForMember(dest => dest.DeliveryUnitCode, opt => opt.MapFrom(src => src.DeliveryUnit != null ? src.DeliveryUnit.Code : null))
                .ForMember(dest => dest.AdditionalInformation, opt => opt.MapFrom(src => src.CustomFields))
                .ForMember(dest => dest.Teams, opt => opt.MapFrom(src => src.Teams));

            CreateMap<CustomField, CustomFieldDTO>().ReverseMap();
            
            CreateMap<Team, TeamDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name));
                
            CreateMap<TeamDTO, Team>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Description, opt => opt.Ignore())
                .ForMember(dest => dest.ProjectId, opt => opt.Ignore())
                .ForMember(dest => dest.LeadId, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Project, opt => opt.Ignore())
                .ForMember(dest => dest.Lead, opt => opt.Ignore())
                .ForMember(dest => dest.Creator, opt => opt.Ignore())
                .ForMember(dest => dest.Updater, opt => opt.Ignore())
                .ForMember(dest => dest.Boards, opt => opt.Ignore())
                //.ForMember(dest => dest.ProjectMembers, opt => opt.Ignore())
                .ForMember(dest => dest.Channels, opt => opt.Ignore());
                
            // Board mappings
            CreateMap<Board, BoardDTO>()
                .ForMember(dest => dest.Columns, opt => opt.Ignore()); // Handle separately if needed
                
            CreateMap<BoardColumn, BoardColumnDTO>()
                .ForMember(dest => dest.StatusName, opt => opt.MapFrom(src => src.Status != null ? src.Status.StatusName : null));
        }
    }
}