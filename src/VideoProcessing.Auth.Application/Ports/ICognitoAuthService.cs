using VideoProcessing.Auth.Application.OutputModels.Auth;

namespace VideoProcessing.Auth.Application.Ports;

/// <summary>
/// Porta (interface) para serviço de autenticação com Amazon Cognito.
/// Define o contrato para operações de autenticação que serão implementadas na camada de infraestrutura.
/// </summary>
public interface ICognitoAuthService
{
    /// <summary>
    /// Realiza autenticação de usuário no Amazon Cognito usando username e password.
    /// </summary>
    /// <param name="username">Nome de usuário para autenticação.</param>
    /// <param name="password">Senha do usuário.</param>
    /// <param name="cancellationToken">Token de cancelamento da operação.</param>
    /// <returns>LoginOutput contendo os tokens de autenticação (AccessToken, IdToken, RefreshToken, ExpiresIn, TokenType).</returns>
    /// <exception cref="UnauthorizedAccessException">Lançada quando as credenciais são inválidas (usuário não encontrado ou senha incorreta).</exception>
    Task<LoginOutput> LoginAsync(string username, string password, CancellationToken cancellationToken = default);

    /// <summary>
    /// Cria novo usuário no Amazon Cognito User Pool via SignUp.
    /// </summary>
    /// <param name="username">Nome de usuário para criação da conta.</param>
    /// <param name="password">Senha do usuário.</param>
    /// <param name="email">Email do usuário.</param>
    /// <param name="cancellationToken">Token de cancelamento da operação.</param>
    /// <returns>CreateUserOutput contendo informações do usuário criado (UserId/UserSub, Username, UserConfirmed, ConfirmationRequired).</returns>
    /// <exception cref="Amazon.CognitoIdentityProvider.Model.UsernameExistsException">Lançada quando o usuário já existe (deve ser tratada como 409 Conflict).</exception>
    /// <exception cref="Amazon.CognitoIdentityProvider.Model.InvalidPasswordException">Lançada quando a senha não atende aos requisitos de política (deve ser tratada como 422 Unprocessable Entity).</exception>
    /// <exception cref="Amazon.CognitoIdentityProvider.Model.InvalidParameterException">Lançada quando algum parâmetro é inválido (deve ser tratada como 400 Bad Request).</exception>
    Task<CreateUserOutput> SignUpAsync(string username, string password, string email, CancellationToken cancellationToken = default);
}
