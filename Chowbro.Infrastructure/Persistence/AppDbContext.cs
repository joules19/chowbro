using Chowbro.Core.Entities;
using Chowbro.Core.Entities.Auth;
using Chowbro.Core.Entities.Customer;
using Chowbro.Core.Entities.Location;
using Chowbro.Core.Entities.Product;
using Chowbro.Core.Entities.Rider;
using Chowbro.Core.Entities.Vendor;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Chowbro.Infrastructure.Persistence
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // Auth
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Device> Devices { get; set; }
        public DbSet<DeviceLoginAttempt> DeviceLoginAttempts { get; set; }


        // Customers
        public DbSet<Customer> Customers { get; set; }

        // Vendors
        public DbSet<Vendor> Vendors { get; set; }
        public DbSet<Branch> Branches { get; set; }
        public DbSet<BusinessType> BusinessTypes { get; set; }


        // Products
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductCategory> ProductCategories { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<ProductOption> ProductOptions { get; set; }
        public DbSet<ProductOptionCategory> ProductOptionCategories { get; set; }

        // Locations
        public DbSet<State> States { get; set; }
        public DbSet<Lga> Lgas { get; set; }

        // Riders
        public DbSet<Rider> Riders { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure relationships
            builder.Entity<Vendor>()
                .HasMany(v => v.Branches)
                .WithOne(b => b.Vendor)
                .HasForeignKey(b => b.VendorId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Vendor>()
                .HasMany(v => v.ProductCategories)
                .WithOne(pc => pc.Vendor)
                .HasForeignKey(pc => pc.VendorId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Product>()
                .HasMany(p => p.Images)
                .WithOne(pi => pi.Product)
                .HasForeignKey(pi => pi.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Product>()
                .HasMany(p => p.OptionCategories)
                .WithOne(oc => oc.Product)
                .HasForeignKey(oc => oc.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ProductOptionCategory>()
                .HasMany(oc => oc.Options)
                .WithOne(o => o.OptionCategory)
                .HasForeignKey(o => o.OptionCategoryId)
                .OnDelete(DeleteBehavior.Cascade);

            // State and LGA relationships
            builder.Entity<State>()
                .HasMany(s => s.Lgas)
                .WithOne(l => l.State)
                .HasForeignKey(l => l.StateId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Branch>()
                .HasOne(b => b.State)
                .WithMany()
                .HasForeignKey(b => b.StateId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Branch>()
                .HasOne(b => b.Lga)
                .WithMany()
                .HasForeignKey(b => b.LgaId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure enums
            builder.Entity<ProductOptionCategory>()
                .Property(oc => oc.SelectionType)
                .HasConversion<string>();

            // Configure indexes
            builder.Entity<Product>()
                .HasIndex(p => new { p.VendorId, p.Name });

            builder.Entity<Branch>()
                .HasIndex(b => b.VendorId);

            builder.Entity<Lga>()
                .HasIndex(l => l.StateId);

            builder.Entity<BusinessType>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
            });

            // Configure soft delete filter
            builder.Entity<Vendor>().HasQueryFilter(v => !v.IsDeleted);
            builder.Entity<Branch>().HasQueryFilter(b => !b.IsDeleted);
            builder.Entity<Product>().HasQueryFilter(p => !p.IsDeleted);
            builder.Entity<ProductCategory>().HasQueryFilter(pc => !pc.IsDeleted);
            builder.Entity<State>().HasQueryFilter(s => !s.IsDeleted);
            builder.Entity<Lga>().HasQueryFilter(l => !l.IsDeleted);
        }
    }
}