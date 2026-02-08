namespace VideoProcessing.Auth.Api.Models;

/// <summary>
/// Modelo padronizado para respostas de sucesso da API.
/// </summary>
/// <typeparam name="T">Tipo dos dados retornados.</typeparam>
public class ApiResponse<T>
{
    /// <summary>
    /// Indica se a operação foi bem-sucedida.
    /// </summary>
    public bool Success { get; init; } = true;

    /// <summary>
    /// Dados retornados pela operação.
    /// </summary>
    public T? Data { get; init; }

    /// <summary>
    /// Timestamp UTC da resposta.
    /// </summary>
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;

    /// <summary>
    /// Cria uma resposta de sucesso com os dados fornecidos.
    /// </summary>
    /// <param name="data">Dados a serem retornados.</param>
    /// <returns>Instância de ApiResponse com Success = true.</returns>
    public static ApiResponse<T> CreateSuccess(T data)
    {
        return new ApiResponse<T>
        {
            Success = true,
            Data = data,
            Timestamp = DateTime.UtcNow
        };
    }
}
