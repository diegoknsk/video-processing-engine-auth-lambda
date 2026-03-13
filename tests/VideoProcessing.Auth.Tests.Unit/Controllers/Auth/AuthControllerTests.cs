using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using VideoProcessing.Auth.Api.Controllers.Auth;
using VideoProcessing.Auth.Application.InputModels.Auth;
using VideoProcessing.Auth.Application.OutputModels.Auth;
using VideoProcessing.Auth.Application.Ports;
using VideoProcessing.Auth.Application.ResponseModels.Auth;
using VideoProcessing.Auth.Application.UseCases.Auth;

namespace VideoProcessing.Auth.Tests.Unit.Controllers.Auth;

public class AuthControllerTests
{
    private readonly Mock<ICognitoAuthService> _cognitoMock;
    private readonly Mock<ILogger<LoginUseCase>> _useCaseLoggerMock;
    private readonly Mock<ILogger<AuthController>> _controllerLoggerMock;
    private readonly LoginUseCase _loginUseCase;
    private readonly AuthController _sut;

    public AuthControllerTests()
    {
        _cognitoMock = new Mock<ICognitoAuthService>();
        _useCaseLoggerMock = new Mock<ILogger<LoginUseCase>>();
        _controllerLoggerMock = new Mock<ILogger<AuthController>>();
        _loginUseCase = new LoginUseCase(_cognitoMock.Object, _useCaseLoggerMock.Object);
        _sut = new AuthController(_loginUseCase, _controllerLoggerMock.Object);
    }

    [Fact]
    public async Task Login_WhenInputIsValid_ShouldReturnOkWithResult()
    {
        // Arrange
        var input = new LoginInput { Email = "user@example.com", Password = "password123" };
        var loginOutput = new LoginOutput
        {
            AccessToken = "access",
            IdToken = "id",
            RefreshToken = "refresh",
            ExpiresIn = 3600,
            TokenType = "Bearer"
        };
        var expectedResponse = new LoginResponseModel
        {
            AccessToken = loginOutput.AccessToken,
            IdToken = loginOutput.IdToken,
            RefreshToken = loginOutput.RefreshToken,
            ExpiresIn = loginOutput.ExpiresIn,
            TokenType = loginOutput.TokenType
        };

        _cognitoMock
            .Setup(x => x.LoginAsync(input.Email, input.Password, It.IsAny<CancellationToken>()))
            .ReturnsAsync(loginOutput);

        // Act
        var result = await _sut.Login(input, CancellationToken.None);

        // Assert
        result.Should().BeOfType<Microsoft.AspNetCore.Mvc.OkObjectResult>();
        var okResult = (Microsoft.AspNetCore.Mvc.OkObjectResult)result;
        okResult.Value.Should().BeEquivalentTo(expectedResponse);
        _cognitoMock.Verify(
            x => x.LoginAsync(input.Email, input.Password, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Login_WhenUseCaseThrows_ShouldPropagateException()
    {
        // Arrange
        var input = new LoginInput { Email = "user@example.com", Password = "wrong" };
        _cognitoMock
            .Setup(x => x.LoginAsync(input.Email, input.Password, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new UnauthorizedAccessException("Credenciais inválidas"));

        // Act & Assert
        await _sut.Invoking(x => x.Login(input, CancellationToken.None))
            .Should()
            .ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("Credenciais inválidas");
        _cognitoMock.Verify(
            x => x.LoginAsync(input.Email, input.Password, It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
