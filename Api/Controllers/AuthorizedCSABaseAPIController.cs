using BusinessLogic.Interfaces;
using BusinessLogic.Interfaces.Entities;
using BusinessLogic.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Api.Controllers
{
    [Authorize]
    public class AuthorizedCSABaseAPIController : ControllerBase
    {

        protected readonly ILogger<AuthorizedCSABaseAPIController> _logger;
        protected readonly IUserService _userService;
        private IUser _currentUser;

        public AuthorizedCSABaseAPIController(IUserService userService,ILogger<AuthorizedCSABaseAPIController> logger)
        {
            _userService = userService;
            _logger = logger;
        }
        public IUser CurrentUser
        {
            get
            {
                _currentUser ??= GetCurrentUser();
                return _currentUser;
            }
        }

        private IUser GetCurrentUser()
        {
            try
            {
                string username = HttpContext.User.Identity.Name;
                if (!string.IsNullOrEmpty(username))
                {
                    return _userService.GetByUsername(username);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error");
            }
            return null;
        }
        protected bool IsProcuraAdmin()
        {
            return HttpContext.User.Claims.Any(x => x.Value == GetEnumDisplayName(Roles.SystemAdmin));
        }
        protected bool IsBusinessUser()
        {
            return HttpContext.User.Claims.Any(x => x.Value == GetEnumDisplayName(Roles.BusinessUser));
        }
        protected bool IsVendorUser()
        {
            return HttpContext.User.Claims.Any(x => x.Value == GetEnumDisplayName(Roles.Vendor));
        }
        public static string GetEnumDisplayName(Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            var attr = field?.GetCustomAttribute<DisplayAttribute>();
            return attr?.Name ?? value.ToString();
        }
    }
}
