using DB.EFModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.EFModel
{
    public class UserPasswordHistory:BaseEntity
    {
        public int Id { get; set; }
        public int ResidentId { get; set; }
        public string? PasswordHash { get; set; }
        
    }
}
