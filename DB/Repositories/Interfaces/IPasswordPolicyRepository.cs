using DB.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.Repositories.Interfaces
{
    public interface IPasswordPolicyRepository
    {
        Task<IEnumerable<PasswordPolicyDTO>> GetAllAsync();
        Task<PasswordPolicyDTO> GetByIdAsync(int id);
        Task<PasswordPolicyDTO> AddAsync(PasswordPolicyDTO dto);
        Task UpdateAsync(int id, PasswordPolicyDTO dto);
        Task DeleteAsync(int id);
        Task<IEnumerable<PasswordPolicyDTO>> GetAllPasswordPolicyDetails();
        Task<bool> SavePasswordPolicyAsync(List<PasswordPolicyDTO> policies);


    }
}
