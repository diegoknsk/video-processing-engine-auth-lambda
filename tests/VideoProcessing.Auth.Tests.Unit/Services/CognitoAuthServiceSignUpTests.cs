using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using VideoProcessing.Auth.Application.OutputModels.Auth;
using VideoProcessing.Auth.Infra.Models;
using VideoProcessing.Auth.Infra.Services;

namespace VideoProcessing.Auth.Tests.Unit.Services;

public class CognitoAuthServiceSignUpTests
{
    private readonly Mock<IAmazonCognitoIdentityProvider> _cognitoClientMock;
    private readonly Mock<IOptions<CognitoOptions>> _optionsMock;
    private readonly Mock<ILogger<CognitoAuthService>> _loggerMock;
    private readonly CognitoOptions _options;

    public CognitoAuthServiceSignUpTests()
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
    public async Task SignUpAsync_WhenSignUpSucceeds_ShouldReturnCreateUserOutput()
    {
        // Arrange
        var username = "testuser";
        var password = "password123";
        var email = "test@example.com";
        var userSub = "user-sub-123";

        var response = new SignUpResponse
        {
            UserSub = userSub,
            UserConfirmed = true
        };

        _cognitoClientMock
            .Setup(x => x.SignUpAsync(It.IsAny<SignUpRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        var service = new CognitoAuthService(_cognitoClientMock.Object, _optionsMock.Object, _loggerMock.Object);

        // Act
        var result = await service.SignUpAsync(username, password, email);

        // Assert
        result.Should().NotBeNull();
        result.UserId.Should().Be(userSub);
        result.Username.Should().Be(username);
        result.UserConfirmed.Should().BeTrue();
        result.ConfirmationRequired.Should().BeFalse();
    }

    [Fact]
    public async Task SignUpAsync_WhenUserConfirmedIsFalse_ShouldSetConfirmationRequiredToTrue()
    {
        // Arrange
        var username = "testuser";
        var password = "password123";
        var email = "test@example.com";
        var userSub = "user-sub-123";

        var response = new SignUpResponse
        {
            UserSub = userSub,
            UserConfirmed = false
        };

        _cognitoClientMock
            .Setup(x => x.SignUpAsync(It.IsAny<SignUpRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        var service = new CognitoAuthService(_cognitoClientMock.Object, _optionsMock.Object, _loggerMock.Object);

        // Act
        var result = await service.SignUpAsync(username, password, email);

        // Assert
        result.UserConfirmed.Should().BeFalse();
        result.ConfirmationRequired.Should().BeTrue();
    }

    [Fact]
    public async Task SignUpAsync_WhenUsernameExistsException_ShouldThrowException()
    {
        // Arrange
        var username = "existinguser";
        var password = "password123";
        var email = "existing@example.com";

        _cognitoClientMock
            .Setup(x => x.SignUpAsync(It.IsAny<SignUpRequest>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new UsernameExistsException("User already exists"));

        var service = new CognitoAuthService(_cognitoClientMock.Object, _optionsMock.Object, _loggerMock.Object);

        // Act & Assert
        await Assert.ThrowsAsync<UsernameExistsException>(
            () => service.SignUpAsync(username, password, email));
    }

    [Fact]
    public async Task SignUpAsync_WhenInvalidPasswordException_ShouldThrowException()
    {
        // Arrange
        var username = "testuser";
        var password = "weak";
        var email = "test@example.com";

        _cognitoClientMock
            .Setup(x => x.SignUpAsync(It.IsAny<SignUpRequest>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidPasswordException("Password does not meet requirements"));

        var service = new CognitoAuthService(_cognitoClientMock.Object, _optionsMock.Object, _loggerMock.Object);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidPasswordException>(
            () => service.SignUpAsync(username, password, email));
    }

    [Fact]
    public async Task SignUpAsync_WhenInvalidParameterException_ShouldThrowArgumentException()
    {
        // Arrange
        var username = "testuser";
        var password = "password123";
        var email = "invalid-email";

        _cognitoClientMock
            .Setup(x => x.SignUpAsync(It.IsAny<SignUpRequest>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidParameterException("Invalid parameter"));

        var service = new CognitoAuthService(_cognitoClientMock.Object, _optionsMock.Object, _loggerMock.Object);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => service.SignUpAsync(username, password, email));

        exception.Message.Should().Be("Parâmetro inválido.");
    }

    [Fact]
    public async Task SignUpAsync_WhenAppClientSecretIsPresent_ShouldIncludeSecretHashInRequest()
    {
        // Arrange
        var username = "testuser";
        var password = "password123";
        var email = "test@example.com";

        var response = new SignUpResponse
        {
            UserSub = "user-sub-123",
            UserConfirmed = true
        };

        SignUpRequest? capturedRequest = null;
        _cognitoClientMock
            .Setup(x => x.SignUpAsync(It.IsAny<SignUpRequest>(), It.IsAny<CancellationToken>()))
            .Callback<SignUpRequest, CancellationToken>((req, ct) => capturedRequest = req)
            .ReturnsAsync(response);

        var service = new CognitoAuthService(_cognitoClientMock.Object, _optionsMock.Object, _loggerMock.Object);

        // Act
        await service.SignUpAsync(username, password, email);

        // Assert
        capturedRequest.Should().NotBeNull();
        capturedRequest!.SecretHash.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task SignUpAsync_WhenAppClientSecretIsEmpty_ShouldNotIncludeSecretHashInRequest()
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
        var email = "test@example.com";

        var response = new SignUpResponse
        {
            UserSub = "user-sub-123",
            UserConfirmed = true
        };

        SignUpRequest? capturedRequest = null;
        _cognitoClientMock
            .Setup(x => x.SignUpAsync(It.IsAny<SignUpRequest>(), It.IsAny<CancellationToken>()))
            .Callback<SignUpRequest, CancellationToken>((req, ct) => capturedRequest = req)
            .ReturnsAsync(response);

        var service = new CognitoAuthService(_cognitoClientMock.Object, optionsMockWithoutSecret.Object, _loggerMock.Object);

        // Act
        await service.SignUpAsync(username, password, email);

        // Assert
        capturedRequest.Should().NotBeNull();
        capturedRequest!.SecretHash.Should().BeNullOrEmpty();
    }

    [Fact]
    public async Task SignUpAsync_ShouldIncludeEmailInUserAttributes()
    {
        // Arrange
        var username = "testuser";
        var password = "password123";
        var email = "test@example.com";

        var response = new SignUpResponse
        {
            UserSub = "user-sub-123",
            UserConfirmed = true
        };

        SignUpRequest? capturedRequest = null;
        _cognitoClientMock
            .Setup(x => x.SignUpAsync(It.IsAny<SignUpRequest>(), It.IsAny<CancellationToken>()))
            .Callback<SignUpRequest, CancellationToken>((req, ct) => capturedRequest = req)
            .ReturnsAsync(response);

        var service = new CognitoAuthService(_cognitoClientMock.Object, _optionsMock.Object, _loggerMock.Object);

        // Act
        await service.SignUpAsync(username, password, email);

        // Assert
        capturedRequest.Should().NotBeNull();
        capturedRequest!.UserAttributes.Should().NotBeNull();
        capturedRequest.UserAttributes.Should().Contain(attr => attr.Name == "email" && attr.Value == email);
    }
}
