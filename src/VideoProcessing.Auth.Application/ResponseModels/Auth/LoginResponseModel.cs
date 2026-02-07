namespace VideoProcessing.Auth.Application.ResponseModels.Auth;

/// <summary>
/// Modelo de resposta da API para o endpoint de login.
/// </summary>
public record LoginResponseModel
{
    /// <summary>
    /// Token de acesso JWT usado para autenticar requisições subsequentes.
    /// </summary>
    public string AccessToken { get; init; } = string.Empty;

    /// <summary>
    /// Token de identidade JWT contendo informações do usuário.
    /// </summary>
    public string IdToken { get; init; } = string.Empty;

    /// <summary>
    /// Token de atualização usado para obter novos tokens de acesso quando expirarem.
    /// </summary>
    public string? RefreshToken { get; init; }

    /// <summary>
    /// Tempo de expiração do token de acesso em segundos.
    /// </summary>
    public int ExpiresIn { get; init; }

    /// <summary>
    /// Tipo do token (geralmente "Bearer").
    /// </summary>
    public string TokenType { get; init; } = string.Empty;
}
