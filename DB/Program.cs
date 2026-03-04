
using DB.EFModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using DB.Profiles;
using DB.Model;
using DB.Repositories.Interfaces;
using DB.Repositories;

namespace DB
{
    public static class Program
    {
        public static IServiceCollection AddDBProject(this IServiceCollection services, IConfiguration configuration)
        {
            return services.AddRepository().AddDbContext<ProcuraDbContext>(option => option.UseSqlServer(configuration.GetConnectionString("ProcuraConnection")));
           
        }
        public static IServiceCollection AddRepository(this IServiceCollection services)
        {
           
            services.AddAutoMapper(typeof(PasswordPolicyProfile));
            services.AddAutoMapper(typeof(UserProfile));
            services.AddAutoMapper(typeof(RoleProfile));
            services.AddAutoMapper(typeof(RoleMenuPermission));
            services.AddAutoMapper(typeof(MenuProfile));
            services.AddAutoMapper(typeof(ContentProfile));
            services.AddAutoMapper(typeof(PaymentProfile));
            services.AddAutoMapper(typeof(EventProfile));
            services.AddAutoMapper(typeof(PaymentRequestProfile));
            services.AddAutoMapper(typeof(PaymentResponseProfile));
            services.AddAutoMapper(typeof(VendorProfile));
            services.AddAutoMapper(typeof(CompanyCategoryProfile));
            services.AddScoped<IUserRepository, UserRepository>();      
            services.AddScoped<IDropdownRepository, DropdownRepository>();          
            services.AddScoped<IPasswordPolicyRepository, PasswordPolicyRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IRolePermissionRepository, RolePermissionRepository>();
            services.AddScoped<IMenuRepository, MenuRepository>();
            services.AddScoped<IContentRepository, ContentRepository>();          
            services.AddScoped<IPaymentRepository, PaymentRepository>();          
            services.AddScoped<INotificationRepository, NotificationRepository>();
            services.AddScoped<IVendorRepository, VendorRepository>();

            return services;
        }
    }
}
