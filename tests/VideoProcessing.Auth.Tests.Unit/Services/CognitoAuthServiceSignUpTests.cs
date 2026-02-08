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
            UserPoolId = "test-pool-id",
            ClientId = "test-client-id"
        };

        _optionsMock.Setup(x => x.Value).Returns(_options);
    }

    [Fact]
    public async Task SignUpAsync_WhenSignUpSucceeds_ShouldReturnCreateUserOutput()
    {
        // Arrange
        var name = "Diego";
        var email = "test@example.com";
        var password = "password123";
        var userSub = "user-sub-123";

        var adminCreateUserResponse = new AdminCreateUserResponse
        {
            User = new UserType
            {
                Username = email,
                Attributes = new List<AttributeType>
                {
                    new AttributeType { Name = "sub", Value = userSub },
                    new AttributeType { Name = "email", Value = email },
                    new AttributeType { Name = "name", Value = name }
                }
            }
        };

        _cognitoClientMock
            .Setup(x => x.AdminCreateUserAsync(It.IsAny<AdminCreateUserRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(adminCreateUserResponse);

        _cognitoClientMock
            .Setup(x => x.AdminSetUserPasswordAsync(It.IsAny<AdminSetUserPasswordRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AdminSetUserPasswordResponse());

        var service = new CognitoAuthService(_cognitoClientMock.Object, _optionsMock.Object, _loggerMock.Object);

        // Act
        var result = await service.SignUpAsync(name, email, password);

        // Assert
        result.Should().NotBeNull();
        result.UserId.Should().Be(userSub);
        result.Username.Should().Be(email);
        result.UserConfirmed.Should().BeTrue();
        result.ConfirmationRequired.Should().BeFalse();
    }

    [Fact]
    public async Task SignUpAsync_WhenAdminCreateUserSucceeds_ShouldAlwaysReturnUserConfirmedTrue()
    {
        // Arrange
        var name = "Diego";
        var email = "test@example.com";
        var password = "password123";
        var userSub = "user-sub-123";

        var adminCreateUserResponse = new AdminCreateUserResponse
        {
            User = new UserType
            {
                Username = email,
                Attributes = new List<AttributeType>
                {
                    new AttributeType { Name = "sub", Value = userSub },
                    new AttributeType { Name = "email", Value = email },
                    new AttributeType { Name = "name", Value = name }
                }
            }
        };

        _cognitoClientMock
            .Setup(x => x.AdminCreateUserAsync(It.IsAny<AdminCreateUserRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(adminCreateUserResponse);

        _cognitoClientMock
            .Setup(x => x.AdminSetUserPasswordAsync(It.IsAny<AdminSetUserPasswordRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AdminSetUserPasswordResponse());

        var service = new CognitoAuthService(_cognitoClientMock.Object, _optionsMock.Object, _loggerMock.Object);

        // Act
        var result = await service.SignUpAsync(name, email, password);

        // Assert
        // Com AdminCreateUser + AdminSetUserPassword, o usuário sempre fica confirmado
        result.UserConfirmed.Should().BeTrue();
        result.ConfirmationRequired.Should().BeFalse();
    }

    [Fact]
    public async Task SignUpAsync_WhenUsernameExistsException_ShouldThrowException()
    {
        // Arrange
        var name = "Diego";
        var email = "existing@example.com";
        var password = "password123";

        _cognitoClientMock
            .Setup(x => x.AdminCreateUserAsync(It.IsAny<AdminCreateUserRequest>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new UsernameExistsException("User already exists"));

        var service = new CognitoAuthService(_cognitoClientMock.Object, _optionsMock.Object, _loggerMock.Object);

        // Act & Assert
        await Assert.ThrowsAsync<UsernameExistsException>(
            () => service.SignUpAsync(name, email, password));
    }

    [Fact]
    public async Task SignUpAsync_WhenInvalidPasswordExceptionOnAdminCreateUser_ShouldThrowException()
    {
        // Arrange
        var name = "Diego";
        var email = "test@example.com";
        var password = "weak";

        // InvalidPasswordException pode ser lançada no AdminCreateUser
        _cognitoClientMock
            .Setup(x => x.AdminCreateUserAsync(It.IsAny<AdminCreateUserRequest>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidPasswordException("Password does not meet requirements"));

        var service = new CognitoAuthService(_cognitoClientMock.Object, _optionsMock.Object, _loggerMock.Object);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidPasswordException>(
            () => service.SignUpAsync(name, email, password));
    }

    [Fact]
    public async Task SignUpAsync_WhenInvalidPasswordExceptionOnAdminSetUserPassword_ShouldThrowException()
    {
        // Arrange
        var name = "Diego";
        var email = "test@example.com";
        var password = "weak";
        var userSub = "user-sub-123";

        var adminCreateUserResponse = new AdminCreateUserResponse
        {
            User = new UserType
            {
                Username = email,
                Attributes = new List<AttributeType>
                {
                    new AttributeType { Name = "sub", Value = userSub },
                    new AttributeType { Name = "email", Value = email },
                    new AttributeType { Name = "name", Value = name }
                }
            }
        };

        // AdminCreateUser sucede, mas AdminSetUserPassword falha com senha inválida
        _cognitoClientMock
            .Setup(x => x.AdminCreateUserAsync(It.IsAny<AdminCreateUserRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(adminCreateUserResponse);

        _cognitoClientMock
            .Setup(x => x.AdminSetUserPasswordAsync(It.IsAny<AdminSetUserPasswordRequest>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidPasswordException("Password does not meet requirements"));

        var service = new CognitoAuthService(_cognitoClientMock.Object, _optionsMock.Object, _loggerMock.Object);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidPasswordException>(
            () => service.SignUpAsync(name, email, password));
    }

    [Fact]
    public async Task SignUpAsync_WhenInvalidParameterException_ShouldThrowArgumentException()
    {
        // Arrange
        var name = "Diego";
        var email = "test@example.com";
        var password = "password123";

        _cognitoClientMock
            .Setup(x => x.AdminCreateUserAsync(It.IsAny<AdminCreateUserRequest>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidParameterException("Invalid parameter"));

        var service = new CognitoAuthService(_cognitoClientMock.Object, _optionsMock.Object, _loggerMock.Object);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => service.SignUpAsync(name, email, password));

        exception.Message.Should().Be("Parâmetro inválido.");
    }

    [Fact]
    public async Task SignUpAsync_ShouldCallAdminCreateUserWithCorrectParameters()
    {
        // Arrange
        var name = "Diego";
        var email = "diegosa@email.com";
        var password = "password123";
        var userSub = "user-sub-123";

        AdminCreateUserRequest? capturedCreateRequest = null;
        AdminSetUserPasswordRequest? capturedPasswordRequest = null;

        var adminCreateUserResponse = new AdminCreateUserResponse
        {
            User = new UserType
            {
                Username = email,
                Attributes = new List<AttributeType>
                {
                    new AttributeType { Name = "sub", Value = userSub },
                    new AttributeType { Name = "email", Value = email },
                    new AttributeType { Name = "name", Value = name }
                }
            }
        };

        _cognitoClientMock
            .Setup(x => x.AdminCreateUserAsync(It.IsAny<AdminCreateUserRequest>(), It.IsAny<CancellationToken>()))
            .Callback<AdminCreateUserRequest, CancellationToken>((req, ct) => capturedCreateRequest = req)
            .ReturnsAsync(adminCreateUserResponse);

        _cognitoClientMock
            .Setup(x => x.AdminSetUserPasswordAsync(It.IsAny<AdminSetUserPasswordRequest>(), It.IsAny<CancellationToken>()))
            .Callback<AdminSetUserPasswordRequest, CancellationToken>((req, ct) => capturedPasswordRequest = req)
            .ReturnsAsync(new AdminSetUserPasswordResponse());

        var service = new CognitoAuthService(_cognitoClientMock.Object, _optionsMock.Object, _loggerMock.Object);

        // Act
        await service.SignUpAsync(name, email, password);

        // Assert - AdminCreateUser
        capturedCreateRequest.Should().NotBeNull();
        capturedCreateRequest!.UserPoolId.Should().Be(_options.UserPoolId);
        capturedCreateRequest.Username.Should().Be(email);
        capturedCreateRequest.TemporaryPassword.Should().Be(password);
        capturedCreateRequest.MessageAction.Should().Be(MessageActionType.SUPPRESS);
        capturedCreateRequest.UserAttributes.Should().NotBeNull();
        capturedCreateRequest.UserAttributes.Should().Contain(attr => attr.Name == "email" && attr.Value == email);
        capturedCreateRequest.UserAttributes.Should().Contain(attr => attr.Name == "name" && attr.Value == name);
        capturedCreateRequest.UserAttributes.Should().Contain(attr => attr.Name == "email_verified" && attr.Value == "true");

        // Assert - AdminSetUserPassword
        capturedPasswordRequest.Should().NotBeNull();
        capturedPasswordRequest!.UserPoolId.Should().Be(_options.UserPoolId);
        capturedPasswordRequest.Username.Should().Be(email);
        capturedPasswordRequest.Password.Should().Be(password);
        capturedPasswordRequest.Permanent.Should().BeTrue();
    }
}
