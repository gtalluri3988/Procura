using AutoMapper;
using DB.EFModel;
using DB.Entity;

namespace DB.Profiles
{
    public class CategoryCodeApprovalProfile : Profile
    {
        public CategoryCodeApprovalProfile()
        {
            CreateMap<CategoryCodeApproval, CategoryCodeApprovalDto>();
            CreateMap<CategoryCodeApprovalDto, CategoryCodeApproval>();

            CreateMap<CategoryCodeApprovalItem, CategoryCodeApprovalItemDto>();
            CreateMap<CategoryCodeApprovalItemDto, CategoryCodeApprovalItem>();
        }
    }
}
