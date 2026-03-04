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
    public class RoleMenuPermissionProfile : Profile
    {
        public RoleMenuPermissionProfile()
        {
            CreateMap<RoleMenuPermission, RoleMenuPermissionDTO>();
            CreateMap<RoleMenuPermissionDTO, RoleMenuPermission>();
        }
    }
}
