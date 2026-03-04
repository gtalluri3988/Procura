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
            //CreateMap<UserDTO, Users>()
            //   .ForMember(dest => dest.Role, opt => opt.Ignore());
            CreateMap<User, UserDTO>();
            CreateMap<UserDTO, User>();
        }
    }
}
