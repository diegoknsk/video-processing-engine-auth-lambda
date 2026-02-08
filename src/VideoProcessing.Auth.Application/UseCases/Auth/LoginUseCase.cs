using Microsoft.Extensions.Logging;
using VideoProcessing.Auth.Application.InputModels.Auth;
using VideoProcessing.Auth.Application.Ports;
using VideoProcessing.Auth.Application.Presenters.Auth;
using VideoProcessing.Auth.Application.ResponseModels.Auth;

namespace VideoProcessing.Auth.Application.UseCases.Auth;

/// <summary>
/// Caso de uso para autenticação de usuário via login.
/// </summary>
public class LoginUseCase
{
    private readonly ICognitoAuthService _cognitoAuthService;
    private readonly ILogger<LoginUseCase> _logger;

    public LoginUseCase(
        ICognitoAuthService cognitoAuthService,
        ILogger<LoginUseCase> logger)
    {
        _cognitoAuthService = cognitoAuthService;
        _logger = logger;
    }

    /// <summary>
    /// Executa o caso de uso de login.
    /// </summary>
    /// <param name="input">Dados de entrada do login (email e password).</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Modelo de resposta com tokens de autenticação.</returns>
    public async Task<LoginResponseModel> ExecuteAsync(LoginInput input, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Executing login use case for user {Email}", input.Email);

        var output = await _cognitoAuthService.LoginAsync(input.Email, input.Password, cancellationToken);

        _logger.LogInformation("Login successful for user {Email}", input.Email);

        return LoginPresenter.Present(output);
    }
}
