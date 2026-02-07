namespace VideoProcessing.Auth.Application.OutputModels.Auth;

/// <summary>
/// Modelo de saída do caso de uso de login contendo os tokens de autenticação.
/// </summary>
public record LoginOutput
{
    /// <summary>
    /// Token de acesso JWT.
    /// </summary>
    public string AccessToken { get; init; } = string.Empty;

    /// <summary>
    /// Token de identidade JWT.
    /// </summary>
    public string IdToken { get; init; } = string.Empty;

    /// <summary>
    /// Token de atualização (pode ser null dependendo da configuração do pool).
    /// </summary>
    public string? RefreshToken { get; init; }

    /// <summary>
    /// Tempo de expiração do token em segundos.
    /// </summary>
    public int ExpiresIn { get; init; }

    /// <summary>
    /// Tipo do token (geralmente "Bearer").
    /// </summary>
    public string TokenType { get; init; } = string.Empty;
}
