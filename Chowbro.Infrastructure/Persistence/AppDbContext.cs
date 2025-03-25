using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Chowbro.Infrastructure.Auth;
using Chowbro.Infrastructure.Entities.Vendor;

namespace Chowbro.Infrastructure
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        
        // Vendors
        public DbSet<Vendor> Vendors { get; set; }

        // Branches
        public DbSet<Branch> Branches { get; set; }
    }
}