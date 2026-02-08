namespace VideoProcessing.Auth.Application.OutputModels.Auth;

/// <summary>
/// Modelo de saída do caso de uso de criação de usuário.
/// </summary>
public record CreateUserOutput
{
    /// <summary>
    /// ID único do usuário (UserSub do Cognito).
    /// </summary>
    public string UserId { get; init; } = string.Empty;

    /// <summary>
    /// Nome de usuário.
    /// </summary>
    public string Username { get; init; } = string.Empty;

    /// <summary>
    /// Indica se o usuário está confirmado.
    /// </summary>
    public bool UserConfirmed { get; init; }

    /// <summary>
    /// Indica se é necessária confirmação do usuário.
    /// </summary>
    public bool ConfirmationRequired { get; init; }
}
