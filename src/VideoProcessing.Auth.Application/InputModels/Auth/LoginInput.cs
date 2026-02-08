namespace VideoProcessing.Auth.Application.InputModels.Auth;

/// <summary>
/// Modelo de entrada para o caso de uso de login.
/// </summary>
public record LoginInput
{
    /// <summary>
    /// Email do usuário (identificador de login quando username_attributes = ["email"] no Cognito).
    /// </summary>
    public string Email { get; init; } = string.Empty;

    /// <summary>
    /// Senha do usuário.
    /// </summary>
    public string Password { get; init; } = string.Empty;
}
