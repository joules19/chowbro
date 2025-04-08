using Chowbro.Core.Models.Settings;
using Chowbro.Core.Repository.Interfaces.Customer;
using Chowbro.Core.Repository.Interfaces.Location;
using Chowbro.Core.Repository.Interfaces.Product;
using Chowbro.Core.Repository.Interfaces.Rider;
using Chowbro.Core.Repository.Interfaces.Vendor;
using Chowbro.Core.Services.Interfaces.Auth;
using Chowbro.Core.Services.Interfaces.Media;
using Chowbro.Core.Services.Interfaces.Notifications;
using Chowbro.Core.Services.Interfaces.Vendor;
using Chowbro.Infrastructure.Persistence.Repositories;
using Chowbro.Infrastructure.Persistence.Repositories.Location;
using Chowbro.Infrastructure.Persistence.Repository.Auth;
using Chowbro.Infrastructure.Persistence.Repository.Customer;
using Chowbro.Infrastructure.Persistence.Repository.Product;
using Chowbro.Infrastructure.Persistence.Repository.Vendor;
using Chowbro.Infrastructure.Services;
using Chowbro.Infrastructure.Services.Auth;
using Chowbro.Infrastructure.Services.BackgroundServices;
using Chowbro.Infrastructure.Services.Media;
using Chowbro.Infrastructure.Services.Notifications;
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
            services.AddScoped<IBusinessTypeRepository, BusinessTypeRepository>();
            services.AddScoped<IRiderRepository, RiderRepository>();
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<IDeviceRepository, DeviceRepository>();

            services.AddScoped<IStateRepository, StateRepository>();
            services.AddScoped<ILgaRepository, LgaRepository>();

            // Services
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IOtpService, OtpService>();
            services.AddScoped<IDeviceValidationService, DeviceValidationService>();


            services.Configure<VendorApprovalOptions>(configuration.GetSection("VendorApproval"));

            // Register the background service
            // services.AddHostedService<VendorApprovalService>();

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