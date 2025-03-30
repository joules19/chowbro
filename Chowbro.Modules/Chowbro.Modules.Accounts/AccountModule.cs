using Chowbro.Core.Interfaces.Notifications;
using Chowbro.Infrastructure.Services;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Chowbro.Modules.Accounts
{
    public static class AccountsModule
    {
        public static IServiceCollection AddAccountsModule(this IServiceCollection services)
        {
            // Register MediatR and handlers from this assembly
            services.AddMediatR(cfg =>
                cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

            // Register validators
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            services.AddScoped<IOtpService, OtpService>(); 

            return services;
        }
    }
}