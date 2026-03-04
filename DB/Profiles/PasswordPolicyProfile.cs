using AutoMapper;
using DB.EFModel;
using DB.Entity;

namespace DB.Profiles
{
    public class PasswordPolicyProfile : Profile
    {
        public PasswordPolicyProfile()
        {
            CreateMap<PasswordPolicy, PasswordPolicyDTO>();
            CreateMap<PasswordPolicyDTO, PasswordPolicy>();
        }
    }
}
