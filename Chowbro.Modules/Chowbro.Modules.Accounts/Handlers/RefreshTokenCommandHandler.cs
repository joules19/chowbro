using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Chowbro.Core.Entities;
using Chowbro.Core.Entities.Auth;
using Chowbro.Core.Models;
using Chowbro.Infrastructure.Persistence.Repository.Auth;
using Chowbro.Modules.Accounts.Commands.Auth;
using Chowbro.Modules.Accounts.DTOs;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Http;

namespace Chowbro.Modules.Accounts.Handlers
{
    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, ApiResponse<AuthResponse>>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IDeviceRepository _deviceRepository;
        private readonly ILogger<RefreshTokenCommandHandler> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public RefreshTokenCommandHandler(
            UserManager<ApplicationUser> userManager,
            IConfiguration configuration,
            IDeviceRepository deviceRepository,
            ILogger<RefreshTokenCommandHandler> logger,
            IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _configuration = configuration;
            _deviceRepository = deviceRepository;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ApiResponse<AuthResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.RefreshToken == request.RefreshToken);

            if (user == null || user.RefreshTokenExpiryTime < DateTime.UtcNow)
                return ApiResponse<AuthResponse>.Fail(null, "Invalid or expired refresh token.", statusCode: HttpStatusCode.Unauthorized);
            if (!string.IsNullOrEmpty(request.DeviceId))
            {
                try
                {
                    // Device validation is security-relevant but not critical
                    var device = await _deviceRepository.GetByDeviceIdAsync(request.DeviceId);

                    if (device == null || device.UserId != user.Id || device.IsBlacklisted)
                    {
                        _logger.LogWarning("Device validation failed for {DeviceId} user {UserId}",
                            request.DeviceId, user.Id);
                        // Continue without failing - just log the warning
                    }
                    else
                    {
                        device.LastSeen = DateTime.UtcNow;
                        await _deviceRepository.UpdateAsync(device);
                        await LogDeviceActivity(device, user.Id, request.DeviceId);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Device processing error during refresh for {DeviceId}", request.DeviceId);
                    // Continue the refresh flow despite device errors
                }
            }
            return await GenerateAuthResponse(user, request.DeviceId);
        }

        private async Task LogDeviceActivity(Device existingDevice, string userId, string deviceId)
        {
            var currentIp = _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString();
            var currentUserAgent = _httpContextAccessor.HttpContext?.Request?.Headers["User-Agent"].ToString();

            // Get the most recent activity for this device
            var lastActivity = await _deviceRepository.GetLastActivityAsync(deviceId);

            // Only log if:
            // 1. This is the first activity ever for this device, OR
            // 2. The IP address has changed, OR
            // 3. The User-Agent has changed
            if (lastActivity == null ||
                lastActivity.IpAddress != currentIp ||
                lastActivity.UserAgent != currentUserAgent)
            {
                await _deviceRepository.AddAssociationHistoryAsync(new DeviceAssociationHistory
                {
                    DeviceId = deviceId,
                    UserId = userId,
                    AssociatedAt = DateTime.UtcNow,
                    AssociationType = "DeviceRefresh",
                    IpAddress = currentIp,
                    UserAgent = currentUserAgent
                });

                _logger.LogInformation("Logged significant device activity for {DeviceId} (IP or UA change)", deviceId);
            }
        }


        private async Task<ApiResponse<AuthResponse>> GenerateAuthResponse(ApplicationUser user, string? deviceId)
        {
            var token = await GenerateJwtToken(user, deviceId);
            var refreshToken = GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _userManager.UpdateAsync(user);

            var roles = await _userManager.GetRolesAsync(user);

            return ApiResponse<AuthResponse>.Success(new AuthResponse
            {
                AccessToken = token,
                RefreshToken = refreshToken,
                Expiration = DateTime.UtcNow.AddMinutes(_configuration.GetValue<int>("JwtSettings:ExpiresInMinutes")),
                DeviceId = deviceId,
                User = new UserDto
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email!,
                    PhoneNumber = user.PhoneNumber!,
                    Roles = roles.ToList()
                }
            });
        }

        private async Task<string> GenerateJwtToken(ApplicationUser user, string? deviceId)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var key = Encoding.UTF8.GetBytes(jwtSettings["Secret"]!);
            var roles = await _userManager.GetRolesAsync(user);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email!),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.PhoneNumber!),
                new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            if (!string.IsNullOrEmpty(deviceId))
            {
                claims.Add(new Claim("device_id", deviceId));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(_configuration.GetValue<int>("JwtSettings:ExpiresInMinutes")),
                Issuer = jwtSettings["Issuer"],
                Audience = jwtSettings["Audience"],
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256)
            };

            return new JwtSecurityTokenHandler().WriteToken(
                new JwtSecurityTokenHandler().CreateToken(tokenDescriptor));
        }

        private static string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }
}