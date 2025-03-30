using Microsoft.Extensions.DependencyInjection;

public static class VendorModule
{
    public static IServiceCollection AddVendorModule(this IServiceCollection services)
    {
        // Only register module-specific dependencies
        // Register MediatR with the assembly containing your handlers
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(AddProductCommandHandler).Assembly));
        services.AddAutoMapper(typeof(ProductProfile));

        // Module-specific services
        // services.AddScoped<IProductService, ProductService>();

        return services;
    }
}