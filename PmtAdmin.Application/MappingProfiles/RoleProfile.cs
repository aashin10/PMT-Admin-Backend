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
    public class RoleProfile : Profile
    {
        public RoleProfile()
        {
            CreateMap<Role, RoleDto>()
                // Convert DateTime to string (formatted)
                .ForMember(dest => dest.CreatedAt,
                    opt => opt.MapFrom(src => src.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss")))

                // Example: count of users associated with the role
                .ForMember(dest => dest.UserCount,
                    opt => opt.MapFrom(src => src.ProjectMembers != null ? src.ProjectMembers.Count : 0))

                // Map RolePermissions → PermissionDto
                .ForMember(dest => dest.Permissions,
                    opt => opt.MapFrom(src =>
                        src.RolePermissions != null
                            ? src.RolePermissions.Select(rp => new PermissionDto
                            {
                                Id = rp.Permission.Id,
                                Name = rp.Permission.Name,
                                Description = rp.Permission.Description
                            }).ToList()
                            : new List<PermissionDto>()
                    ));

            // Optional reverse mapping (if you ever need it)
            CreateMap<RoleDto, Role>()
                .ForMember(dest => dest.Id, opt => opt.Ignore()) // usually handled by DB
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.RolePermissions, opt => opt.Ignore())
                .ForMember(dest => dest.ProjectMembers, opt => opt.Ignore())
                .ForMember(dest => dest.ProjectManagerRoles, opt => opt.Ignore());
        }
    }
}

