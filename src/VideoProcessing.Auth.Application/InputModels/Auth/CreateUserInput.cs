namespace VideoProcessing.Auth.Application.InputModels.Auth;

/// <summary>
/// Modelo de entrada para o caso de uso de criação de usuário.
/// </summary>
public record CreateUserInput
{
    /// <summary>
    /// Nome de usuário para criação da conta.
    /// </summary>
    public string Username { get; init; } = string.Empty;

    /// <summary>
    /// Senha do usuário.
    /// </summary>
    public string Password { get; init; } = string.Empty;

    /// <summary>
    /// Email do usuário.
    /// </summary>
    public string Email { get; init; } = string.Empty;
}
