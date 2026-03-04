using BusinessLogic.Interfaces;
using BusinessLogic.Interfaces.Entities;
using BusinessLogic.Models.Users;
using BusinessLogic.Services;
using Microsoft.Extensions.DependencyInjection;
using YourNamespace.Services;

namespace BusinessLogic
{
    public static class startup
    {
        public static IServiceCollection AddBusinessLogic(this IServiceCollection services)
        {
            return services.AddServices();
        }
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            //services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IDropDownService, DropDownService>();
            //services.AddScoped<ICardService, CardService>();
            return services;
        }
    }
}
