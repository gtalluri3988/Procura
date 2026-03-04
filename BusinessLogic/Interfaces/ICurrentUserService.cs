using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace YourNamespace.Services
{
    public interface ICurrentUserService
    {
        string? GetUserId();
        string? GetUsername();
        string? GetEmail();
        IEnumerable<Claim> GetUserClaims();
    }

   
}