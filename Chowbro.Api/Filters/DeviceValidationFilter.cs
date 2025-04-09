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

            // Validate device ID (always perform device validation)
            if (!context.HttpContext.Request.Headers.TryGetValue(DeviceIdHeaderName, out var deviceIdHeader))
            {
                _logger.LogWarning("Request missing device ID header");
                context.Result = new UnauthorizedObjectResult(new
                {
                    Message = "Device ID is required",
                    Code = "DEVICE_ID_REQUIRED"
                });
                return;
            }

            var deviceId = deviceIdHeader.ToString();

            // Check if device exists and is not blacklisted
            var isValidDevice = await _deviceValidationService.IsValidDeviceAsync(deviceId);
            if (!isValidDevice)
            {
                _logger.LogWarning("Invalid device ID: {DeviceId}", deviceId);
                context.Result = new UnauthorizedObjectResult(new
                {
                    Message = "Invalid device ID",
                    Code = "INVALID_DEVICE_ID"
                });
                return;
            }

            // Check for suspicious activity (e.g., too many login attempts)
            var loginCount = await _deviceRepository.GetLoginCountToday(deviceId);
            if (loginCount > MaxLoginAttempts)
            {
                await _deviceRepository.FlagForReview(deviceId);
                _logger.LogWarning($"Suspicious activity from device {deviceId} with {loginCount} attempts today");

                context.Result = new UnauthorizedObjectResult(new
                {
                    Message = "Account access temporarily restricted due to suspicious activity",
                    Code = "ACCOUNT_TEMPORARILY_LOCKED"
                });
                return;
            }

            // Update the device's last seen time
            await _deviceValidationService.UpdateLastSeenAsync(deviceId);

            // Store validated device ID for use in controllers
            context.HttpContext.Items["ValidatedDeviceId"] = deviceId;

            // Check if the action or controller is marked with [AuthenticationAction]
            var isAuthenticationAction = context.ActionDescriptor.EndpointMetadata
                .Any(em => em is AuthenticationActionAttribute);

            // Only record the login attempt if it's an authentication-related action
            if (isAuthenticationAction)
            {
                await _deviceRepository.RecordLoginAttempt(deviceId, context.HttpContext.Connection.RemoteIpAddress?.ToString());
            }

            await next();
        }

        [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
        public class SkipDeviceValidationAttribute : Attribute { }

        [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
        public class RequireDeviceValidationAttribute : Attribute { }

        [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
        public class AuthenticationActionAttribute : Attribute { }
    }
}
