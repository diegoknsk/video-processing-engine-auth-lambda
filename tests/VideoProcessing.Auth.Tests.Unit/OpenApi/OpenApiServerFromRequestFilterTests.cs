using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.OpenApi.Models;
using Moq;
using Swashbuckle.AspNetCore.SwaggerGen;
using VideoProcessing.Auth.Api.OpenApi;

namespace VideoProcessing.Auth.Tests.Unit.OpenApi;

[Collection("EnvVar")]
public class OpenApiServerFromRequestFilterTests
{
    private const string GatewayStageKey = "GATEWAY_STAGE";

    [Fact]
    public void Apply_WhenHttpContextIsNull_ShouldNotAddServer()
    {
        // Arrange
        var accessorMock = new Mock<IHttpContextAccessor>();
        accessorMock.Setup(x => x.HttpContext).Returns((HttpContext?)null);
        var filter = new OpenApiServerFromRequestFilter(accessorMock.Object);
        var document = new OpenApiDocument { Servers = new List<OpenApiServer>() };

        // Act
        filter.Apply(document, CreateFilterContext());

        // Assert
        document.Servers.Should().BeEmpty();
    }

    [Fact]
    public void Apply_WhenRequestIsNull_ShouldNotAddServer()
    {
        // Arrange
        var httpContextMock = new Mock<HttpContext>();
#pragma warning disable CS8625
        httpContextMock.Setup(x => x.Request).Returns((HttpRequest?)null);
#pragma warning restore CS8625
        var accessorMock = new Mock<IHttpContextAccessor>();
        accessorMock.Setup(x => x.HttpContext).Returns(httpContextMock.Object);
        var filter = new OpenApiServerFromRequestFilter(accessorMock.Object);
        var document = new OpenApiDocument { Servers = new List<OpenApiServer>() };

        // Act
        filter.Apply(document, CreateFilterContext());

        // Assert
        document.Servers.Should().BeEmpty();
    }

    [Fact]
    public void Apply_WhenHostIsEmpty_ShouldNotAddServer()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Request.Scheme = "https";
        context.Request.Host = new HostString("");
        context.Request.PathBase = PathString.Empty;
        var accessorMock = new Mock<IHttpContextAccessor>();
        accessorMock.Setup(x => x.HttpContext).Returns(context);
        var filter = new OpenApiServerFromRequestFilter(accessorMock.Object);
        var document = new OpenApiDocument { Servers = new List<OpenApiServer>() };

        // Act
        filter.Apply(document, CreateFilterContext());

        // Assert
        document.Servers.Should().BeEmpty();
    }

    [Fact]
    public void Apply_WhenNoStageAndNoPathBase_ShouldBuildSimpleUrl()
    {
        // Arrange
        UnsetStage();
        var context = new DefaultHttpContext();
        context.Request.Scheme = "https";
        context.Request.Host = new HostString("api.example.com");
        context.Request.PathBase = PathString.Empty;
        var accessorMock = new Mock<IHttpContextAccessor>();
        accessorMock.Setup(x => x.HttpContext).Returns(context);
        var filter = new OpenApiServerFromRequestFilter(accessorMock.Object);
        var document = new OpenApiDocument { Servers = new List<OpenApiServer>() };

        try
        {
            // Act
            filter.Apply(document, CreateFilterContext());

            // Assert
            document.Servers.Should().HaveCount(1);
            document.Servers[0].Url.Should().Be("https://api.example.com");
            document.Servers[0].Description.Should().Be("API (derivado do request)");
        }
        finally
        {
            UnsetStage();
        }
    }

    [Fact]
    public void Apply_WhenStageIsSet_ShouldIncludeStageInUrl()
    {
        // Arrange
        SetStage("dev");
        var context = new DefaultHttpContext();
        context.Request.Scheme = "https";
        context.Request.Host = new HostString("api.example.com");
        context.Request.PathBase = PathString.Empty;
        var accessorMock = new Mock<IHttpContextAccessor>();
        accessorMock.Setup(x => x.HttpContext).Returns(context);
        var filter = new OpenApiServerFromRequestFilter(accessorMock.Object);
        var document = new OpenApiDocument { Servers = new List<OpenApiServer>() };

        try
        {
            // Act
            filter.Apply(document, CreateFilterContext());

            // Assert
            document.Servers.Should().HaveCount(1);
            document.Servers[0].Url.Should().Be("https://api.example.com/dev");
        }
        finally
        {
            UnsetStage();
        }
    }

    [Fact]
    public void Apply_WhenPathBaseIsSet_ShouldIncludePathBase()
    {
        // Arrange
        UnsetStage();
        var context = new DefaultHttpContext();
        context.Request.Scheme = "https";
        context.Request.Host = new HostString("api.example.com");
        context.Request.PathBase = new PathString("/auth");
        var accessorMock = new Mock<IHttpContextAccessor>();
        accessorMock.Setup(x => x.HttpContext).Returns(context);
        var filter = new OpenApiServerFromRequestFilter(accessorMock.Object);
        var document = new OpenApiDocument { Servers = new List<OpenApiServer>() };

        try
        {
            // Act
            filter.Apply(document, CreateFilterContext());

            // Assert
            document.Servers.Should().HaveCount(1);
            document.Servers[0].Url.Should().Be("https://api.example.com/auth");
        }
        finally
        {
            UnsetStage();
        }
    }

    [Fact]
    public void Apply_WhenStageAndPathBaseAreSet_ShouldCombineCorrectly()
    {
        // Arrange
        SetStage("dev");
        var context = new DefaultHttpContext();
        context.Request.Scheme = "https";
        context.Request.Host = new HostString("api.example.com");
        context.Request.PathBase = new PathString("/auth");
        var accessorMock = new Mock<IHttpContextAccessor>();
        accessorMock.Setup(x => x.HttpContext).Returns(context);
        var filter = new OpenApiServerFromRequestFilter(accessorMock.Object);
        var document = new OpenApiDocument { Servers = new List<OpenApiServer>() };

        try
        {
            // Act
            filter.Apply(document, CreateFilterContext());

            // Assert
            document.Servers.Should().HaveCount(1);
            document.Servers[0].Url.Should().Be("https://api.example.com/dev/auth");
        }
        finally
        {
            UnsetStage();
        }
    }

    [Fact]
    public void Apply_WhenStageHasLeadingSlash_ShouldNormalizeIt()
    {
        // Arrange
        SetStage("/dev");
        var context = new DefaultHttpContext();
        context.Request.Scheme = "https";
        context.Request.Host = new HostString("host.example.com");
        context.Request.PathBase = PathString.Empty;
        var accessorMock = new Mock<IHttpContextAccessor>();
        accessorMock.Setup(x => x.HttpContext).Returns(context);
        var filter = new OpenApiServerFromRequestFilter(accessorMock.Object);
        var document = new OpenApiDocument { Servers = new List<OpenApiServer>() };

        try
        {
            // Act
            filter.Apply(document, CreateFilterContext());

            // Assert - não deve duplicar barra
            document.Servers.Should().HaveCount(1);
            document.Servers[0].Url.Should().Be("https://host.example.com/dev");
        }
        finally
        {
            UnsetStage();
        }
    }

    private static DocumentFilterContext CreateFilterContext()
    {
        var apiDescriptions = Array.Empty<Microsoft.AspNetCore.Mvc.ApiExplorer.ApiDescription>();
        var schemaGenerator = new Mock<Swashbuckle.AspNetCore.SwaggerGen.ISchemaGenerator>().Object;
        var schemaRepository = new Swashbuckle.AspNetCore.SwaggerGen.SchemaRepository();
        return new DocumentFilterContext(apiDescriptions, schemaGenerator, schemaRepository);
    }

    private static void SetStage(string value)
    {
        Environment.SetEnvironmentVariable(GatewayStageKey, value);
    }

    private static void UnsetStage()
    {
        Environment.SetEnvironmentVariable(GatewayStageKey, null);
    }
}
