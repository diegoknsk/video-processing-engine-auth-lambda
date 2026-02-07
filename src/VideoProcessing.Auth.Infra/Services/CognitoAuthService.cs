using Amazon;
using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using VideoProcessing.Auth.Application.OutputModels.Auth;
using VideoProcessing.Auth.Application.Ports;
using VideoProcessing.Auth.Infra.Models;

namespace VideoProcessing.Auth.Infra.Services;

/// <summary>
/// Implementação do serviço de autenticação com Amazon Cognito.
/// </summary>
public class CognitoAuthService : ICognitoAuthService
{
    private readonly IAmazonCognitoIdentityProvider _cognitoClient;
    private readonly CognitoOptions _options;
    private readonly ILogger<CognitoAuthService> _logger;

    public CognitoAuthService(
        IAmazonCognitoIdentityProvider cognitoClient,
        IOptions<CognitoOptions> options,
        ILogger<CognitoAuthService> logger)
    {
        _cognitoClient = cognitoClient;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<LoginOutput> LoginAsync(string username, string password, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Attempting login for user {Username}", username);

        try
        {
            var request = new InitiateAuthRequest
            {
                ClientId = _options.AppClientId,
                AuthFlow = AuthFlowType.USER_PASSWORD_AUTH,
                AuthParameters = new Dictionary<string, string>
                {
                    { "USERNAME", username },
                    { "PASSWORD", password }
                }
            };

            // Calcular SECRET_HASH apenas se AppClientSecret estiver configurado
            if (!string.IsNullOrWhiteSpace(_options.AppClientSecret))
            {
                var secretHash = SecretHashCalculator.ComputeSecretHash(
                    username,
                    _options.AppClientId,
                    _options.AppClientSecret);
                request.AuthParameters.Add("SECRET_HASH", secretHash);
            }

            var response = await _cognitoClient.InitiateAuthAsync(request, cancellationToken);

            if (response.AuthenticationResult == null)
            {
                _logger.LogWarning("Login failed for user {Username}: AuthenticationResult is null", username);
                throw new UnauthorizedAccessException("Credenciais inválidas");
            }

            var authResult = response.AuthenticationResult;

            _logger.LogInformation("Login successful for user {Username}", username);

            return new LoginOutput
            {
                AccessToken = authResult.AccessToken,
                IdToken = authResult.IdToken,
                RefreshToken = authResult.RefreshToken,
                ExpiresIn = authResult.ExpiresIn,
                TokenType = authResult.TokenType
            };
        }
        catch (NotAuthorizedException)
        {
            _logger.LogWarning("Login failed for user {Username}: NotAuthorizedException", username);
            throw new UnauthorizedAccessException("Credenciais inválidas");
        }
        catch (UserNotFoundException)
        {
            _logger.LogWarning("Login failed for user {Username}: UserNotFoundException", username);
            throw new UnauthorizedAccessException("Credenciais inválidas");
        }
    }

    public async Task<CreateUserOutput> SignUpAsync(string username, string password, string email, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Attempting to create user {Username} with email {Email}", username, email);

        try
        {
            var request = new SignUpRequest
            {
                ClientId = _options.AppClientId,
                Username = username,
                Password = password,
                UserAttributes = new List<AttributeType>
                {
                    new AttributeType { Name = "email", Value = email }
                }
            };

            // Calcular SECRET_HASH apenas se AppClientSecret estiver configurado
            if (!string.IsNullOrWhiteSpace(_options.AppClientSecret))
            {
                var secretHash = SecretHashCalculator.ComputeSecretHash(
                    username,
                    _options.AppClientId,
                    _options.AppClientSecret);
                request.SecretHash = secretHash;
            }

            var response = await _cognitoClient.SignUpAsync(request, cancellationToken);

            _logger.LogInformation("User {Username} created successfully; UserSub: {UserSub}, UserConfirmed: {UserConfirmed}", 
                username, response.UserSub, response.UserConfirmed);

            return new CreateUserOutput
            {
                UserId = response.UserSub,
                Username = username,
                UserConfirmed = response.UserConfirmed,
                ConfirmationRequired = !response.UserConfirmed
            };
        }
        catch (UsernameExistsException ex)
        {
            _logger.LogWarning("User creation failed for {Username}: UsernameExistsException - {ErrorCode}", username, ex.ErrorCode);
            throw;
        }
        catch (InvalidPasswordException ex)
        {
            _logger.LogWarning("User creation failed for {Username}: InvalidPasswordException - {ErrorCode}", username, ex.ErrorCode);
            throw;
        }
        catch (InvalidParameterException ex)
        {
            _logger.LogWarning("User creation failed for {Username}: InvalidParameterException - {ErrorCode}", username, ex.ErrorCode);
            throw new ArgumentException("Parâmetro inválido.", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "User creation failed for {Username}: Unexpected error", username);
            throw;
        }
    }
}
