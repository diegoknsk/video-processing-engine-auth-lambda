using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using VideoProcessing.Auth.Application.InputModels.Auth;
using VideoProcessing.Auth.Application.OutputModels.Auth;
using VideoProcessing.Auth.Application.Ports;
using VideoProcessing.Auth.Application.ResponseModels.Auth;
using VideoProcessing.Auth.Application.UseCases.Auth;

namespace VideoProcessing.Auth.Tests.Unit.UseCases.Auth;

public class LoginUseCaseTests
{
    private readonly Mock<ICognitoAuthService> _cognitoAuthServiceMock;
    private readonly Mock<ILogger<LoginUseCase>> _loggerMock;
    private readonly LoginUseCase _useCase;

    public LoginUseCaseTests()
    {
        _cognitoAuthServiceMock = new Mock<ICognitoAuthService>();
        _loggerMock = new Mock<ILogger<LoginUseCase>>();
        _useCase = new LoginUseCase(_cognitoAuthServiceMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_WhenLoginSucceeds_ShouldReturnLoginResponseModel()
    {
        // Arrange
        var input = new LoginInput { Username = "testuser", Password = "password123" };
        var output = new LoginOutput
        {
            AccessToken = "access-token",
            IdToken = "id-token",
            RefreshToken = "refresh-token",
            ExpiresIn = 3600,
            TokenType = "Bearer"
        };

        _cognitoAuthServiceMock
            .Setup(x => x.LoginAsync(input.Username, input.Password, It.IsAny<CancellationToken>()))
            .ReturnsAsync(output);

        // Act
        var result = await _useCase.ExecuteAsync(input);

        // Assert
        result.Should().NotBeNull();
        result.AccessToken.Should().Be("access-token");
        result.IdToken.Should().Be("id-token");
        result.RefreshToken.Should().Be("refresh-token");
        result.ExpiresIn.Should().Be(3600);
        result.TokenType.Should().Be("Bearer");

        _cognitoAuthServiceMock.Verify(
            x => x.LoginAsync(input.Username, input.Password, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WhenServiceThrowsUnauthorizedAccessException_ShouldPropagateException()
    {
        // Arrange
        var input = new LoginInput { Username = "testuser", Password = "wrongpassword" };

        _cognitoAuthServiceMock
            .Setup(x => x.LoginAsync(input.Username, input.Password, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new UnauthorizedAccessException("Credenciais inválidas"));

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => _useCase.ExecuteAsync(input));

        _cognitoAuthServiceMock.Verify(
            x => x.LoginAsync(input.Username, input.Password, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WhenCancellationTokenIsCancelled_ShouldPropagateCancellation()
    {
        // Arrange
        var input = new LoginInput { Username = "testuser", Password = "password123" };
        var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        _cognitoAuthServiceMock
            .Setup(x => x.LoginAsync(input.Username, input.Password, cancellationTokenSource.Token))
            .ThrowsAsync(new OperationCanceledException());

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(
            () => _useCase.ExecuteAsync(input, cancellationTokenSource.Token));
    }
}
