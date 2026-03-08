using AutoMapper;
using DB.EFModel;
using DB.Entity;
using DB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.Profiles
{
    public class MaterialBudgetProfile : Profile
    {
        public MaterialBudgetProfile()
        {
            CreateMap<MaterialBudget, MaterialBudgetDto>();
            CreateMap<MaterialBudgetDto, MaterialBudget>();
        }
    }
}
