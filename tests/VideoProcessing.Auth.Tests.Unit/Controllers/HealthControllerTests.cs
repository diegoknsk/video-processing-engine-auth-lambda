using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VideoProcessing.Auth.Api.Controllers;

namespace VideoProcessing.Auth.Tests.Unit.Controllers;

public class HealthControllerTests
{
    private readonly HealthController _controller;

    public HealthControllerTests()
    {
        _controller = new HealthController();
    }

    [Fact]
    public void Health_ShouldReturnOkObjectResult()
    {
        // Act
        var result = _controller.Health();

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.StatusCode.Should().Be(StatusCodes.Status200OK);
    }

    [Fact]
    public void Health_ShouldReturnStatusHealthy()
    {
        // Act
        var result = _controller.Health();

        // Assert
        var okResult = result as OkObjectResult;
        var value = okResult!.Value;
        
        value.Should().NotBeNull();
        var statusProperty = value!.GetType().GetProperty("status");
        statusProperty.Should().NotBeNull();
        var statusValue = statusProperty!.GetValue(value);
        statusValue.Should().Be("Healthy");
    }

    [Fact]
    public void Health_ShouldReturnTimestamp()
    {
        // Act
        var result = _controller.Health();

        // Assert
        var okResult = result as OkObjectResult;
        var value = okResult!.Value;
        
        value.Should().NotBeNull();
        var timestampProperty = value!.GetType().GetProperty("timestamp");
        timestampProperty.Should().NotBeNull();
        var timestampValue = timestampProperty!.GetValue(value);
        timestampValue.Should().BeOfType<DateTime>();
        
        var timestamp = (DateTime)timestampValue!;
        timestamp.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }
}
