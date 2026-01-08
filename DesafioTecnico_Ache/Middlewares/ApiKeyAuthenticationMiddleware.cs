using DesafioTecnico_Ache.DTOs.Responses;
using System.Text.Json;

namespace DesafioTecnico_Ache.Middlewares;

/// <summary>
/// Middleware de autenticação por API Key
/// Implementa segurança básica conforme recomendações OWASP:
/// - API Key Authentication
/// - Header customizado (X-API-Key)
/// - Logging de tentativas de acesso não autorizadas
/// 
/// Em produção, considerar:
/// - OAuth 2.0 / JWT
/// - Integração com SAP Cloud Platform Identity Authentication
/// - Rate limiting por API Key
/// - Rotação de chaves
/// </summary>
public class ApiKeyAuthenticationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IConfiguration _configuration;
    private readonly ILogger<ApiKeyAuthenticationMiddleware> _logger;
    private const string ApiKeyHeaderName = "X-API-Key";

    public ApiKeyAuthenticationMiddleware(
        RequestDelegate next,
        IConfiguration configuration,
        ILogger<ApiKeyAuthenticationMiddleware> logger)
    {
        _next = next;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Permitir acesso ao Swagger sem autenticação em desenvolvimento
        if (context.Request.Path.StartsWithSegments("/swagger"))
        {
            await _next(context);
            return;
        }

        // Verificar se a API Key está presente no header
        if (!context.Request.Headers.TryGetValue(ApiKeyHeaderName, out var extractedApiKey))
        {
            _logger.LogWarning("Tentativa de acesso sem API Key. IP: {IpAddress}, Path: {Path}",
                context.Connection.RemoteIpAddress,
                context.Request.Path);

            await WriteUnauthorizedResponse(context, "API Key não fornecida");
            return;
        }

        // Validar a API Key
        var validApiKeys = _configuration.GetSection("ApiKeys").Get<string[]>() ?? Array.Empty<string>();
        
        if (!validApiKeys.Contains(extractedApiKey.ToString()))
        {
            _logger.LogWarning("Tentativa de acesso com API Key inválida. IP: {IpAddress}, Path: {Path}",
                context.Connection.RemoteIpAddress,
                context.Request.Path);

            await WriteUnauthorizedResponse(context, "API Key inválida");
            return;
        }

        _logger.LogInformation("Acesso autorizado via API Key. Path: {Path}", context.Request.Path);

        await _next(context);
    }

    private async Task WriteUnauthorizedResponse(HttpContext context, string message)
    {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        context.Response.ContentType = "application/json";

        var errorResponse = new ApiErrorResponse
        {
            ErrorCode = "AUTH_001",
            Message = message,
            Path = context.Request.Path
        };

        var json = JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(json);
    }
}
