using System.Text;
using System.Text.Json;
using Amazon.CognitoIdentityProvider.Model;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using VideoProcessing.Auth.Api.Middleware;
using VideoProcessing.Auth.Api.Models;

namespace VideoProcessing.Auth.Tests.Unit.Middleware;

public class GlobalExceptionMiddlewareTests
{
    private readonly Mock<ILogger<GlobalExceptionMiddleware>> _loggerMock;
    private readonly Mock<RequestDelegate> _nextMock;
    private readonly GlobalExceptionMiddleware _middleware;

    public GlobalExceptionMiddlewareTests()
    {
        _loggerMock = new Mock<ILogger<GlobalExceptionMiddleware>>();
        _nextMock = new Mock<RequestDelegate>();
        _middleware = new GlobalExceptionMiddleware(_nextMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task InvokeAsync_WhenNotAuthorizedException_ShouldReturn401WithInvalidCredentials()
    {
        // Arrange
        var context = CreateHttpContext();
        _nextMock.Setup(x => x(It.IsAny<HttpContext>())).ThrowsAsync(new NotAuthorizedException("Invalid credentials"));

        // Act
        await _middleware.InvokeAsync(context);

        // Assert
        context.Response.StatusCode.Should().Be(StatusCodes.Status401Unauthorized);
        var errorResponse = await DeserializeErrorResponse(context);
        errorResponse.Success.Should().BeFalse();
        errorResponse.Error.Code.Should().Be("InvalidCredentials");
        errorResponse.Error.Message.Should().Be("Credenciais inválidas.");
    }

    [Fact]
    public async Task InvokeAsync_WhenUserNotFoundException_ShouldReturn401WithInvalidCredentials()
    {
        // Arrange
        var context = CreateHttpContext();
        _nextMock.Setup(x => x(It.IsAny<HttpContext>())).ThrowsAsync(new UserNotFoundException("User not found"));

        // Act
        await _middleware.InvokeAsync(context);

        // Assert
        context.Response.StatusCode.Should().Be(StatusCodes.Status401Unauthorized);
        var errorResponse = await DeserializeErrorResponse(context);
        errorResponse.Success.Should().BeFalse();
        errorResponse.Error.Code.Should().Be("InvalidCredentials");
        errorResponse.Error.Message.Should().Be("Credenciais inválidas.");
    }

    [Fact]
    public async Task InvokeAsync_WhenUsernameExistsException_ShouldReturn409WithUserAlreadyExists()
    {
        // Arrange
        var context = CreateHttpContext();
        _nextMock.Setup(x => x(It.IsAny<HttpContext>())).ThrowsAsync(new UsernameExistsException("User already exists"));

        // Act
        await _middleware.InvokeAsync(context);

        // Assert
        context.Response.StatusCode.Should().Be(StatusCodes.Status409Conflict);
        var errorResponse = await DeserializeErrorResponse(context);
        errorResponse.Success.Should().BeFalse();
        errorResponse.Error.Code.Should().Be("UserAlreadyExists");
        errorResponse.Error.Message.Should().Be("Usuário já existe.");
    }

    [Fact]
    public async Task InvokeAsync_WhenInvalidPasswordException_ShouldReturn422WithInvalidPassword()
    {
        // Arrange
        var context = CreateHttpContext();
        _nextMock.Setup(x => x(It.IsAny<HttpContext>())).ThrowsAsync(new InvalidPasswordException("Password does not meet requirements"));

        // Act
        await _middleware.InvokeAsync(context);

        // Assert
        context.Response.StatusCode.Should().Be(StatusCodes.Status422UnprocessableEntity);
        var errorResponse = await DeserializeErrorResponse(context);
        errorResponse.Success.Should().BeFalse();
        errorResponse.Error.Code.Should().Be("InvalidPassword");
        errorResponse.Error.Message.Should().Be("Senha não atende aos requisitos de política.");
    }

    [Fact]
    public async Task InvokeAsync_WhenTooManyRequestsException_ShouldReturn429WithTooManyRequests()
    {
        // Arrange
        var context = CreateHttpContext();
        _nextMock.Setup(x => x(It.IsAny<HttpContext>())).ThrowsAsync(new TooManyRequestsException("Too many requests"));

        // Act
        await _middleware.InvokeAsync(context);

        // Assert
        context.Response.StatusCode.Should().Be(StatusCodes.Status429TooManyRequests);
        var errorResponse = await DeserializeErrorResponse(context);
        errorResponse.Success.Should().BeFalse();
        errorResponse.Error.Code.Should().Be("TooManyRequests");
        errorResponse.Error.Message.Should().Be("Limite de requisições excedido.");
    }

    [Fact]
    public async Task InvokeAsync_WhenInvalidParameterException_ShouldReturn400WithInvalidParameter()
    {
        // Arrange
        var context = CreateHttpContext();
        _nextMock.Setup(x => x(It.IsAny<HttpContext>())).ThrowsAsync(new InvalidParameterException("Invalid parameter"));

        // Act
        await _middleware.InvokeAsync(context);

        // Assert
        context.Response.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        var errorResponse = await DeserializeErrorResponse(context);
        errorResponse.Success.Should().BeFalse();
        errorResponse.Error.Code.Should().Be("InvalidParameter");
        errorResponse.Error.Message.Should().Be("Parâmetro inválido.");
    }

    [Fact]
    public async Task InvokeAsync_WhenUnauthorizedAccessException_ShouldReturn401WithUnauthorized()
    {
        // Arrange
        var context = CreateHttpContext();
        _nextMock.Setup(x => x(It.IsAny<HttpContext>())).ThrowsAsync(new UnauthorizedAccessException("Access denied"));

        // Act
        await _middleware.InvokeAsync(context);

        // Assert
        context.Response.StatusCode.Should().Be(StatusCodes.Status401Unauthorized);
        var errorResponse = await DeserializeErrorResponse(context);
        errorResponse.Success.Should().BeFalse();
        errorResponse.Error.Code.Should().Be("Unauthorized");
        errorResponse.Error.Message.Should().Be("Acesso não autorizado.");
    }

    [Fact]
    public async Task InvokeAsync_WhenArgumentException_ShouldReturn400WithBadRequest()
    {
        // Arrange
        var context = CreateHttpContext();
        _nextMock.Setup(x => x(It.IsAny<HttpContext>())).ThrowsAsync(new ArgumentException("Invalid argument"));

        // Act
        await _middleware.InvokeAsync(context);

        // Assert
        context.Response.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        var errorResponse = await DeserializeErrorResponse(context);
        errorResponse.Success.Should().BeFalse();
        errorResponse.Error.Code.Should().Be("BadRequest");
        errorResponse.Error.Message.Should().Be("Requisição inválida.");
    }

    [Fact]
    public async Task InvokeAsync_WhenGenericException_ShouldReturn500WithInternalServerError()
    {
        // Arrange
        var context = CreateHttpContext();
        _nextMock.Setup(x => x(It.IsAny<HttpContext>())).ThrowsAsync(new Exception("Unexpected error"));

        // Act
        await _middleware.InvokeAsync(context);

        // Assert
        context.Response.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        var errorResponse = await DeserializeErrorResponse(context);
        errorResponse.Success.Should().BeFalse();
        errorResponse.Error.Code.Should().Be("InternalServerError");
        errorResponse.Error.Message.Should().Be("Erro interno do servidor.");
    }

    [Fact]
    public async Task InvokeAsync_WhenNoException_ShouldNotModifyResponse()
    {
        // Arrange
        var context = CreateHttpContext();
        _nextMock.Setup(x => x(It.IsAny<HttpContext>())).Returns(Task.CompletedTask);

        // Act
        await _middleware.InvokeAsync(context);

        // Assert
        context.Response.StatusCode.Should().Be(200); // Default status code
        context.Response.Body.Length.Should().Be(0); // No body written
    }

    [Fact]
    public async Task InvokeAsync_WhenExceptionOccurs_ShouldLogError()
    {
        // Arrange
        var context = CreateHttpContext();
        var exception = new Exception("Test exception");
        _nextMock.Setup(x => x(It.IsAny<HttpContext>())).ThrowsAsync(exception);

        // Act
        await _middleware.InvokeAsync(context);

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                exception,
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)),
            Times.Once);
    }

    private static HttpContext CreateHttpContext()
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        return context;
    }

    private static async Task<ApiErrorResponse> DeserializeErrorResponse(HttpContext context)
    {
        context.Response.Body.Position = 0;
        var reader = new StreamReader(context.Response.Body, Encoding.UTF8);
        var json = await reader.ReadToEndAsync();
        
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        
        return JsonSerializer.Deserialize<ApiErrorResponse>(json, options)!;
    }
}
