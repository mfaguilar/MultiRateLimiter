using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.RateLimiting;
using AppointmentsWeb.Services;
using AppointmentsWeb.Controllers.Filters;

namespace AppointmentsWeb.Middlewares;

/// <summary>
/// Maneja las políticas de Rate Limit aplicadas a un endpoint usando el EnableMultiRateLimitingAttribute.
/// </summary>
public class MultiRateLimitingMiddleware(
        RequestDelegate next,
        ILogger<MultiRateLimitingMiddleware> logger,
        IOptions<MultiRateLimiterOptions> options,
        IMultiRateLimiterService rateLimiterService)
{

    private readonly MultiRateLimiterOptions _options = options.Value;


    public async Task InvokeAsync(HttpContext context)
    {
        var endpoint = context.GetEndpoint();
        var attribute = endpoint?.Metadata.GetMetadata<EnableMultiRateLimitingAttribute>();

        // Si el endpoint no tiene un EnableMultiRateLimitingAttribute, simplemente se termina la ejecución de este middleware.
        if (attribute == null)
        {
            await next(context);
            return;
        }

        foreach (var policyName in attribute.PolicyNames)
        {
            if (!this._options.Policies.TryGetValue(policyName, out var configurePolicy))
            {
                logger.LogWarning($"La política '{policyName}' no se encontró. En el endpoint: '{endpoint.DisplayName}'.");
                continue;
            }

            var clientIp = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var key = $"{clientIp}_{context.Request.Path}_{policyName}";

            var partition = configurePolicy(context);
            var limiter = rateLimiterService.GetOrCreateLimiter(key,
                () => partition.Factory(partition.PartitionKey));

            using var lease = await limiter.AcquireAsync();

            if (!lease.IsAcquired)
            {
                if (this._options.OnRejected != null)
                {
                    await this._options.OnRejected(
                        new OnRejectedContext
                        {
                            HttpContext = context,
                            Lease = lease
                        }, 
                        CancellationToken.None);
                }
                else
                {
                    logger.LogInformation($"Se llegó al límite de request según la política {key}");
                    context.Response.StatusCode = this._options.RejectionStatusCode;
                    await context.Response.WriteAsync(Resources.Exceptions.TooManyRequests);
                }
                return;
            }
        }

        await next(context);
    }
}

// Add extension method for easy registration
public static class MultipleRateLimitingMiddlewareExtensions
{
    /// <summary>
    /// Agrega el MultiRateLimitingMiddleware.
    /// </summary>
    public static IApplicationBuilder UseMultipleRateLimiting(
        this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<MultiRateLimitingMiddleware>();
    }
}