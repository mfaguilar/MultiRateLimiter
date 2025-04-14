using System.Threading.RateLimiting;

namespace AppointmentsWeb.Services;

/// <summary>
/// Servicio que almacena en memoria un diccionario de Rate Limiters.
/// </summary>
public interface IMultiRateLimiterService
{
    /// <summary>
    /// Retorna el Rate Limiter asociado a una 'key'. Si el Rate Limiter no existía lo crea primero.
    /// </summary>
    /// <param name="key">Identificador con el que se almacena el Rate Limiter.</param>
    /// <param name="factory">Función que crea el Rate Limiter.</param>
    public RateLimiter GetOrCreateLimiter(string key, Func<RateLimiter> factory);
}