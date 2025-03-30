using Chowbro.Core.Interfaces.Auth;
using Chowbro.Core.Interfaces.Vendors;
using Chowbro.Core.Middlewares;
using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Chowbro.Infrastructure.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IVendorProvider _vendorProvider;

        public CurrentUserService(
            IHttpContextAccessor httpContextAccessor,
            IVendorProvider vendorProvider)
        {
            _httpContextAccessor = httpContextAccessor;
            _vendorProvider = vendorProvider;
        }

        public string? UserId => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier)
                         ?? _httpContextAccessor.HttpContext?.User?.FindFirstValue("uid");

        public string? Email => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Email);

        public string? PhoneNumber => _httpContextAccessor.HttpContext?.User?.FindFirstValue(JwtRegisteredClaimNames.UniqueName);

        public string? Name => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Name);

        public IEnumerable<string> Roles => _httpContextAccessor.HttpContext?.User?
            .FindAll(ClaimTypes.Role)
            .Select(c => c.Value) ?? Enumerable.Empty<string>();

        public bool IsAuthenticated => UserId != null;

        public bool IsVendor => Roles.Contains("Vendor");

        public async Task<Vendor?> GetVendorAsync()
        {
            if (!IsVendor || string.IsNullOrEmpty(UserId))
                return null;

            return await _vendorProvider.GetVendorByUserIdAsync(UserId);
        }

        public async Task<Guid?> GetVendorIdAsync()
        {
            if (!IsVendor || string.IsNullOrEmpty(UserId))
                return null;

            return await _vendorProvider.GetVendorIdForUserAsync(UserId);
        }

        public async Task<Guid> GetRequiredVendorIdAsync()
        {
            if (!IsVendor || string.IsNullOrEmpty(UserId))
                throw new VendorRequiredException();

            var vendorId = await _vendorProvider.GetVendorIdForUserAsync(UserId);
            return vendorId ?? throw new VendorRequiredException();
        }

      
    }
}