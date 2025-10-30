using AutoMapper;
using PmtAdmin.Application.Dto;
using PmtAdmin.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PmtAdmin.Application.MappingProfiles
{
    public class ProjectStatusProfile : Profile
    {
        public ProjectStatusProfile()
        {
            CreateMap<ProjectStatus, ProjectStatusDto>();
        }
    }
}
