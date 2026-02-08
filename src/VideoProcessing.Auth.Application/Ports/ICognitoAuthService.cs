using VideoProcessing.Auth.Application.OutputModels.Auth;

namespace VideoProcessing.Auth.Application.Ports;

/// <summary>
/// Porta (interface) para serviço de autenticação com Amazon Cognito.
/// Define o contrato para operações de autenticação que serão implementadas na camada de infraestrutura.
/// </summary>
public interface ICognitoAuthService
{
    /// <summary>
    /// Realiza autenticação de usuário no Amazon Cognito usando email e password (quando username_attributes = ["email"], o email é enviado como USERNAME).
    /// </summary>
    /// <param name="email">Email do usuário (identificador de login).</param>
    /// <param name="password">Senha do usuário.</param>
    /// <param name="cancellationToken">Token de cancelamento da operação.</param>
    /// <returns>LoginOutput contendo os tokens de autenticação (AccessToken, IdToken, RefreshToken, ExpiresIn, TokenType).</returns>
    /// <exception cref="UnauthorizedAccessException">Lançada quando as credenciais são inválidas (usuário não encontrado ou senha incorreta).</exception>
    Task<LoginOutput> LoginAsync(string email, string password, CancellationToken cancellationToken = default);

    /// <summary>
    /// Cria novo usuário no Amazon Cognito User Pool via SignUp.
    /// </summary>
    /// <param name="name">Nome do usuário (ex.: "Diego").</param>
    /// <param name="email">Email do usuário (identificador de login quando username_attributes = ["email"]).</param>
    /// <param name="password">Senha do usuário.</param>
    /// <param name="cancellationToken">Token de cancelamento da operação.</param>
    /// <returns>CreateUserOutput contendo informações do usuário criado (UserId/UserSub, Username, UserConfirmed, ConfirmationRequired).</returns>
    /// <exception cref="Amazon.CognitoIdentityProvider.Model.UsernameExistsException">Lançada quando o usuário já existe (deve ser tratada como 409 Conflict).</exception>
    /// <exception cref="Amazon.CognitoIdentityProvider.Model.InvalidPasswordException">Lançada quando a senha não atende aos requisitos de política (deve ser tratada como 422 Unprocessable Entity).</exception>
    /// <exception cref="Amazon.CognitoIdentityProvider.Model.InvalidParameterException">Lançada quando algum parâmetro é inválido (deve ser tratada como 400 Bad Request).</exception>
    Task<CreateUserOutput> SignUpAsync(string name, string email, string password, CancellationToken cancellationToken = default);
}
