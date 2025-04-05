using System.Reflection;
using Chowbro.Core.Events.Vendor.Handlers;
using Chowbro.Modules.Vendors.Profiles;
using Microsoft.Extensions.DependencyInjection;

namespace Chowbro.Modules.Vendors;

public static class VendorModule
{
    public static IServiceCollection AddVendorModule(this IServiceCollection services)
    {
        
        services.AddMediatR(cfg => 
        {
            cfg.RegisterServicesFromAssembly(typeof(VendorRegisteredEventHandler).Assembly);
        });
        // services.AddMediatR(cfg =>
        //     cfg.RegisterServicesFromAssembly(typeof(VendorModule).Assembly));
        // services.AddMediatR(cfg =>
        //     cfg.RegisterServicesFromAssembly(typeof(AddProductCommandHandler).Assembly));
      
        // Mapping Profiles
        services.AddAutoMapper(typeof(ProductProfile));
        services.AddAutoMapper(typeof(VendorProfile).Assembly);

        return services;
    }
}