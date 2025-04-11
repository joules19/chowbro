using Chowbro.Core.Entities.Vendor;
using Chowbro.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Chowbro.Modules.Vendors.Data;

public static class BusinessTypeSeeder
    {
        public static async Task SeedAsync(AppDbContext context)
        {
            if (await context.BusinessTypes.AnyAsync())
                return;

            var businessTypes = new List<BusinessType>
            {
                new BusinessType { Name = "Restaurant", Description = "Food service establishment", IsActive = true },
                new BusinessType { Name = "Grocery", Description = "Food retail store", IsActive = true },
                new BusinessType { Name = "Pharmacy", Description = "Medical retail store", IsActive = true },
                new BusinessType { Name = "Supermarket", Description = "Supermarket", IsActive = true },
            };

            await context.BusinessTypes.AddRangeAsync(businessTypes);
            await context.SaveChangesAsync();
        }
    }
