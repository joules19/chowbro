namespace Chowbro.Api.Middlewares;

public class DeviceInfoMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<DeviceInfoMiddleware> _logger;

    public DeviceInfoMiddleware(RequestDelegate next, ILogger<DeviceInfoMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            // Always ensure DeviceId exists in context (empty if not provided)
            context.Items["DeviceId"] = context.Request.Headers.TryGetValue("X-Device-Id", out var deviceId)
                ? deviceId.ToString()
                : string.Empty;

            // Optional device information
            context.Items["DeviceName"] = context.Request.Headers.TryGetValue("X-Device-Name", out var deviceName)
                ? deviceName.ToString()
                : "Unknown";

            context.Items["DeviceModel"] = context.Request.Headers.TryGetValue("X-Device-Model", out var deviceModel)
                ? deviceModel.ToString()
                : "Unknown";

            // Log missing device ID (warning level for tracking)
            if (string.IsNullOrEmpty(context.Items["DeviceId"]?.ToString()))
            {
                _logger.LogWarning("Request is missing X-Device-Id header from IP: {RemoteIp}",
                    context.Connection.RemoteIpAddress);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing device headers");
            // Ensure default values are set even if processing fails
            context.Items["DeviceId"] = string.Empty;
            context.Items["DeviceName"] = "Unknown";
            context.Items["DeviceModel"] = "Unknown";
        }

        await _next(context);
    }
}