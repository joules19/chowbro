namespace Chowbro.Api.Middlewares;

public class DeviceInfoMiddleware
{
    private readonly RequestDelegate _next;

    public DeviceInfoMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Headers.TryGetValue("X-Device-Id", out var deviceId))
        {
            context.Items["DeviceId"] = deviceId;
        }

        if (context.Request.Headers.TryGetValue("X-Device-Name", out var deviceName))
        {
            context.Items["DeviceName"] = deviceName;
        }

        if (context.Request.Headers.TryGetValue("X-Device-Model", out var deviceModel))
        {
            context.Items["DeviceModel"] = deviceModel;
        }

        await _next(context);
    }
}