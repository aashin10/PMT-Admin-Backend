using AutoMapper;
using PmtAdmin.Application.Command.Settings;
using PmtAdmin.Application.Dto.SettingsDTO;
using PmtAdmin.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PmtAdmin.Application.MappingProfiles
{
    public class SuperAdminProfile : Profile
    {
        public SuperAdminProfile()
        {
            // Entity -> DTO
            CreateMap<User, SuperAdminDto>();

            //Command->Entity
            CreateMap<CreateSuperAdminCommand, User>();
        }
    }
}
