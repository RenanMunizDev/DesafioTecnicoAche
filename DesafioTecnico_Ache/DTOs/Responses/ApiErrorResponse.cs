namespace DesafioTecnico_Ache.DTOs.Responses;

/// <summary>
/// DTO padrão de resposta para erros da API
/// Segue recomendações OWASP para não expor detalhes sensíveis do sistema
/// </summary>
public class ApiErrorResponse
{
    /// <summary>
    /// Código de erro único para rastreabilidade
    /// </summary>
    public string ErrorCode { get; set; } = string.Empty;

    /// <summary>
    /// Mensagem amigável ao usuário
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Detalhes adicionais (apenas em ambiente de desenvolvimento)
    /// </summary>
    public object? Details { get; set; }

    /// <summary>
    /// Timestamp do erro
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Path da requisição que gerou o erro
    /// </summary>
    public string? Path { get; set; }
}
