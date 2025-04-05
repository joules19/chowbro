using Chowbro.Core.Events.Customer.Handlers;
using Microsoft.Extensions.DependencyInjection;

namespace Chowbro.Modules.Customers;

public static class CustomerModule
{
    public static IServiceCollection AddCustomerModule(this IServiceCollection services)
    {
        services.AddMediatR(cfg => 
        {
            cfg.RegisterServicesFromAssembly(typeof(CustomerRegisteredEventHandler).Assembly);
        });
        

        return services;
    }
}