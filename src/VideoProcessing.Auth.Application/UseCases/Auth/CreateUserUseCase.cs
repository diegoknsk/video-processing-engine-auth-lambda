using Microsoft.Extensions.Logging;
using VideoProcessing.Auth.Application.InputModels.Auth;
using VideoProcessing.Auth.Application.Ports;
using VideoProcessing.Auth.Application.Presenters.Auth;
using VideoProcessing.Auth.Application.ResponseModels.Auth;

namespace VideoProcessing.Auth.Application.UseCases.Auth;

/// <summary>
/// Caso de uso para criação de usuário via SignUp.
/// </summary>
public class CreateUserUseCase
{
    private readonly ICognitoAuthService _cognitoAuthService;
    private readonly ILogger<CreateUserUseCase> _logger;

    public CreateUserUseCase(
        ICognitoAuthService cognitoAuthService,
        ILogger<CreateUserUseCase> logger)
    {
        _cognitoAuthService = cognitoAuthService;
        _logger = logger;
    }

    /// <summary>
    /// Executa o caso de uso de criação de usuário.
    /// </summary>
    /// <param name="input">Dados de entrada da criação de usuário (name, email e password).</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Modelo de resposta com informações do usuário criado.</returns>
    public async Task<CreateUserResponseModel> ExecuteAsync(CreateUserInput input, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Executing create user use case for email {Email}", input.Email);

        var output = await _cognitoAuthService.SignUpAsync(input.Name, input.Email, input.Password, cancellationToken);

        _logger.LogInformation("User {Email} created successfully; UserId: {UserId}, UserConfirmed: {UserConfirmed}",
            input.Email, output.UserId, output.UserConfirmed);

        return CreateUserPresenter.Present(output);
    }
}
