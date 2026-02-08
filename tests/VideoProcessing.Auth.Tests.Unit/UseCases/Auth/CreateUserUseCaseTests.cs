using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using VideoProcessing.Auth.Application.InputModels.Auth;
using VideoProcessing.Auth.Application.OutputModels.Auth;
using VideoProcessing.Auth.Application.Ports;
using VideoProcessing.Auth.Application.UseCases.Auth;

namespace VideoProcessing.Auth.Tests.Unit.UseCases.Auth;

public class CreateUserUseCaseTests
{
    private readonly Mock<ICognitoAuthService> _cognitoAuthServiceMock;
    private readonly Mock<ILogger<CreateUserUseCase>> _loggerMock;
    private readonly CreateUserUseCase _useCase;

    public CreateUserUseCaseTests()
    {
        _cognitoAuthServiceMock = new Mock<ICognitoAuthService>();
        _loggerMock = new Mock<ILogger<CreateUserUseCase>>();
        _useCase = new CreateUserUseCase(_cognitoAuthServiceMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_WhenSignUpSucceeds_ShouldReturnCreateUserResponseModel()
    {
        // Arrange
        var input = new CreateUserInput
        {
            Name = "Diego",
            Email = "test@example.com",
            Password = "password123"
        };
        var output = new CreateUserOutput
        {
            UserId = "user-sub-123",
            Username = "test@example.com",
            UserConfirmed = true,
            ConfirmationRequired = false
        };

        _cognitoAuthServiceMock
            .Setup(x => x.SignUpAsync(input.Name, input.Email, input.Password, It.IsAny<CancellationToken>()))
            .ReturnsAsync(output);

        // Act
        var result = await _useCase.ExecuteAsync(input);

        // Assert
        result.Should().NotBeNull();
        result.UserId.Should().Be("user-sub-123");
        result.Username.Should().Be("test@example.com");
        result.UserConfirmed.Should().BeTrue();
        result.ConfirmationRequired.Should().BeFalse();

        _cognitoAuthServiceMock.Verify(
            x => x.SignUpAsync(input.Name, input.Email, input.Password, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WhenUserConfirmedIsFalse_ShouldSetConfirmationRequiredToTrue()
    {
        // Arrange
        var input = new CreateUserInput
        {
            Name = "Diego",
            Email = "test@example.com",
            Password = "password123"
        };
        var output = new CreateUserOutput
        {
            UserId = "user-sub-123",
            Username = "test@example.com",
            UserConfirmed = false,
            ConfirmationRequired = true
        };

        _cognitoAuthServiceMock
            .Setup(x => x.SignUpAsync(input.Name, input.Email, input.Password, It.IsAny<CancellationToken>()))
            .ReturnsAsync(output);

        // Act
        var result = await _useCase.ExecuteAsync(input);

        // Assert
        result.UserConfirmed.Should().BeFalse();
        result.ConfirmationRequired.Should().BeTrue();
    }

    [Fact]
    public async Task ExecuteAsync_WhenServiceThrowsUsernameExistsException_ShouldPropagateException()
    {
        // Arrange
        var input = new CreateUserInput
        {
            Name = "Diego",
            Email = "existing@example.com",
            Password = "password123"
        };

        _cognitoAuthServiceMock
            .Setup(x => x.SignUpAsync(input.Name, input.Email, input.Password, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Amazon.CognitoIdentityProvider.Model.UsernameExistsException("User already exists"));

        // Act & Assert
        await Assert.ThrowsAsync<Amazon.CognitoIdentityProvider.Model.UsernameExistsException>(
            () => _useCase.ExecuteAsync(input));

        _cognitoAuthServiceMock.Verify(
            x => x.SignUpAsync(input.Name, input.Email, input.Password, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WhenServiceThrowsInvalidPasswordException_ShouldPropagateException()
    {
        // Arrange
        var input = new CreateUserInput
        {
            Name = "Diego",
            Email = "test@example.com",
            Password = "weak"
        };

        _cognitoAuthServiceMock
            .Setup(x => x.SignUpAsync(input.Name, input.Email, input.Password, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Amazon.CognitoIdentityProvider.Model.InvalidPasswordException("Password does not meet requirements"));

        // Act & Assert
        await Assert.ThrowsAsync<Amazon.CognitoIdentityProvider.Model.InvalidPasswordException>(
            () => _useCase.ExecuteAsync(input));

        _cognitoAuthServiceMock.Verify(
            x => x.SignUpAsync(input.Name, input.Email, input.Password, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WhenCancellationTokenIsCancelled_ShouldPropagateCancellation()
    {
        // Arrange
        var input = new CreateUserInput
        {
            Name = "Diego",
            Email = "test@example.com",
            Password = "password123"
        };
        var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        _cognitoAuthServiceMock
            .Setup(x => x.SignUpAsync(input.Name, input.Email, input.Password, cancellationTokenSource.Token))
            .ThrowsAsync(new OperationCanceledException());

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(
            () => _useCase.ExecuteAsync(input, cancellationTokenSource.Token));
    }
}
