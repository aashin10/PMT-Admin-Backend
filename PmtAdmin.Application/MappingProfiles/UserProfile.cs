using AutoMapper;
using PmtAdmin.Application.Command;
using PmtAdmin.Application.Dto;
using PmtAdmin.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PmtAdmin.Application.MappingProfiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            // Command to Entity
            CreateMap<CreateUserCommand, User>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
                .ForMember(dest => dest.LastLogin, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedBy, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore());

            // Entity to DTO
            CreateMap<User, UserDto>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.IsActive ? "Active" : "Inactive"))
                .ForMember(dest => dest.Created_At, opt => opt.MapFrom(src =>
                    src.CreatedAt.ToString("MM/dd/yyyy")))
                .ForMember(dest => dest.Last_Login, opt => opt.MapFrom(src =>
                    src.LastLogin.HasValue ? src.LastLogin.Value.ToString("MM/dd/yyyy") : null));
        }
    }
}
