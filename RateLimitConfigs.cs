using AppointmentsWeb.Services.AppSettings;
using System.Threading.RateLimiting;

namespace AppointmentsWeb.Utils;

/// <summary>
/// Brinda configuraciones para Políticas de Rate Limiters.
/// </summary>
public class RateLimitConfigs
{
    private string _remoteIpAddress;


    /// <summary>
    /// Obtiene una función que configura la Política para el RateLimiter, según la configuración indicada.
    /// </summary>
    /// <param name="windowSettings">Configuración.</param>
    /// <exception cref="NotImplementedException">Si la configuración pasada no está contemplada tira excepción.</exception>
    public Func<HttpContext, RateLimitPartition<string>> GetConfigPolicyFunc(ILimitWindow windowSettings)
    {
        return context =>
            windowSettings switch
            {
                LoginSettings => this.GetLoginLimit(context, this.GetFixedWindow(windowSettings)),
                RegisterSettings => this.GetLimit(context, this.GetFixedWindow(windowSettings)),
                PasswordResetSettings => this.GetPasswordResetLimit(context, this.GetFixedWindow(windowSettings)),
                PasswordResetGlobalSettings => this.GetPasswordResetGlobalLimit(this.GetFixedWindow(windowSettings)),
                _ => throw new NotImplementedException()
            };
    }


    private string GetRemoteIpAddress(HttpContext context) => 
        this._remoteIpAddress = this._remoteIpAddress ?? context.Connection.RemoteIpAddress?.ToString() ?? "unknown";


    private RateLimitPartition<string> GetLimit(HttpContext context, FixedWindowRateLimiterOptions options)
    {
        return RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: this.GetRemoteIpAddress(context), // Límite por IP
                    factory: _ => options);
    }


    private RateLimitPartition<string> GetLoginLimit(HttpContext context, FixedWindowRateLimiterOptions options)
    {
        string username = Request.GetUsernameFromLoginRequest(context);
        var ip = this.GetRemoteIpAddress(context);

        return RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: $"{ip}_{username}", // Límite por IP y username
                    factory: _ => options);
    }


    private RateLimitPartition<string> GetPasswordResetLimit(HttpContext context, FixedWindowRateLimiterOptions options)
    {
        var email = Request.GetEmailFromPasswordResetRequest(context);

        return RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: $"password-reset_{email}", // Límite por email
            factory: _ => options);
    }


    private RateLimitPartition<string> GetPasswordResetGlobalLimit(FixedWindowRateLimiterOptions options)
    {
        return RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: $"password-reset_global",
            factory: _ => options);
    }


    private FixedWindowRateLimiterOptions GetFixedWindow(ILimitWindow window)
    {
        return new FixedWindowRateLimiterOptions
        {
            PermitLimit = window.PermitLimit,
            Window = TimeSpan.FromMinutes(window.TimeWindow),
            AutoReplenishment = true
        };
    }
}
