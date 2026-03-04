using AutoMapper;
using DB.EFModel;
using DB.Entity;


namespace DB.Profiles
{
    public class ContentProfile : Profile
    {
        public ContentProfile()
        {

            CreateMap<ContentManagement, ContentManagementDTO>();
            CreateMap<ContentManagementDTO, ContentManagement>();
        }
    }
}
