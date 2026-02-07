using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Amazon.Runtime;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using VideoProcessing.Auth.Application.OutputModels.Auth;
using VideoProcessing.Auth.Infra.Models;
using VideoProcessing.Auth.Infra.Services;

namespace VideoProcessing.Auth.Tests.Unit.Services;

public class CognitoAuthServiceTests
{
    private readonly Mock<IAmazonCognitoIdentityProvider> _cognitoClientMock;
    private readonly Mock<IOptions<CognitoOptions>> _optionsMock;
    private readonly Mock<ILogger<CognitoAuthService>> _loggerMock;
    private readonly CognitoOptions _options;

    public CognitoAuthServiceTests()
    {
        _cognitoClientMock = new Mock<IAmazonCognitoIdentityProvider>();
        _optionsMock = new Mock<IOptions<CognitoOptions>>();
        _loggerMock = new Mock<ILogger<CognitoAuthService>>();

        _options = new CognitoOptions
        {
            Region = "us-east-1",
            AppClientId = "test-client-id",
            AppClientSecret = "test-client-secret",
            UserPoolId = "test-pool-id"
        };

        _optionsMock.Setup(x => x.Value).Returns(_options);
    }

    [Fact]
    public async Task LoginAsync_WhenLoginSucceeds_ShouldReturnLoginOutput()
    {
        // Arrange
        var username = "testuser";
        var password = "password123";
        var accessToken = "access-token";
        var idToken = "id-token";
        var refreshToken = "refresh-token";

        var authResult = new AuthenticationResultType
        {
            AccessToken = accessToken,
            IdToken = idToken,
            RefreshToken = refreshToken,
            ExpiresIn = 3600,
            TokenType = "Bearer"
        };

        var response = new InitiateAuthResponse
        {
            AuthenticationResult = authResult
        };

        _cognitoClientMock
            .Setup(x => x.InitiateAuthAsync(It.IsAny<InitiateAuthRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        var service = new CognitoAuthService(_cognitoClientMock.Object, _optionsMock.Object, _loggerMock.Object);

        // Act
        var result = await service.LoginAsync(username, password);

        // Assert
        result.Should().NotBeNull();
        result.AccessToken.Should().Be(accessToken);
        result.IdToken.Should().Be(idToken);
        result.RefreshToken.Should().Be(refreshToken);
        result.ExpiresIn.Should().Be(3600);
        result.TokenType.Should().Be("Bearer");
    }

    [Fact]
    public async Task LoginAsync_WhenNotAuthorizedException_ShouldThrowUnauthorizedAccessException()
    {
        // Arrange
        var username = "testuser";
        var password = "wrongpassword";

        _cognitoClientMock
            .Setup(x => x.InitiateAuthAsync(It.IsAny<InitiateAuthRequest>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new NotAuthorizedException("Invalid credentials"));

        var service = new CognitoAuthService(_cognitoClientMock.Object, _optionsMock.Object, _loggerMock.Object);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => service.LoginAsync(username, password));

        exception.Message.Should().Be("Credenciais inválidas");
    }

    [Fact]
    public async Task LoginAsync_WhenUserNotFoundException_ShouldThrowUnauthorizedAccessException()
    {
        // Arrange
        var username = "nonexistentuser";
        var password = "password123";

        _cognitoClientMock
            .Setup(x => x.InitiateAuthAsync(It.IsAny<InitiateAuthRequest>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new UserNotFoundException("User not found"));

        var service = new CognitoAuthService(_cognitoClientMock.Object, _optionsMock.Object, _loggerMock.Object);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => service.LoginAsync(username, password));

        exception.Message.Should().Be("Credenciais inválidas");
    }

    [Fact]
    public async Task LoginAsync_WhenAuthenticationResultIsNull_ShouldThrowUnauthorizedAccessException()
    {
        // Arrange
        var username = "testuser";
        var password = "password123";

        var response = new InitiateAuthResponse
        {
            AuthenticationResult = null
        };

        _cognitoClientMock
            .Setup(x => x.InitiateAuthAsync(It.IsAny<InitiateAuthRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        var service = new CognitoAuthService(_cognitoClientMock.Object, _optionsMock.Object, _loggerMock.Object);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => service.LoginAsync(username, password));

        exception.Message.Should().Be("Credenciais inválidas");
    }

    [Fact]
    public async Task LoginAsync_WhenAppClientSecretIsPresent_ShouldIncludeSecretHashInRequest()
    {
        // Arrange
        var username = "testuser";
        var password = "password123";

        var authResult = new AuthenticationResultType
        {
            AccessToken = "access-token",
            IdToken = "id-token",
            RefreshToken = "refresh-token",
            ExpiresIn = 3600,
            TokenType = "Bearer"
        };

        var response = new InitiateAuthResponse
        {
            AuthenticationResult = authResult
        };

        InitiateAuthRequest? capturedRequest = null;
        _cognitoClientMock
            .Setup(x => x.InitiateAuthAsync(It.IsAny<InitiateAuthRequest>(), It.IsAny<CancellationToken>()))
            .Callback<InitiateAuthRequest, CancellationToken>((req, ct) => capturedRequest = req)
            .ReturnsAsync(response);

        var service = new CognitoAuthService(_cognitoClientMock.Object, _optionsMock.Object, _loggerMock.Object);

        // Act
        await service.LoginAsync(username, password);

        // Assert
        capturedRequest.Should().NotBeNull();
        capturedRequest!.AuthParameters.Should().ContainKey("SECRET_HASH");
        capturedRequest.AuthParameters["SECRET_HASH"].Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task LoginAsync_WhenAppClientSecretIsEmpty_ShouldNotIncludeSecretHashInRequest()
    {
        // Arrange
        var optionsWithoutSecret = new CognitoOptions
        {
            Region = "us-east-1",
            AppClientId = "test-client-id",
            AppClientSecret = string.Empty,
            UserPoolId = "test-pool-id"
        };

        var optionsMockWithoutSecret = new Mock<IOptions<CognitoOptions>>();
        optionsMockWithoutSecret.Setup(x => x.Value).Returns(optionsWithoutSecret);

        var username = "testuser";
        var password = "password123";

        var authResult = new AuthenticationResultType
        {
            AccessToken = "access-token",
            IdToken = "id-token",
            RefreshToken = "refresh-token",
            ExpiresIn = 3600,
            TokenType = "Bearer"
        };

        var response = new InitiateAuthResponse
        {
            AuthenticationResult = authResult
        };

        InitiateAuthRequest? capturedRequest = null;
        _cognitoClientMock
            .Setup(x => x.InitiateAuthAsync(It.IsAny<InitiateAuthRequest>(), It.IsAny<CancellationToken>()))
            .Callback<InitiateAuthRequest, CancellationToken>((req, ct) => capturedRequest = req)
            .ReturnsAsync(response);

        var service = new CognitoAuthService(_cognitoClientMock.Object, optionsMockWithoutSecret.Object, _loggerMock.Object);

        // Act
        await service.LoginAsync(username, password);

        // Assert
        capturedRequest.Should().NotBeNull();
        capturedRequest!.AuthParameters.Should().NotContainKey("SECRET_HASH");
    }
}
