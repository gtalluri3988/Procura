
using DB.EFModel;
using DB.Entity;

namespace DB.Repositories.Interfaces
{
    public interface IUserRepository 
    {
        User GetUserByUsernameAsync(string email,bool includeInactive=false);
        void LogAuthAttempt(string username, string ip, string response, DateTime? jwtExpiryDate, bool isOnline);
        Task<UserDTO> GetByIdAsync(int id);
        Task<bool> CheckPassword(string userName,string Password,int roleId);
        int? RoleForUser(int userId);
        Task<IEnumerable<UserDTO>> GetUserListAsync();

        Task<IEnumerable<UserDTO>> GetBidderUserListAsync(int roleId);
        Task<UserDTO> SaveUserAsync(UserDTO user);
        Task UpdateUserAsync(int userId, UserDTO user);

        Task UpdateAsync(int id, UserDTO dto);
        //Task<RoleDTO?> GetRoleAsync(int userId);

        Task<IEnumerable<UserDTO>> GetUserListAsync(int? siteLevelId, int? siteOfficeId, bool? status);
        Task<IEnumerable<UserDTO>> GetBidderUserListAsync(int? siteLevelId, int? siteOfficeId, bool? status);





        Task<bool> IsPasswordReusedAsync(int userId, string newHash, int historyCount);
        Task SaveHistoryAsync(int userId, string passwordHash, int historyCount);

    }
    
}
