using AutoMapper;
using PmtAdmin.Application.Dto;
using PmtAdmin.Domain.Entities;

namespace PmtAdmin.Application.MappingProfiles
{
    public class ProjectProfile : Profile
    {
        public ProjectProfile()
        {
            CreateMap<Project, ProjectDTO>();
        }

    }
}
