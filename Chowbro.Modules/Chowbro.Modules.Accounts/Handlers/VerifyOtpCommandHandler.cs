using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Chowbro.Core.Entities;
using Chowbro.Core.Entities.Auth;
using Chowbro.Core.Events;
using Chowbro.Core.Events.Customer;
using Chowbro.Core.Events.Rider;
using Chowbro.Core.Events.Vendor;
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
    public class VerifyOtpCommandHandler : IRequestHandler<VerifyOtpCommand, ApiResponse<AuthResponse>>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IMediator _mediator;
        private readonly IDeviceRepository _deviceRepository;
        private readonly ILogger<VerifyOtpCommandHandler> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public VerifyOtpCommandHandler(
            UserManager<ApplicationUser> userManager,
            IConfiguration configuration,
            IMediator mediator,
            IDeviceRepository deviceRepository,
            ILogger<VerifyOtpCommandHandler> logger,
            IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _configuration = configuration;
            _mediator = mediator;
            _deviceRepository = deviceRepository;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ApiResponse<AuthResponse>> Handle(VerifyOtpCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u =>
                u.Email == request.ContactInfo || u.PhoneNumber == request.ContactInfo);

            if (user == null)
                return ApiResponse<AuthResponse>.Fail(null, "User not found.", HttpStatusCode.NotFound);

            if (string.IsNullOrEmpty(user.OtpCode) ||
                user.OtpExpires == null ||
                user.OtpCode != request.Otp ||
                user.OtpExpires < DateTime.UtcNow)
                return ApiResponse<AuthResponse>.Fail(null, "Invalid or expired OTP.",
                    statusCode: HttpStatusCode.BadRequest);

            await _deviceRepository.RecordLoginAttempt(request.DeviceId, _httpContextAccessor.HttpContext.Connection.RemoteIpAddress?.ToString());

            if (string.IsNullOrEmpty(request.DeviceId))
                return ApiResponse<AuthResponse>.Fail(null, "Device ID is required.", HttpStatusCode.BadRequest);

            try
            {
                await ProcessDeviceAssociation(request.DeviceId, user.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Device association failed for {DeviceId}", request.DeviceId);
                return ApiResponse<AuthResponse>.Fail(null,
                    "Device processing failed. Please contact support.",
                    HttpStatusCode.InternalServerError);
            }

            // Clear OTP and confirm phone if needed
            user.OtpCode = null;
            user.OtpExpires = null;

            if (!user.PhoneNumberConfirmed)
            {
                user.PhoneNumberConfirmed = true;
                await PublishRegistrationEvent(user, cancellationToken);
            }

            await _userManager.UpdateAsync(user);
            return await GenerateJwtToken(user, request.DeviceId);
        }

        private async Task ProcessDeviceAssociation(string deviceId, string userId)
        {
            var device = await _deviceRepository.GetByDeviceIdAsync(deviceId);
            var now = DateTime.UtcNow;

            if (device == null)
            {
                device = new Device
                {
                    DeviceId = deviceId,
                    UserId = userId,
                    FirstSeen = now,
                    LastSeen = now,
                    IsActive = true,
                    AssociationCount = 1,
                    LastAssociatedDate = now
                };
                await _deviceRepository.AddAsync(device);
            }
            else
            {
                device.UserId = userId;
                device.LastSeen = now;
                device.AssociationCount++;
                device.LastAssociatedDate = now;
                await _deviceRepository.UpdateAsync(device);

                if (device.AssociationCount > 3)
                {
                    _logger.LogWarning("High device associations: {DeviceId} count {Count}",
                        deviceId, device.AssociationCount);
                }
            }

            await LogDeviceActivity(deviceId, userId, "Login");
        }

        private async Task LogDeviceActivity(string deviceId, string userId, string activityType)
        {
            var ipAddress = _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString();
            var userAgent = _httpContextAccessor.HttpContext?.Request?.Headers["User-Agent"].ToString();

            await _deviceRepository.AddAssociationHistoryAsync(new DeviceAssociationHistory
            {
                DeviceId = deviceId,
                UserId = userId,
                AssociatedAt = DateTime.UtcNow,
                AssociationType = activityType,
                IpAddress = ipAddress,
                UserAgent = userAgent
            });
        }

        private async Task PublishRegistrationEvent(ApplicationUser user, CancellationToken cancellationToken)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var userEvent = new UserRegisteredEvent
            {
                UserId = user.Id,
                Email = user.Email!,
                PhoneNumber = user.PhoneNumber!,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Roles = roles
            };

            if (roles.Contains("Vendor"))
            {
                await _mediator.Publish(new VendorRegisteredEvent
                {
                    UserId = userEvent.UserId,
                    Email = userEvent.Email!,
                    PhoneNumber = userEvent.PhoneNumber!,
                    FirstName = userEvent.FirstName,
                    LastName = userEvent.LastName,
                    Roles = userEvent.Roles
                }, cancellationToken);
            }
            else if (roles.Contains("Customer"))
            {
                await _mediator.Publish(new CustomerRegisteredEvent
                {
                    UserId = userEvent.UserId,
                    Email = userEvent.Email!,
                    PhoneNumber = userEvent.PhoneNumber!,
                    FirstName = userEvent.FirstName,
                    LastName = userEvent.LastName,
                    Roles = userEvent.Roles
                }, cancellationToken);
            }
            else if (roles.Contains("Rider"))
            {
                await _mediator.Publish(new RiderRegisteredEvent
                {
                    UserId = userEvent.UserId,
                    Email = userEvent.Email!,
                    PhoneNumber = userEvent.PhoneNumber!,
                    FirstName = userEvent.FirstName,
                    LastName = userEvent.LastName,
                    Roles = userEvent.Roles
                }, cancellationToken);
            }
        }

        private async Task<ApiResponse<AuthResponse>> GenerateJwtToken(ApplicationUser user, string deviceId)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var key = Encoding.UTF8.GetBytes(jwtSettings["Secret"]!);
            var expiresInMinutes = _configuration.GetValue<int>("JwtSettings:ExpiresInMinutes");

            var roles = await _userManager.GetRolesAsync(user);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email!),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.PhoneNumber!),
                new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
                new Claim(ClaimTypes.Role, string.Join(",", roles)),
                new Claim("device_id", deviceId),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(expiresInMinutes),
                Issuer = jwtSettings["Issuer"],
                Audience = jwtSettings["Audience"],
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            var refreshToken = GenerateRefreshToken();
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _userManager.UpdateAsync(user);

            return ApiResponse<AuthResponse>.Success(new AuthResponse
            {
                AccessToken = tokenHandler.WriteToken(token),
                RefreshToken = refreshToken,
                Expiration = token.ValidTo,
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
            }, statusCode: HttpStatusCode.OK);
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }
}