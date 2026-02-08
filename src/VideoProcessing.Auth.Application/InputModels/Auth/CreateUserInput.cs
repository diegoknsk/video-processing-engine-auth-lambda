namespace VideoProcessing.Auth.Application.InputModels.Auth;

/// <summary>
/// Modelo de entrada para o caso de uso de criação de usuário.
/// </summary>
public record CreateUserInput
{
    /// <summary>
    /// Nome do usuário (ex.: "Diego").
    /// </summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// Email do usuário (identificador de login no Cognito quando username_attributes = ["email"]).
    /// </summary>
    public string Email { get; init; } = string.Empty;

    /// <summary>
    /// Senha do usuário.
    /// </summary>
    public string Password { get; init; } = string.Empty;
}
