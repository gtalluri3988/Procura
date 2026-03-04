using BusinessLogic.Interfaces;
using BusinessLogic.Interfaces.Entities;


namespace BusinessLogic.Models.Users
{
    public class User : IUser
    {
        private readonly UserObject _user;
        private readonly IUserService _userService;

        public User(UserObject userObject,IUserService userService)
        {
            _user = userObject;
            _userService = userService;
            _Role = new Lazy<string>(() => _userService.RoleForUser(Id));

        }
        private readonly Lazy<string> _Role;
        public int Id { get => _user.Id; }
        public string Role { get => _Role.Value;}
        public UserObject Details { get => _user; }
        public bool HasRole(Roles role)=>Role.ToString() == role.ToString();
        public bool IsCSAAdmin() => HasRole(Roles.SystemAdmin);

        
    }


}
