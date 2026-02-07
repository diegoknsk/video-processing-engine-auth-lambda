namespace VideoProcessing.Auth.Application.ResponseModels.Auth;

/// <summary>
/// Modelo de resposta da API para o endpoint de criação de usuário.
/// </summary>
public record CreateUserResponseModel
{
    /// <summary>
    /// ID único do usuário criado (UserSub do Cognito).
    /// </summary>
    public string UserId { get; init; } = string.Empty;

    /// <summary>
    /// Nome de usuário criado.
    /// </summary>
    public string Username { get; init; } = string.Empty;

    /// <summary>
    /// Indica se o usuário está confirmado.
    /// </summary>
    public bool UserConfirmed { get; init; }

    /// <summary>
    /// Indica se é necessária confirmação do usuário (via email/SMS).
    /// </summary>
    public bool ConfirmationRequired { get; init; }
}
