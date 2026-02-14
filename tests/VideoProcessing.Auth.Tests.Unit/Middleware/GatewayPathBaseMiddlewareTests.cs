using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using VideoProcessing.Auth.Api.Middleware;

namespace VideoProcessing.Auth.Tests.Unit.Middleware;

public class GatewayPathBaseMiddlewareTests
{
    private const string GatewayPathPrefixKey = "GATEWAY_PATH_PREFIX";

    [Fact]
    public async Task InvokeAsync_WhenEnvNotSet_ShouldNotAlterPath()
    {
        var (context, nextMock) = CreateContextAndNext("/auth/health");
        UnsetEnv();
        try
        {
            var middleware = new GatewayPathBaseMiddleware(nextMock.Object, Mock.Of<ILogger<GatewayPathBaseMiddleware>>());

            await middleware.InvokeAsync(context);

            context.Request.PathBase.Should().Be(PathString.Empty);
            context.Request.Path.Should().Be(new PathString("/auth/health"));
            nextMock.Verify(x => x(It.IsAny<HttpContext>()), Times.Once);
        }
        finally
        {
            Environment.SetEnvironmentVariable(GatewayPathPrefixKey, null);
        }
    }

    [Fact]
    public async Task InvokeAsync_WhenEnvEmpty_ShouldNotAlterPath()
    {
        var (context, nextMock) = CreateContextAndNext("/auth/health");
        SetEnv("");
        try
        {
            var middleware = new GatewayPathBaseMiddleware(nextMock.Object, Mock.Of<ILogger<GatewayPathBaseMiddleware>>());

            await middleware.InvokeAsync(context);

            context.Request.PathBase.Should().Be(PathString.Empty);
            context.Request.Path.Should().Be(new PathString("/auth/health"));
            nextMock.Verify(x => x(It.IsAny<HttpContext>()), Times.Once);
        }
        finally
        {
            Environment.SetEnvironmentVariable(GatewayPathPrefixKey, null);
        }
    }

    [Fact]
    public async Task InvokeAsync_WhenPrefixSetAndPathStartsWithPrefix_ShouldSetPathBaseAndPath()
    {
        var (context, nextMock) = CreateContextAndNext("/auth/health");
        SetEnv("/auth");
        try
        {
            var middleware = new GatewayPathBaseMiddleware(nextMock.Object, Mock.Of<ILogger<GatewayPathBaseMiddleware>>());

            await middleware.InvokeAsync(context);

            context.Request.PathBase.Should().Be(new PathString("/auth"));
            context.Request.Path.Should().Be(new PathString("/health"));
            nextMock.Verify(x => x(It.IsAny<HttpContext>()), Times.Once);
        }
        finally
        {
            Environment.SetEnvironmentVariable(GatewayPathPrefixKey, null);
        }
    }

    [Fact]
    public async Task InvokeAsync_WhenPrefixSetAndPathWithDifferentCasing_ShouldStripCaseInsensitive()
    {
        var (context, nextMock) = CreateContextAndNext("/Auth/login");
        SetEnv("/auth");
        try
        {
            var middleware = new GatewayPathBaseMiddleware(nextMock.Object, Mock.Of<ILogger<GatewayPathBaseMiddleware>>());

            await middleware.InvokeAsync(context);

            context.Request.PathBase.Should().Be(new PathString("/Auth"));
            context.Request.Path.Should().Be(new PathString("/login"));
            nextMock.Verify(x => x(It.IsAny<HttpContext>()), Times.Once);
        }
        finally
        {
            Environment.SetEnvironmentVariable(GatewayPathPrefixKey, null);
        }
    }

    [Fact]
    public async Task InvokeAsync_WhenPrefixSetAndPathAllUpperCase_ShouldStripCaseInsensitive()
    {
        var (context, nextMock) = CreateContextAndNext("/AUTH/users/create");
        SetEnv("/auth");
        try
        {
            var middleware = new GatewayPathBaseMiddleware(nextMock.Object, Mock.Of<ILogger<GatewayPathBaseMiddleware>>());

            await middleware.InvokeAsync(context);

            context.Request.PathBase.Should().Be(new PathString("/AUTH"));
            context.Request.Path.Should().Be(new PathString("/users/create"));
            nextMock.Verify(x => x(It.IsAny<HttpContext>()), Times.Once);
        }
        finally
        {
            Environment.SetEnvironmentVariable(GatewayPathPrefixKey, null);
        }
    }

    [Fact]
    public async Task InvokeAsync_WhenPrefixSetAndPathDoesNotStartWithPrefix_ShouldNotAlterPath()
    {
        var (context, nextMock) = CreateContextAndNext("/other/health");
        SetEnv("/auth");
        try
        {
            var middleware = new GatewayPathBaseMiddleware(nextMock.Object, Mock.Of<ILogger<GatewayPathBaseMiddleware>>());

            await middleware.InvokeAsync(context);

            context.Request.PathBase.Should().Be(PathString.Empty);
            context.Request.Path.Should().Be(new PathString("/other/health"));
            nextMock.Verify(x => x(It.IsAny<HttpContext>()), Times.Once);
        }
        finally
        {
            Environment.SetEnvironmentVariable(GatewayPathPrefixKey, null);
        }
    }

    [Fact]
    public async Task InvokeAsync_WhenPrefixSetAndPathIsOnlyPrefix_ShouldSetPathToSlash()
    {
        var (context, nextMock) = CreateContextAndNext("/auth");
        SetEnv("/auth");
        try
        {
            var middleware = new GatewayPathBaseMiddleware(nextMock.Object, Mock.Of<ILogger<GatewayPathBaseMiddleware>>());

            await middleware.InvokeAsync(context);

            context.Request.PathBase.Should().Be(new PathString("/auth"));
            context.Request.Path.Should().Be(new PathString("/"));
            nextMock.Verify(x => x(It.IsAny<HttpContext>()), Times.Once);
        }
        finally
        {
            Environment.SetEnvironmentVariable(GatewayPathPrefixKey, null);
        }
    }

    [Fact]
    public async Task InvokeAsync_WhenPrefixSetWithoutLeadingSlash_ShouldNormalizeAndStrip()
    {
        var (context, nextMock) = CreateContextAndNext("/auth/health");
        SetEnv("auth");
        try
        {
            var middleware = new GatewayPathBaseMiddleware(nextMock.Object, Mock.Of<ILogger<GatewayPathBaseMiddleware>>());

            await middleware.InvokeAsync(context);

            context.Request.PathBase.Should().Be(new PathString("/auth"));
            context.Request.Path.Should().Be(new PathString("/health"));
            nextMock.Verify(x => x(It.IsAny<HttpContext>()), Times.Once);
        }
        finally
        {
            Environment.SetEnvironmentVariable(GatewayPathPrefixKey, null);
        }
    }

    private static (HttpContext context, Mock<RequestDelegate> nextMock) CreateContextAndNext(string path)
    {
        var context = new DefaultHttpContext();
        context.Request.Path = new PathString(path);
        var nextMock = new Mock<RequestDelegate>();
        nextMock.Setup(x => x(It.IsAny<HttpContext>())).Returns(Task.CompletedTask);
        return (context, nextMock);
    }

    private static void SetEnv(string value)
    {
        Environment.SetEnvironmentVariable(GatewayPathPrefixKey, value);
    }

    private static void UnsetEnv()
    {
        Environment.SetEnvironmentVariable(GatewayPathPrefixKey, null);
    }
}
