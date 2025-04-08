using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Chowbro.Core.Services;
using Chowbro.Core.Services.Interfaces.Auth;

namespace Chowbro.Api.Filters
{
    public class DeviceValidationFilter : IAsyncActionFilter
    {
        private readonly IDeviceValidationService _deviceValidationService;
        private readonly ILogger<DeviceValidationFilter> _logger;
        private const string DeviceIdHeaderName = "X-Device-Id";

        public DeviceValidationFilter(
            IDeviceValidationService deviceValidationService,
            ILogger<DeviceValidationFilter> logger)
        {
            _deviceValidationService = deviceValidationService;
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
            if (!await _deviceValidationService.IsValidDeviceAsync(deviceId))
            {
                _logger.LogWarning("Invalid device ID: {DeviceId}", deviceId);
                context.Result = new UnauthorizedObjectResult(new
                {
                    Message = "Invalid device ID",
                    Code = "INVALID_DEVICE_ID"
                });
                return;
            }
            
            // if (await _deviceRepository.GetLoginCountToday(deviceId) > 20)
            // {
            //     await _deviceRepository.FlagForReview(deviceId);
            //     _logger.LogWarning($"Suspicious activity from device {deviceId}");
            // }
            
            await _deviceValidationService.UpdateLastSeenAsync(deviceId);

            // Store validated device ID for use in controllers
            context.HttpContext.Items["ValidatedDeviceId"] = deviceId;
            await next();
        }
        
        [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
        public class SkipDeviceValidationAttribute : Attribute { }
        
        [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
        public class RequireDeviceValidationAttribute : Attribute { }
    }
}