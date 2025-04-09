using System.Net;
using Chowbro.Core.Entities;
using Chowbro.Core.Entities.Auth;
using Chowbro.Core.Models;
using Chowbro.Core.Services.Interfaces.Notifications;
using Chowbro.Infrastructure.Auth;
using Chowbro.Infrastructure.Helpers;
using Chowbro.Infrastructure.Persistence.Repository.Auth;
using Chowbro.Modules.Accounts.Commands.Auth;
using Chowbro.Modules.Accounts.DTOs;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Chowbro.Modules.Accounts.Handlers
{
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, ApiResponse<OtpResponse>>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IOtpService _otpService;
        private readonly IDeviceRepository _deviceRepository;
        private readonly ILogger<RegisterCommandHandler> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public RegisterCommandHandler(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IOtpService otpService,
            IDeviceRepository deviceRepository,
            ILogger<RegisterCommandHandler> logger,
            IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _otpService = otpService;
            _deviceRepository = deviceRepository;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ApiResponse<OtpResponse>> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            var model = request.Model;

            if (string.IsNullOrWhiteSpace(model.DeviceId))
            {
                return ApiResponse<OtpResponse>.Fail(null, "Device ID is required", HttpStatusCode.BadRequest);
            }

            var existingUser = await _userManager.Users.FirstOrDefaultAsync(u => u.Email == model.Email || u.PhoneNumber == model.PhoneNumber);

            if (existingUser != null)
            {
                if (existingUser.EmailConfirmed || existingUser.PhoneNumberConfirmed)
                    return ApiResponse<OtpResponse>.Fail(null, "Email already in use. Please login.", HttpStatusCode.Conflict);

                return ApiResponse<OtpResponse>.Fail(null, "Email or phone number already in use. Please login.", HttpStatusCode.Conflict);
            }

            var existingDevice = await _deviceRepository.GetByDeviceIdAsync(model.DeviceId);
            if (existingDevice?.IsBlacklisted == true)
            {
                _logger.LogWarning("Blacklisted device attempted registration: {DeviceId}", model.DeviceId);
                return ApiResponse<OtpResponse>.Fail(null, "Device not allowed", HttpStatusCode.Forbidden);
            }

            // Create user
            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                FirstName = model.FirstName,
                LastName = model.LastName,
                ReferralCode = model.ReferralCode ?? string.Empty,
                DateOfBirth = model.DateOfBirth,
                Country = "NIGERIA"
            };

            user.PasswordHash = _userManager.PasswordHasher.HashPassword(user, model.Email);

            var result = await _userManager.CreateAsync(user);
            if (!result.Succeeded)
                return ApiResponse<OtpResponse>.Fail(null, "User registration failed.", HttpStatusCode.InternalServerError, result.Errors.Select(e => e.Description).ToList());

            // Handle device registration
            try
            {
                var ipAddress = _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString();
                var userAgent = _httpContextAccessor.HttpContext?.Request?.Headers["User-Agent"].ToString();

                if (existingDevice == null)
                {
                    var newDevice = new Device
                    {
                        DeviceId = model.DeviceId,
                        FirstSeen = DateTime.UtcNow,
                        LastSeen = DateTime.UtcNow,
                        IsActive = true,
                        UserId = user.Id,
                        AssociationCount = 1,
                        LastAssociatedDate = DateTime.UtcNow
                    };
                    await _deviceRepository.AddAsync(newDevice);
                }
                else
                {
                    existingDevice.UserId = user.Id;
                    existingDevice.LastSeen = DateTime.UtcNow;
                    existingDevice.AssociationCount++;
                    existingDevice.LastAssociatedDate = DateTime.UtcNow;
                    await _deviceRepository.UpdateAsync(existingDevice);
                }

                // Log association history for both cases
                await _deviceRepository.AddAssociationHistoryAsync(new DeviceAssociationHistory
                {
                    DeviceId = model.DeviceId,
                    UserId = user.Id,
                    AssociatedAt = DateTime.UtcNow,
                    AssociationType = "Register",
                    IpAddress = ipAddress,
                    UserAgent = userAgent
                });

                if (existingDevice?.AssociationCount > 3)
                {
                    _logger.LogWarning("Device {DeviceId} has been associated with {Count} accounts",
                        model.DeviceId, existingDevice.AssociationCount);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while handling device registration for {DeviceId}", model.DeviceId);
            }

            // Assign role and generate OTP
            var userRole = string.IsNullOrEmpty(model.Role) ? Roles.Customer : model.Role;
            if (await _roleManager.RoleExistsAsync(userRole))
            {
                await _userManager.AddToRoleAsync(user, userRole);
            }

            var otp = OtpHelper.GenerateOtp(4);
            user.OtpCode = otp;
            user.OtpExpires = DateTime.UtcNow.AddMinutes(10);
            await _userManager.UpdateAsync(user);

            return ApiResponse<OtpResponse>.Success(new OtpResponse
            {
                RequiresOtp = true,
                Message = "An OTP has been sent to your email.",
                DeviceId = model.DeviceId
            }, statusCode: HttpStatusCode.Created);
        }
    }
}