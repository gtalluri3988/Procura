using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using YourNamespace.Services;

namespace BusinessLogic.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

        public string? GetUserId() => User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        public string? GetUsername() => User?.FindFirst(ClaimTypes.Name)?.Value;

        public string? GetEmail() => User?.FindFirst(ClaimTypes.Email)?.Value;

        public IEnumerable<Claim> GetUserClaims() => User?.Claims ?? Enumerable.Empty<Claim>();
    }
}
