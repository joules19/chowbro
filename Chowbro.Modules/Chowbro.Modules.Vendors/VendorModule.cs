using Chowbro.Modules.Vendors.Profiles;
using Microsoft.Extensions.DependencyInjection;

public static class VendorModule
{
    public static IServiceCollection AddVendorModule(this IServiceCollection services)
    {
        // Register MediatR with the assembly containing your handlers
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(AddProductCommandHandler).Assembly));

        // Mapping Profiles
        services.AddAutoMapper(typeof(ProductProfile));
        services.AddAutoMapper(typeof(VendorProfile).Assembly);

        return services;
    }
}