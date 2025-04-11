using Chowbro.Core.Entities.Vendor;
using Chowbro.Core.Repository.Interfaces.Vendor;
using Microsoft.EntityFrameworkCore;

namespace Chowbro.Infrastructure.Persistence.Repository.Vendor;

public class BusinessTypeRepository : IBusinessTypeRepository
{
    private readonly AppDbContext _context;

    public BusinessTypeRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<BusinessType> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.BusinessTypes.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<IEnumerable<BusinessType>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.BusinessTypes.ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<BusinessType>> GetActiveAsync(CancellationToken cancellationToken = default)
    {
        return await _context.BusinessTypes
            .Where(bt => bt.IsActive == true)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(BusinessType businessType, CancellationToken cancellationToken = default)
    {
        await _context.BusinessTypes.AddAsync(businessType, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(BusinessType businessType, CancellationToken cancellationToken = default)
    {
        _context.BusinessTypes.Update(businessType);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var businessType = await GetByIdAsync(id, cancellationToken);
        if (businessType != null)
        {
            _context.BusinessTypes.Remove(businessType);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.BusinessTypes.AnyAsync(bt => bt.Id == id, cancellationToken);
    }
}
