using BusinessLogic.Interfaces.Entities;
using BusinessLogic.Models.Users;
using DB.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Interfaces
{
    public interface IUserService
    {
        IUser Authenticate(string username, string password,int RoleId ,out AuthenticationError loginError);
        //IUser GetByEmail(string? username);
        void LogAuthAttempt(string Username, string ip, string response,DateTime? jwtExpiryDate,bool online);
        IUser GetByUsername(string userName);
        //Task<User> GetCurrentUserAsync(string userName);
        Task<bool> CheckPassword(string userName, string password,int roleId);
        string GetUserId();
        string GetUserName();
        List<string> GetUserRoles();
        string RoleForUser(int userId);
        Task<IEnumerable<UserDTO>> GetUserListAsync(); 
        Task<IEnumerable<UserDTO>> GetBiddingUsersList();
        Task<UserDTO> CreateUserAsync(UserDTO dto);
        Task UpdateUserAsync(int id, UserDTO dto);
        Task<UserDTO> GetUserByIdAsync(int id);
        //Task<string> CheckResidentUserByEmail(string userName);
        //Task UpdateResidentPsswordAsync(int residentId, string currentPassword, string newPassword);
        //Task<RoleDTO> RoleIdUser(int userId);
    }
    public enum AuthenticationErrorReason
    {
        UserDeactivated,
        Other
    }
    public class AuthenticationError
    {
        public AuthenticationErrorReason Reason { get; init; }
        public string Message { get; init; }
        public static AuthenticationError UserDeactivated(string Message) => new()
        {
            Reason = AuthenticationErrorReason.UserDeactivated,
            Message = Message
        };
        public static AuthenticationError Other(string Message) => new()
        {
            Reason = AuthenticationErrorReason.UserDeactivated,
            Message = Message
        };
    }
}
