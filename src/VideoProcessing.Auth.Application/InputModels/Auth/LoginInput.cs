namespace VideoProcessing.Auth.Application.InputModels.Auth;

/// <summary>
/// Modelo de entrada para o caso de uso de login.
/// </summary>
public record LoginInput
{
    /// <summary>
    /// Nome de usuário para autenticação.
    /// </summary>
    public string Username { get; init; } = string.Empty;

    /// <summary>
    /// Senha do usuário.
    /// </summary>
    public string Password { get; init; } = string.Empty;
}
