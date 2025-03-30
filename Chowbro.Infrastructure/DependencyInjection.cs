using Chowbro.Core.Interfaces.Auth;
using Chowbro.Core.Interfaces.Media;
using Chowbro.Core.Interfaces.Vendors;
using Chowbro.Infrastructure.Persistence.Repositories;
using Chowbro.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Chowbro.Infrastructure
{
    public static class InfrastructureExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            // Database Repositories
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IProductCategoryRepository, ProductCategoryRepository>();
            services.AddScoped<IVendorRepository, VendorRepository>();

            // Services
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<IVendorProvider, VendorProvider>();
            services.AddScoped<ICloudinaryService, CloudinaryService>();

            // Other infrastructure registrations...
            return services;
        }
    }
}