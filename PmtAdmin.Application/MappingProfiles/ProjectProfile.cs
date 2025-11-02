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
            CreateMap<Team, TeamDTO>().ReverseMap();
        }
    }
}