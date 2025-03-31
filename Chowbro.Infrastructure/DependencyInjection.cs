using Chowbro.Core.Interfaces.Auth;
using Chowbro.Core.Interfaces.Location;
using Chowbro.Core.Interfaces.Media;
using Chowbro.Core.Interfaces.Notifications;
using Chowbro.Core.Interfaces.Vendors;
using Chowbro.Core.Models.Settings;
using Chowbro.Infrastructure.Persistence.Repositories;
using Chowbro.Infrastructure.Persistence.Repositories.Location;
using Chowbro.Infrastructure.Services;
using Chowbro.Infrastructure.Settings;
using CloudinaryDotNet;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Chowbro.Infrastructure
{
    public static class InfrastructureExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // Database Repositories
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IProductCategoryRepository, ProductCategoryRepository>();
            services.AddScoped<IVendorRepository, VendorRepository>();
            services.AddScoped<IStateRepository, StateRepository>();
            services.AddScoped<ILgaRepository, LgaRepository>();

            // Services
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IOtpService, OtpService>();

            services.Configure<VendorApprovalOptions>(configuration.GetSection("VendorApproval"));

            // Register the background service
            services.AddHostedService<VendorApprovalService>();

            services.Configure<CloudinarySettings>(configuration.GetSection("CloudinarySettings"));
            services.AddSingleton(provider =>
            {
                var settings = provider.GetRequiredService<IOptions<CloudinarySettings>>().Value;
                return new Cloudinary(new Account(
                    settings.CloudName,
                    settings.ApiKey,
                    settings.ApiSecret));
            });

            services.AddScoped<IVendorProvider, VendorProvider>();
            services.AddScoped<ICloudinaryService, CloudinaryService>();

            // Other infrastructure registrations...
            return services;
        }
    }
}