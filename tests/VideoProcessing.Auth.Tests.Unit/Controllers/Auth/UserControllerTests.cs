using Amazon.CognitoIdentityProvider.Model;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using VideoProcessing.Auth.Api.Controllers.Auth;
using VideoProcessing.Auth.Application.InputModels.Auth;
using VideoProcessing.Auth.Application.OutputModels.Auth;
using VideoProcessing.Auth.Application.Ports;
using VideoProcessing.Auth.Application.ResponseModels.Auth;
using VideoProcessing.Auth.Application.UseCases.Auth;

namespace VideoProcessing.Auth.Tests.Unit.Controllers.Auth;

public class UserControllerTests
{
    private readonly Mock<ICognitoAuthService> _cognitoMock;
    private readonly Mock<ILogger<CreateUserUseCase>> _useCaseLoggerMock;
    private readonly Mock<ILogger<UserController>> _controllerLoggerMock;
    private readonly CreateUserUseCase _createUserUseCase;
    private readonly UserController _sut;

    public UserControllerTests()
    {
        _cognitoMock = new Mock<ICognitoAuthService>();
        _useCaseLoggerMock = new Mock<ILogger<CreateUserUseCase>>();
        _controllerLoggerMock = new Mock<ILogger<UserController>>();
        _createUserUseCase = new CreateUserUseCase(_cognitoMock.Object, _useCaseLoggerMock.Object);
        _sut = new UserController(_createUserUseCase, _controllerLoggerMock.Object);
    }

    [Fact]
    public async Task Create_WhenInputIsValid_ShouldReturn201WithResult()
    {
        // Arrange
        var input = new CreateUserInput { Name = "Diego", Email = "user@example.com", Password = "password123" };
        var createOutput = new CreateUserOutput
        {
            UserId = "user-sub-123",
            Username = input.Email,
            UserConfirmed = true,
            ConfirmationRequired = false
        };

        _cognitoMock
            .Setup(x => x.SignUpAsync(input.Name, input.Email, input.Password, It.IsAny<CancellationToken>()))
            .ReturnsAsync(createOutput);

        // Act
        var result = await _sut.Create(input, CancellationToken.None);

        // Assert
        result.Should().BeOfType<ObjectResult>();
        var objectResult = (ObjectResult)result;
        objectResult.StatusCode.Should().Be(Microsoft.AspNetCore.Http.StatusCodes.Status201Created);
        objectResult.Value.Should().BeEquivalentTo(new CreateUserResponseModel
        {
            UserId = createOutput.UserId,
            Username = createOutput.Username,
            UserConfirmed = createOutput.UserConfirmed,
            ConfirmationRequired = createOutput.ConfirmationRequired
        });
        _cognitoMock.Verify(
            x => x.SignUpAsync(input.Name, input.Email, input.Password, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Create_WhenUseCaseThrows_ShouldPropagateException()
    {
        // Arrange
        var input = new CreateUserInput { Name = "Diego", Email = "existing@example.com", Password = "password123" };
        _cognitoMock
            .Setup(x => x.SignUpAsync(input.Name, input.Email, input.Password, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new UsernameExistsException("User already exists"));

        // Act & Assert
        await _sut.Invoking(x => x.Create(input, CancellationToken.None))
            .Should()
            .ThrowAsync<UsernameExistsException>();
        _cognitoMock.Verify(
            x => x.SignUpAsync(input.Name, input.Email, input.Password, It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
