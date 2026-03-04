using DB.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Interfaces
{
    public interface IPasswordPolicyService
    {
        Task<IEnumerable<PasswordPolicyDTO>> GetAllPasswordPolicyAsync();
        Task<bool> SavePasswordPolicyAsync(List<PasswordPolicyDTO> dto);
    }
}
