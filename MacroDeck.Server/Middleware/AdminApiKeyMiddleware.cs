using MacroDeck.Server.Services;
using Microsoft.AspNetCore.Http;

namespace MacroDeck.Server.Middleware;

/// <summary>
/// Enforces X-MacroDeck-Admin-Key header on all /api/* requests.
/// </summary>
public class AdminApiKeyMiddleware
{
    private const string ApiKeyHeader = "X-MacroDeck-Admin-Key";
    private readonly RequestDelegate _next;

    public AdminApiKeyMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IConfigAdminService config)
    {
        if (context.Request.Path.StartsWithSegments("/api"))
        {
            if (!context.Request.Headers.TryGetValue(ApiKeyHeader, out var providedKey)
                || !string.Equals(providedKey, config.GetAdminApiKey(), StringComparison.Ordinal))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsJsonAsync(new { error = "Invalid or missing admin API key." });
                return;
            }
        }

        await _next(context);
    }
}
