using AutoMapper;
using Chowbro.Core.Interfaces.Vendors;
using Chowbro.Core.Models.Settings;
using Chowbro.Core.Models.Vendor;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using static Chowbro.Core.Enums.Vendor;

namespace Chowbro.Infrastructure.Services
{
    public class VendorApprovalService : BackgroundService
    {
        private readonly ILogger<VendorApprovalService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly TimeSpan _checkInterval = TimeSpan.FromHours(1);

        public VendorApprovalService(
            ILogger<VendorApprovalService> logger,
            IServiceProvider serviceProvider, 
            IOptions<VendorApprovalOptions> options)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _checkInterval = options.Value.CheckInterval;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var vendorRepository = scope.ServiceProvider.GetRequiredService<IVendorRepository>();
                    var mapper = scope.ServiceProvider.GetRequiredService<IMapper>();

                    var pendingVendors = await vendorRepository.GetPendingApprovalVendorsAsync();

                    foreach (var vendor in pendingVendors)
                    {
                        if (ShouldApproveVendor(vendor))
                        {
                            vendor.Status = VendorStatus.Active;
                            await vendorRepository.UpdateAsync(vendor);

                            var vendorDto = mapper.Map<VendorDto>(vendor);
                            _logger.LogInformation("Approved vendor {VendorId} - {VendorName}",
                                vendorDto.Id, vendorDto.Name);
                        }
                    }

                    await vendorRepository.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing vendor approvals");
                }

                await Task.Delay(_checkInterval, stoppingToken);
            }
        }

        private bool ShouldApproveVendor(Vendor vendor)
        {
            // Implement your approval logic here
            // Could check documents, verification status, etc.
            return true; // Simplified for example
        }
    }
}