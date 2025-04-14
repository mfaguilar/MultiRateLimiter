using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;

namespace AppointmentsWeb.Utils;


/// <summary>
/// Opciones para el MultiRateLimitingMiddleware.
/// </summary>
public class MultiRateLimiterOptions
{
    private readonly Dictionary<string, Func<HttpContext, RateLimitPartition<string>>> _policies = new();
    
    
    /// <summary>
    /// El Http Status Code a poner cuando se alcanzó el Rate Limit.
    /// </summary>
    public int RejectionStatusCode { get; set; } = StatusCodes.Status429TooManyRequests;
    
    /// <summary>
    /// Función a ejecutar cuando una request se rechace porque se alcanzó el Rate Limit.
    /// </summary>
    public Func<OnRejectedContext, CancellationToken, ValueTask>? OnRejected { get; set; }


    public IReadOnlyDictionary<string, Func<HttpContext, RateLimitPartition<string>>> Policies => this._policies;


    /// <summary>
    /// Agrega una política de Rate Limit.
    /// </summary>
    /// <param name="policyName">Nombre de la política.</param>
    /// <param name="configurePolicy">Configuración de la política.</param>
    public MultiRateLimiterOptions AddPolicy(string policyName, Func<HttpContext, RateLimitPartition<string>> configurePolicy)
    {
        this._policies[policyName] = configurePolicy;
        return this;
    }
}