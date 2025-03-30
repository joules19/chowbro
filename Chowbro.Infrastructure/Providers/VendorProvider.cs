using Chowbro.Core.Entities;
using Chowbro.Core.Interfaces.Vendors;
using Microsoft.AspNetCore.Identity;

namespace Chowbro.Infrastructure.Services
{
    public class VendorProvider : IVendorProvider
    {
        private readonly IVendorRepository _vendorRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public VendorProvider(
            IVendorRepository vendorRepository,
            UserManager<ApplicationUser> userManager)
        {
            _vendorRepository = vendorRepository;
            _userManager = userManager;
        }

        public async Task<Vendor?> GetVendorByUserIdAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return null;

            var vendor = await _vendorRepository.GetByIdAsync(Guid.Parse(userId));
            if (vendor == null) return null;

            return vendor;
        }

        public async Task<Guid?> GetVendorIdForUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return null;

            var vendor = await _vendorRepository.GetByIdAsync(Guid.Parse(userId));
            if (vendor == null) return null;
            return vendor.Id;
        }
    }
}