using Chowbro.Modules.Accounts.Services;
using Chowbro.Modules.Accounts.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Chowbro.Modules.Accounts
{
    public static class AccountsModule
    {
        public static IServiceCollection AddAccountsModule(this IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUserService, UserService>();
            return services;
        }
    }
}