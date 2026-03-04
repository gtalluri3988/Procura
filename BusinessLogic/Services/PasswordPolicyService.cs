using BusinessLogic.Interfaces;
using DB.Entity;
using DB.Repositories;
using DB.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services
{
    public class PasswordPolicyService:IPasswordPolicyService
    {
        private readonly IPasswordPolicyRepository _passwordpolicyRepository;

        public PasswordPolicyService(IPasswordPolicyRepository passwordpolicyRepository)
        {
            _passwordpolicyRepository = passwordpolicyRepository;
        }

        public async Task<IEnumerable<PasswordPolicyDTO>> GetAllPasswordPolicyAsync()
        {
            return await _passwordpolicyRepository.GetAllPasswordPolicyDetails();
        }
        public async Task<bool> SavePasswordPolicyAsync(List<PasswordPolicyDTO> dto)
        {
            return await _passwordpolicyRepository.SavePasswordPolicyAsync(dto);
        }
    }
}
