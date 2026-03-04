using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.Entity
{
    public class AdminDTO
    {
    }

    public class RoleDTO
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Status { get; set; }
    }
    public class PasswordPolicyDTO
    {
        public int Id { get; set; }
        public string? Code { get; set; }
        public string? Item { get; set; }
        public string? Value { get; set; }
        public int Status { get; set; }

    }

    public class UserDetailsDTO
    {
    }

    public class RolePermissionDTO
    {
    }

}
