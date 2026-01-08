using DesafioTecnico_Ache.DTOs.Responses;
using System.Collections.Concurrent;
using System.Text.Json;

namespace DesafioTecnico_Ache.Middlewares;

/// <summary>
/// Middleware de Rate Limiting
/// Implementa proteção contra:
/// - DoS (Denial of Service) - OWASP
/// - Brute force attacks
/// - API abuse
/// 
/// Usa algoritmo de Token Bucket simplificado
/// Em produção, considerar:
/// - AspNetCoreRateLimit library
/// - Redis para rate limiting distribuído
/// - Rate limiting por API Key específico
/// </summary>
public class RateLimitingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RateLimitingMiddleware> _logger;
    private static readonly ConcurrentDictionary<string, ClientRateLimitInfo> _clients = new();
    
    // Configurações de rate limiting
    private const int MaxRequestsPerMinute = 100;
    private const int TimeWindowSeconds = 60;

    public RateLimitingMiddleware(
        RequestDelegate next,
        ILogger<RateLimitingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Não aplicar rate limiting ao Swagger
        if (context.Request.Path.StartsWithSegments("/swagger"))
        {
            await _next(context);
            return;
        }

        var clientId = GetClientIdentifier(context);
        var clientInfo = _clients.GetOrAdd(clientId, _ => new ClientRateLimitInfo());

        bool isLimitExceeded;
        int remainingRequests;
        DateTime resetTime;

        lock (clientInfo)
        {
            var now = DateTime.UtcNow;

            // Resetar contador se passou o time window
            if ((now - clientInfo.WindowStart).TotalSeconds >= TimeWindowSeconds)
            {
                clientInfo.RequestCount = 0;
                clientInfo.WindowStart = now;
            }

            clientInfo.RequestCount++;

            // Verificar se excedeu o limite
            isLimitExceeded = clientInfo.RequestCount > MaxRequestsPerMinute;
            remainingRequests = Math.Max(0, MaxRequestsPerMinute - clientInfo.RequestCount);
            resetTime = clientInfo.WindowStart.AddSeconds(TimeWindowSeconds);
        }

        // Headers de rate limiting (fora do lock)
        context.Response.Headers["X-RateLimit-Limit"] = MaxRequestsPerMinute.ToString();
        context.Response.Headers["X-RateLimit-Remaining"] = remainingRequests.ToString();
        context.Response.Headers["X-RateLimit-Reset"] = resetTime.ToString("yyyy-MM-ddTHH:mm:ssZ");

        if (isLimitExceeded)
        {
            _logger.LogWarning("Rate limit excedido. Cliente: {ClientId}", clientId);

            context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
            context.Response.ContentType = "application/json";

            var errorResponse = new ApiErrorResponse
            {
                ErrorCode = "RATE_001",
                Message = $"Limite de requisições excedido. Máximo: {MaxRequestsPerMinute} por minuto",
                Path = context.Request.Path
            };

            var json = JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await context.Response.WriteAsync(json);
            return;
        }

        await _next(context);
    }

    private string GetClientIdentifier(HttpContext context)
    {
        // Priorizar API Key se disponível
        if (context.Request.Headers.TryGetValue("X-API-Key", out var apiKey))
        {
            return $"apikey_{apiKey}";
        }

        // Fallback para IP address
        var ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        return $"ip_{ipAddress}";
    }
}

/// <summary>
/// Informações de rate limiting por cliente
/// </summary>
public class ClientRateLimitInfo
{
    public int RequestCount { get; set; }
    public DateTime WindowStart { get; set; } = DateTime.UtcNow;
}
