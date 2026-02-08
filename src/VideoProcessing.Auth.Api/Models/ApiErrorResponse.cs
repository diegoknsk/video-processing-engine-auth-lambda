namespace VideoProcessing.Auth.Api.Models;

/// <summary>
/// Modelo padronizado para respostas de erro da API.
/// </summary>
public class ApiErrorResponse
{
    /// <summary>
    /// Indica se a operação foi bem-sucedida (sempre false para erros).
    /// </summary>
    public bool Success { get; init; } = false;

    /// <summary>
    /// Detalhes do erro ocorrido.
    /// </summary>
    public ErrorDetail Error { get; init; } = null!;

    /// <summary>
    /// Timestamp UTC da resposta.
    /// </summary>
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;

    /// <summary>
    /// Cria uma resposta de erro com código e mensagem fornecidos.
    /// </summary>
    /// <param name="code">Código do erro.</param>
    /// <param name="message">Mensagem descritiva do erro.</param>
    /// <returns>Instância de ApiErrorResponse com Success = false.</returns>
    public static ApiErrorResponse Create(string code, string message)
    {
        return new ApiErrorResponse
        {
            Success = false,
            Error = new ErrorDetail
            {
                Code = code,
                Message = message
            },
            Timestamp = DateTime.UtcNow
        };
    }
}

/// <summary>
/// Detalhes de um erro ocorrido na API.
/// </summary>
public class ErrorDetail
{
    /// <summary>
    /// Código identificador do erro.
    /// </summary>
    public string Code { get; init; } = string.Empty;

    /// <summary>
    /// Mensagem descritiva do erro.
    /// </summary>
    public string Message { get; init; } = string.Empty;
}
