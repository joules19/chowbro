using Chowbro.Core.Entities.Vendor;
using Chowbro.Core.Repository.Interfaces.Vendor;
using Chowbro.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Chowbro.Infrastructure.Persistence.Repository.Vendor
{
    public class StoreOperationRepository : IStoreOperationRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<StoreOperationRepository> _logger;

        public StoreOperationRepository(
            AppDbContext context,
            ILogger<StoreOperationRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<StoreOperation?> GetByVendorIdAsync(Guid vendorId, CancellationToken cancellationToken = default)
        {
            try
            {
                return await _context.StoreOperations
                    .Include(so => so.OpeningHours)
                    .FirstOrDefaultAsync(so => so.VendorId == vendorId, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving store operations for vendor {VendorId}", vendorId);
                throw;
            }
        }

        public async Task UpsertAsync(StoreOperation operations, CancellationToken cancellationToken)
        {
            try
            {
                var existingOperation = await _context.StoreOperations
                    .Include(so => so.OpeningHours)
                    .FirstOrDefaultAsync(so => so.VendorId == operations.VendorId, cancellationToken);

                if (existingOperation == null)
                {
                    await _context.StoreOperations.AddAsync(operations, cancellationToken);
                }
                else
                {
                    existingOperation.DeliveryType = operations.DeliveryType;
                    existingOperation.OrderCutoffTime = operations.OrderCutoffTime;
                    existingOperation.MenuReadyTime = operations.MenuReadyTime;

                    var existingHours = existingOperation.OpeningHours.ToList();

                    foreach (var incomingHour in operations.OpeningHours)
                    {
                        var existingHour = existingHours.FirstOrDefault(oh => oh.Day == incomingHour.Day);

                        if (existingHour != null)
                        {
                            existingHour.OpenTime = incomingHour.OpenTime;
                            existingHour.CloseTime = incomingHour.CloseTime;
                            existingHour.IsClosed = incomingHour.IsClosed;
                            _context.Entry(existingHour).State = EntityState.Modified;
                        }
                        else
                        {
                            var newHour = new OpeningHours
                            {
                                Day = incomingHour.Day,
                                OpenTime = incomingHour.OpenTime,
                                CloseTime = incomingHour.CloseTime,
                                IsClosed = incomingHour.IsClosed,
                                StoreOperationId = existingOperation.Id
                            };
                            _context.OpeningHours.Add(newHour);
                        }
                    }

                    // Remove days not present in the incoming data
                    var incomingDays = operations.OpeningHours.Select(oh => oh.Day).ToList();
                    var hoursToRemove = existingHours.Where(oh => !incomingDays.Contains(oh.Day)).ToList();

                    if (hoursToRemove.Any())
                    {
                        _context.OpeningHours.RemoveRange(hoursToRemove);
                    }
                }

                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error upserting store operations for vendor {VendorId}", operations.VendorId);
                throw;
            }
        }
    }
}