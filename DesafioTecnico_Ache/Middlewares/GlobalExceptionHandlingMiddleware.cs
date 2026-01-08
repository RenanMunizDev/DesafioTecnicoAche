using DesafioTecnico_Ache.DTOs.Responses;
using DesafioTecnico_Ache.Services;
using FluentValidation;
using System.Net;
using System.Text.Json;

namespace DesafioTecnico_Ache.Middlewares;

/// <summary>
/// Middleware global para tratamento de exceções
/// Implementa práticas de segurança OWASP:
/// - Não expõe stack traces ou detalhes internos em produção
/// - Log detalhado para auditoria
/// - Respostas padronizadas
/// - Códigos HTTP apropriados
/// </summary>
public class GlobalExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;
    private readonly IWebHostEnvironment _environment;

    public GlobalExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionHandlingMiddleware> logger,
        IWebHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro não tratado: {Message}. Path: {Path}",
                ex.Message,
                context.Request.Path);

            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var errorResponse = new ApiErrorResponse
        {
            Path = context.Request.Path
        };

        switch (exception)
        {
            case ValidationException validationException:
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                errorResponse.ErrorCode = "VAL_001";
                errorResponse.Message = "Erro de validação";
                errorResponse.Details = validationException.Errors.Select(e => new
                {
                    Property = e.PropertyName,
                    Error = e.ErrorMessage
                });
                break;

            case BusinessException businessException:
                context.Response.StatusCode = (int)HttpStatusCode.UnprocessableEntity;
                errorResponse.ErrorCode = "BUS_001";
                errorResponse.Message = businessException.Message;
                break;

            case KeyNotFoundException:
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                errorResponse.ErrorCode = "NOT_001";
                errorResponse.Message = "Recurso não encontrado";
                break;

            case UnauthorizedAccessException:
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                errorResponse.ErrorCode = "AUTH_002";
                errorResponse.Message = "Acesso não autorizado";
                break;

            default:
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                errorResponse.ErrorCode = "SYS_001";
                errorResponse.Message = "Erro interno do servidor";
                
                // Apenas em desenvolvimento, mostrar detalhes da exceção
                if (_environment.IsDevelopment())
                {
                    errorResponse.Details = new
                    {
                        Type = exception.GetType().Name,
                        Message = exception.Message,
                        StackTrace = exception.StackTrace
                    };
                }
                break;
        }

        var json = JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        });

        await context.Response.WriteAsync(json);
    }
}
