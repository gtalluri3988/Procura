using BusinessLogic.Interfaces;
using BusinessLogic.Interfaces.Entities;
using BusinessLogic.Models;
using BusinessLogic.Models.Users;
using DB.Entity;
using DB.Repositories;
using DB.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Security.Claims;

namespace BusinessLogic.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IVendorRepository _vendorRepository;

        private readonly IHttpContextAccessor _httpContextAccessor;
        public UserService(IUserRepository userRepository, IVendorRepository vendorRepository,
            IHttpContextAccessor httpContextAccessor)
        {
            _userRepository = userRepository;
            _vendorRepository=vendorRepository;
            _httpContextAccessor = httpContextAccessor;


        }
        public IUser Authenticate(string email, string password, int RoleId, out AuthenticationError error)
        {
            try
            {
                (var user, error) = DoAuthenticate(email, password, RoleId);
                if (user != null)
                {
                    return user;
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }
            return null;
        }
        private (IUser user, AuthenticationError error) DoAuthenticate(string userName, string password, int RoleId)
        {
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
            {
                return (null, AuthenticationError.Other("Password or Email is empty"));
            }
            userName = userName.ToLower();
            var host = string.Empty;
            if (RoleId == 0)
            {
                var useObj1 = _userRepository.GetUserByUsernameAsync(userName, includeInactive: true);
                UserObject userObject = new UserObject();
                if (useObj1 != null)
                {
                    userObject.Id = useObj1.Id;
                    userObject.Email = useObj1?.EmailAddress?.Trim() ?? "";
                    userObject.UserName = useObj1?.UserName?.Trim() ?? "";
                    userObject.FirstName = useObj1?.FullName?.Trim() ?? "";
                    userObject.LastName = useObj1?.FullName?.Trim() ?? "";
                    userObject.RoleId = useObj1.RoleId == null ? 0 : useObj1.RoleId;
                }
                var user = new User(userObject, this);
                return (user, AuthenticationError.Other("Wrong username or password"));
            }
            else
            {
                var useObj1 =_vendorRepository.GetVendorByROCandPasswordAsync(userName, password);
                UserObject userObject = new UserObject();
                if (useObj1 != null)
                {
                    userObject.Id = useObj1.Id;
                    userObject.Email = useObj1?.Email?.Trim() ?? "";
                    userObject.UserName = useObj1?.RocNumber?.Trim() ?? "";
                    userObject.FirstName = useObj1?.CompanyName?.Trim() ?? "";
                    userObject.LastName = useObj1?.CompanyName?.Trim() ?? "";
                    userObject.RoleId = useObj1.RoleId;
                   // userObject.IsFirstTimeLogin = useObj1.IsFirstTimeLogin;
                }
                var user = new User(userObject, this);
                return (user, AuthenticationError.Other("Wrong username or password"));
            }

        }
        public async Task<bool> CheckPassword(string userName, string password, int roleId)
        {
            return await _userRepository.CheckPassword(userName, password, roleId);

        }

        public void LogAuthAttempt(string username, string ip, string response, DateTime? jwtExpiryDate, bool isOnline)
        {
            _userRepository.LogAuthAttempt(username, ip, response, jwtExpiryDate, isOnline);
        }

        public IUser GetByUsername(string userName)
        {
            var useObj = _userRepository.GetUserByUsernameAsync(userName, includeInactive: true);
            UserObject userObject = new UserObject();
            if (useObj != null)
            {
                userObject.Id = useObj.Id;
                //userObject.Email = useObj?.Email?.Trim() ?? "";
                //userObject.UserName = useObj?.UserName?.Trim() ?? "";
                //userObject.FirstName = useObj?.FirstName?.Trim() ?? "";
                //userObject.LastName = useObj?.LastName?.Trim() ?? "";
            }
            return new User(userObject, this);
        }
        public string RoleForUser(int userId)
        {
            var roleId = _userRepository.RoleForUser(userId);

            if (roleId != null && Enum.IsDefined(typeof(Roles), roleId))
            {
                var roleEnum = (Roles)roleId!;
                return GetEnumDisplayName(roleEnum);
            }

            return string.Empty;
        }
        public static string GetEnumDisplayName(Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            var attr = field?.GetCustomAttribute<DisplayAttribute>();
            return attr?.Name ?? value.ToString();
        }
        //public async Task<RoleDTO> RoleIdUser(int userId)
        //{
        //    var roleId = await _userRepository.GetRoleAsync(userId);
        //    if (roleId == null)
        //    {
        //        throw new KeyNotFoundException($"No role found for user with ID {userId}");
        //    }
        //    return roleId;
        //}
        public string GetUserId()
        {
            return _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }

        public string GetUserName()
        {
            return _httpContextAccessor.HttpContext?.User.Identity?.Name;
        }

        public List<string> GetUserRoles()
        {
            return _httpContextAccessor.HttpContext?.User.FindAll(ClaimTypes.Role).Select(r => r.Value).ToList() ?? new List<string>();
        }

        public Task<IEnumerable<UserDTO>> GetUserListAsync()
        {
            return _userRepository.GetUserListAsync();
        }

        public Task<IEnumerable<UserDTO>> GetBiddingUsersList()
        {
            return _userRepository.GetBidderUserListAsync((int)Roles.Bidder);
        }
        
        public async Task<UserDTO> CreateUserAsync(UserDTO dto)
        {
            try
            {
                return await _userRepository.SaveUserAsync(dto);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message); // Return only the error message
            }

        }

        public async Task UpdateUserAsync(int id, UserDTO dto)
        {
            await _userRepository.UpdateUserAsync(id, dto);
        }

        public async Task<UserDTO> GetUserByIdAsync(int id)
        {
            return await _userRepository.GetByIdAsync(id);
        }

        //public async Task<string> CheckResidentUserByEmail(string userName)
        //{
        //    return await _userRepository.CheckResidentUserByEmail(userName);
        //}

        //public async Task UpdateResidentPsswordAsync(int residentId, string currentPassword, string newPassword)
        //{
        //     await _userRepository.UpdateResidentPsswordAsync(residentId, currentPassword, newPassword);
        //}
    }
}
