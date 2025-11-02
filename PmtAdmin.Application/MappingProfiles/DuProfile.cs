using AutoMapper;
using PmtAdmin.Application.Command;
using PmtAdmin.Application.Command.Du;
using PmtAdmin.Application.Dto;
using PmtAdmin.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PmtAdmin.Application.MappingProfiles
{
    public class DuProfile : Profile
    {
        public DuProfile()
        {

            CreateMap<DeliveryUnit, DuDto>()
                        .ForMember(dest => dest.ProjectCount,
                                   opt => opt.MapFrom(src => src.Projects
                                       .Count(p => p.Status != null && p.Status.Name == "Active" && p.DeletedAt == null)));
            CreateMap<CreateDuCommand, DeliveryUnit>()
                           .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                           .ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.Code))
                           .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                           .ForMember(dest => dest.DuHeadName, opt => opt.MapFrom(src => src.HeadName))
                           .ForMember(dest => dest.DuHeadEmail, opt => opt.MapFrom(src => src.HeadEmail));

            CreateMap<UpdateDuCommand, DeliveryUnit>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.Code))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.DuHeadName, opt => opt.MapFrom(src => src.HeadName))
                .ForMember(dest => dest.DuHeadEmail, opt => opt.MapFrom(src => src.HeadEmail));
        }

    }
}

