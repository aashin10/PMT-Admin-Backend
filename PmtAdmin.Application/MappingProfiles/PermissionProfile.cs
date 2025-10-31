using AutoMapper;
using PmtAdmin.Domain.Entities;
using RoleManagement.Application.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PmtAdmin.Application.MappingProfiles
{
   
    
        public class PermissionProfile : Profile
        {
            public PermissionProfile()
            {
                CreateMap<Permission, PermissionDto>();
            }
        }
    }

