using AutoMapper;
using DB.EFModel;
using DB.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserDTO>()
                // User.SiteOffice is the int FK; UserDTO.SiteOfficeId holds it
                .ForMember(dest => dest.SiteOfficeId, opt => opt.MapFrom(src => src.SiteOffice))
                // User.State is the navigation property; UserDTO.SiteOffice holds it
                .ForMember(dest => dest.SiteOffice, opt => opt.MapFrom(src => src.State))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.EmailAddress))
                .ForMember(dest => dest.Mobile, opt => opt.MapFrom(src => src.MobileNo))
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FullName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.FullName));

            CreateMap<UserDTO, User>()
                // UserDTO.SiteOfficeId is the int FK; User.SiteOffice holds it
                .ForMember(dest => dest.SiteOffice, opt => opt.MapFrom(src => src.SiteOfficeId))
                // User.State is the navigation property — do not overwrite it from DTO
                .ForMember(dest => dest.State, opt => opt.Ignore())
                .ForMember(dest => dest.SiteLevel, opt => opt.Ignore())
                .ForMember(dest => dest.Role, opt => opt.Ignore())
                .ForMember(dest => dest.Designation, opt => opt.Ignore())
                .ForMember(dest => dest.EmailAddress, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.MobileNo, opt => opt.MapFrom(src => src.Mobile));
        }
    }
}
