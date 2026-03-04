using DB.EFModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.Entity
{
    public class ContentManagementDTO
    {
        public int Id { get; set; }  // Primary Key
        public string? Title { get; set; }
        public string? Description { get; set; }

        public int CommunityId { get; set; }
        public bool StatusId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public CommunityDTO? community { get; set; }
        public List<ContentPicture>? ContentPictures { get; set; }
    }
}
