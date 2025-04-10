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
            using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                var existing = await _context.StoreOperations
                    .Include(so => so.OpeningHours)
                    .FirstOrDefaultAsync(so => so.VendorId == operations.VendorId, cancellationToken);

                if (existing == null)
                {
                    // Insert new record
                    await _context.StoreOperations.AddAsync(operations, cancellationToken);
                }
                else
                {
                    // Update existing record
                    _context.Entry(existing).CurrentValues.SetValues(operations);

                    // Remove deleted opening hours
                    foreach (var existingHour in existing.OpeningHours.ToList())
                    {
                        if (!operations.OpeningHours.Any(oh => oh.Id == existingHour.Id))
                        {
                            _context.OpeningHours.Remove(existingHour);
                        }
                    }

                    // Update or add opening hours
                    foreach (var hour in operations.OpeningHours)
                    {
                        var existingHour = existing.OpeningHours
                            .FirstOrDefault(oh => oh.Id == hour.Id);

                        if (existingHour != null)
                        {
                            _context.Entry(existingHour).CurrentValues.SetValues(hour);
                        }
                        else
                        {
                            hour.StoreOperationId = existing.Id;
                            await _context.OpeningHours.AddAsync(hour, cancellationToken);
                        }
                    }
                }

                await _context.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);
                _logger.LogError(ex, "Error upserting store operations for vendor {VendorId}", operations.VendorId);
                throw;
            }
        }

    }
}