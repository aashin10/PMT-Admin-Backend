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
                .ForMember(dest => dest.Created_At, opt => opt.Ignore())
                .ForMember(dest => dest.Updated_At, opt => opt.Ignore())
                .ForMember(dest => dest.Deleted_At, opt => opt.Ignore())
                .ForMember(dest => dest.Last_Login, opt => opt.Ignore())
                .ForMember(dest => dest.Updated_By, opt => opt.Ignore())
                .ForMember(dest => dest.Deleted_By, opt => opt.Ignore())
                .ForMember(dest => dest.Is_Deleted, opt => opt.Ignore());

            // Entity to DTO
            CreateMap<User, UserDto>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Is_Active ? "Active" : "Inactive"))
                .ForMember(dest => dest.Created_At, opt => opt.MapFrom(src =>
                    src.Created_At.ToString("MM/dd/yyyy")))
                .ForMember(dest => dest.Last_Login, opt => opt.MapFrom(src =>
                    src.Last_Login.HasValue ? src.Last_Login.Value.ToString("MM/dd/yyyy") : null));
        }
    }
}
