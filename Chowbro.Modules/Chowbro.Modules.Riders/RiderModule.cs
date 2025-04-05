using Chowbro.Core.Events.Rider.Handlers;
using Microsoft.Extensions.DependencyInjection;

namespace Chowbro.Modules.Riders;

public static class RiderModule
{
    public static IServiceCollection AddRiderModule(this IServiceCollection services)
    {
        services.AddMediatR(cfg => 
        {
            cfg.RegisterServicesFromAssembly(typeof(RiderRegisteredEventHandler).Assembly);
        });
        

        return services;
    }
}