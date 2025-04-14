using System.Collections.Concurrent;
using System.Threading.RateLimiting;

namespace AppointmentsWeb.Services;


/// <summary>
/// Servicio que almacena en memoria un diccionario de Rate Limiters.
/// </summary>
public class MultiRateLimiterService : IMultiRateLimiterService
{
    private static readonly ConcurrentDictionary<string, RateLimiter> _limiters = new();


    /// <summary>
    /// Retorna el Rate Limiter asociado a una 'key'. Si el Rate Limiter no existía lo crea primero.
    /// </summary>
    /// <param name="key">Identificador con el que se almacena el Rate Limiter.</param>
    /// <param name="factory">Función que crea el Rate Limiter.</param>
    public RateLimiter GetOrCreateLimiter(string key, Func<RateLimiter> factory)
    {
        return _limiters.GetOrAdd(key, _ => factory());
    }
}


// Add extension method for easy registration
public static class MultiRateLimiterServiceExtensions
{
    /// <summary>
    /// Agrega el servicio de MultiRateLimiter con sus opciones.
    /// </summary>
    /// <param name="configureOptions">Un delegate con el que configurar las MultiRateLimiterOptions.</param>
    public static IServiceCollection AddMultipleRateLimiter(
        this IServiceCollection services,
        Action<MultiRateLimiterOptions> configureOptions)
    {
        services.Configure(configureOptions);
        services.AddSingleton<IMultiRateLimiterService, MultiRateLimiterService>();
        return services;
    }
}
