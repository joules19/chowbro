using Chowbro.Core.Entities;
using Chowbro.Infrastructure.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Chowbro.Modules.Accounts.Data.Seeders
{
    public class SuperAdminSeeder
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<SuperAdminSeeder> _logger;
        private const string SuperAdminEmail = "superadmin@chowbro.com";
        private const string SuperAdminPassword = "Super@Admin123";

        public SuperAdminSeeder(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ILogger<SuperAdminSeeder> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        public async Task SeedAsync()
        {
            try
            {
                // Ensure the SuperAdmin role exists
                if (!await _roleManager.RoleExistsAsync(Roles.SuperAdmin))
                {
                    await _roleManager.CreateAsync(new IdentityRole(Roles.SuperAdmin));
                    _logger.LogInformation("Created {Role} role", Roles.SuperAdmin);
                }

                // Check if super admin already exists
                var superAdmin = await _userManager.FindByEmailAsync(SuperAdminEmail);
                if (superAdmin != null)
                {
                    _logger.LogInformation("SuperAdmin already exists");
                    return;
                }

                // Create super admin user
                superAdmin = new ApplicationUser
                {
                    UserName = SuperAdminEmail,
                    Email = SuperAdminEmail,
                    FirstName = "Super",
                    LastName = "Admin",
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true
                };

                var result = await _userManager.CreateAsync(superAdmin, SuperAdminPassword);
                if (!result.Succeeded)
                {
                    throw new Exception($"Failed to create super admin: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }

                // Assign SuperAdmin role
                result = await _userManager.AddToRoleAsync(superAdmin, Roles.SuperAdmin);
                if (!result.Succeeded)
                {
                    throw new Exception($"Failed to assign role to super admin: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }

                _logger.LogInformation("Successfully created SuperAdmin user");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while seeding the SuperAdmin");
                throw;
            }
        }
    }
}