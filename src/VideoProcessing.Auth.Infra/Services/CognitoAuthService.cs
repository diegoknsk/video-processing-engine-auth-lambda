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

    public async Task<LoginOutput> LoginAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Attempting login for user {Email}", email);

        try
        {
            var request = new InitiateAuthRequest
            {
                ClientId = _options.ClientId,
                AuthFlow = AuthFlowType.USER_PASSWORD_AUTH,
                AuthParameters = new Dictionary<string, string>
                {
                    { "USERNAME", email },
                    { "PASSWORD", password }
                }
            };

            var response = await _cognitoClient.InitiateAuthAsync(request, cancellationToken);

            if (response.AuthenticationResult == null)
            {
                _logger.LogWarning("Login failed for user {Email}: AuthenticationResult is null", email);
                throw new UnauthorizedAccessException("Credenciais inválidas");
            }

            var authResult = response.AuthenticationResult;

            _logger.LogInformation("Login successful for user {Email}", email);

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
            _logger.LogWarning("Login failed for user {Email}: NotAuthorizedException", email);
            throw new UnauthorizedAccessException("Credenciais inválidas");
        }
        catch (UserNotFoundException)
        {
            _logger.LogWarning("Login failed for user {Email}: UserNotFoundException", email);
            throw new UnauthorizedAccessException("Credenciais inválidas");
        }
    }

    public async Task<CreateUserOutput> SignUpAsync(string name, string email, string password, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Attempting to create user {Email}", email);

        try
        {
            // AdminCreateUser: cria usuário já confirmado sem envio de email
            var createUserRequest = new AdminCreateUserRequest
            {
                UserPoolId = _options.UserPoolId,
                Username = email,
                TemporaryPassword = password,
                MessageAction = MessageActionType.SUPPRESS,
                UserAttributes = new List<AttributeType>
                {
                    new AttributeType { Name = "email", Value = email },
                    new AttributeType { Name = "name", Value = name },
                    new AttributeType { Name = "email_verified", Value = "true" }
                }
            };

            var createUserResponse = await _cognitoClient.AdminCreateUserAsync(createUserRequest, cancellationToken);

            // AdminSetUserPassword: define senha permanente para permitir login imediato
            var setPasswordRequest = new AdminSetUserPasswordRequest
            {
                UserPoolId = _options.UserPoolId,
                Username = email,
                Password = password,
                Permanent = true
            };

            await _cognitoClient.AdminSetUserPasswordAsync(setPasswordRequest, cancellationToken);

            // O UserSub está no atributo "sub" dentro de User.Attributes
            var userSub = createUserResponse.User?.Attributes?.FirstOrDefault(a => a.Name == "sub")?.Value 
                ?? createUserResponse.User?.Username 
                ?? string.Empty;

            _logger.LogInformation("User {Email} created successfully with AdminCreateUser; UserSub: {UserSub}",
                email, userSub);

            return new CreateUserOutput
            {
                UserId = userSub,
                Username = email,
                UserConfirmed = true,
                ConfirmationRequired = false
            };
        }
        catch (UsernameExistsException ex)
        {
            _logger.LogWarning("User creation failed for {Email}: UsernameExistsException - {ErrorCode}", email, ex.ErrorCode);
            throw;
        }
        catch (InvalidPasswordException ex)
        {
            _logger.LogWarning("User creation failed for {Email}: InvalidPasswordException - {ErrorCode}", email, ex.ErrorCode);
            throw;
        }
        catch (InvalidParameterException ex)
        {
            _logger.LogWarning("User creation failed for {Email}: InvalidParameterException - {ErrorCode}", email, ex.ErrorCode);
            throw new ArgumentException("Parâmetro inválido.", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "User creation failed for {Email}: Unexpected error", email);
            throw;
        }
    }
}
