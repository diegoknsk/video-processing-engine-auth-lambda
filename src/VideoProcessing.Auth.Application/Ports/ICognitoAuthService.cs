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
}
