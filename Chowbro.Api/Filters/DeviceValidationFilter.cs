using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Chowbro.Core.Services.Interfaces.Auth;
using Chowbro.Infrastructure.Persistence.Repository.Auth;

namespace Chowbro.Api.Filters
{
    public class DeviceValidationFilter : IAsyncActionFilter
    {
        private readonly IDeviceValidationService _deviceValidationService;
        private readonly IDeviceRepository _deviceRepository;
        private readonly ILogger<DeviceValidationFilter> _logger;
        private const string DeviceIdHeaderName = "X-Device-Id";
        private const int MaxLoginAttempts = 20;

        public DeviceValidationFilter(
            IDeviceValidationService deviceValidationService,
            IDeviceRepository deviceRepository,
            ILogger<DeviceValidationFilter> logger)
        {
            _deviceValidationService = deviceValidationService;
            _deviceRepository = deviceRepository;
            _logger = logger;
        }

        public async Task OnActionExecutionAsync(
            ActionExecutingContext context,
            ActionExecutionDelegate next)
        {
            // Skip validation for endpoints with [SkipDeviceValidation] attribute
            if (context.ActionDescriptor.EndpointMetadata.Any(em => em is SkipDeviceValidationAttribute))
            {
                await next();
                return;
            }

            // Get device ID if available (no longer mandatory)
            var deviceId = context.HttpContext.Request.Headers.TryGetValue(DeviceIdHeaderName, out var deviceIdHeader)
                ? deviceIdHeader.ToString()
                : null;

            if (string.IsNullOrEmpty(deviceId))
            {
                _logger.LogWarning("Request missing device ID header");
                // Continue without failing - just add warning to context
                context.HttpContext.Items["DeviceWarning"] = "Missing device ID";
            }
            else
            {
                try
                {
                    // Check device status but don't fail the request
                    var deviceStatus = await ValidateDevice(deviceId);
                    context.HttpContext.Items["DeviceStatus"] = deviceStatus;

                    if (deviceStatus.IsBlacklisted)
                    {
                        _logger.LogWarning("Blacklisted device access: {DeviceId}", deviceId);
                        context.HttpContext.Items["DeviceWarning"] = "Blacklisted device";
                    }

                    // Check for suspicious activity
                    if (deviceStatus.LoginCountToday > MaxLoginAttempts)
                    {
                        await _deviceRepository.FlagForReview(deviceId);
                        _logger.LogWarning($"Suspicious activity from device {deviceId}");
                        context.HttpContext.Items["DeviceWarning"] = "Suspicious activity detected";
                    }

                    // Update last seen for valid devices
                    if (!deviceStatus.IsBlacklisted)
                    {
                        await _deviceValidationService.UpdateLastSeenAsync(deviceId);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Device validation error for {DeviceId}", deviceId);
                    // Continue processing despite device validation errors
                }
            }

            // Store device ID for use in controllers
            context.HttpContext.Items["DeviceId"] = deviceId;

            // Only record login attempts for authentication actions with valid device IDs
            var isAuthenticationAction = context.ActionDescriptor.EndpointMetadata
                .Any(em => em is AuthenticationActionAttribute);

            if (isAuthenticationAction && !string.IsNullOrEmpty(deviceId))
            {
                try
                {
                    await _deviceRepository.RecordLoginAttempt(
                        deviceId,
                        context.HttpContext.Connection.RemoteIpAddress?.ToString());
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to record login attempt for {DeviceId}", deviceId);
                }
            }

            await next();
        }

        private async Task<DeviceStatus> ValidateDevice(string deviceId)
        {
            var device = await _deviceRepository.GetByDeviceIdAsync(deviceId);
            var loginCount = await _deviceRepository.GetLoginCountToday(deviceId);

            return new DeviceStatus
            {
                Exists = device != null,
                IsBlacklisted = device?.IsBlacklisted ?? false,
                LoginCountToday = loginCount,
                UserId = device?.UserId
            };
        }

        private class DeviceStatus
        {
            public bool Exists { get; set; }
            public bool IsBlacklisted { get; set; }
            public int LoginCountToday { get; set; }
            public string? UserId { get; set; }
        }

        [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
        public class SkipDeviceValidationAttribute : Attribute { }

        [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
        public class RequireDeviceValidationAttribute : Attribute { }

        [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
        public class AuthenticationActionAttribute : Attribute { }
    }
}