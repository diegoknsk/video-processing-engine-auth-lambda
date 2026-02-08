using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Moq;
using VideoProcessing.Auth.Api.Filters;
using VideoProcessing.Auth.Api.Models;

namespace VideoProcessing.Auth.Tests.Unit.Filters;

public class ApiResponseFilterTests
{
    private readonly Mock<ILogger<ApiResponseFilter>> _loggerMock;
    private readonly ApiResponseFilter _filter;

    public ApiResponseFilterTests()
    {
        _loggerMock = new Mock<ILogger<ApiResponseFilter>>();
        _filter = new ApiResponseFilter(_loggerMock.Object);
    }

    [Fact]
    public void OnActionExecuted_WhenResultIsOkObjectResult_ShouldWrapInApiResponse()
    {
        // Arrange
        var testData = new { id = 1, name = "Test" };
        var okResult = new OkObjectResult(testData);
        var context = CreateActionExecutedContext(okResult);

        // Act
        _filter.OnActionExecuted(context);

        // Assert
        context.Result.Should().BeOfType<ObjectResult>();
        var objectResult = context.Result as ObjectResult;
        objectResult!.StatusCode.Should().Be(StatusCodes.Status200OK);
        objectResult.Value.Should().BeOfType<ApiResponse<object>>();
        
        var apiResponse = objectResult.Value as ApiResponse<object>;
        apiResponse!.Success.Should().BeTrue();
        apiResponse.Data.Should().BeEquivalentTo(testData);
        apiResponse.Timestamp.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void OnActionExecuted_WhenResultIsObjectResultWithStatusCode200_ShouldWrapInApiResponse()
    {
        // Arrange
        var testData = new { id = 2, name = "Test2" };
        var objectResult = new ObjectResult(testData) { StatusCode = StatusCodes.Status200OK };
        var context = CreateActionExecutedContext(objectResult);

        // Act
        _filter.OnActionExecuted(context);

        // Assert
        context.Result.Should().BeOfType<ObjectResult>();
        var result = context.Result as ObjectResult;
        result!.StatusCode.Should().Be(StatusCodes.Status200OK);
        result.Value.Should().BeOfType<ApiResponse<object>>();
        
        var apiResponse = result.Value as ApiResponse<object>;
        apiResponse!.Success.Should().BeTrue();
        apiResponse.Data.Should().BeEquivalentTo(testData);
    }

    [Fact]
    public void OnActionExecuted_WhenResultIsObjectResultWithStatusCode201_ShouldWrapInApiResponse()
    {
        // Arrange
        var testData = new { userId = "123", username = "testuser" };
        var objectResult = new ObjectResult(testData) { StatusCode = StatusCodes.Status201Created };
        var context = CreateActionExecutedContext(objectResult);

        // Act
        _filter.OnActionExecuted(context);

        // Assert
        context.Result.Should().BeOfType<ObjectResult>();
        var result = context.Result as ObjectResult;
        result!.StatusCode.Should().Be(StatusCodes.Status201Created);
        result.Value.Should().BeOfType<ApiResponse<object>>();
        
        var apiResponse = result.Value as ApiResponse<object>;
        apiResponse!.Success.Should().BeTrue();
        apiResponse.Data.Should().BeEquivalentTo(testData);
    }

    [Fact]
    public void OnActionExecuted_WhenResultIsBadRequestResult_ShouldNotModify()
    {
        // Arrange
        var badRequestResult = new BadRequestResult();
        var context = CreateActionExecutedContext(badRequestResult);

        // Act
        _filter.OnActionExecuted(context);

        // Assert
        context.Result.Should().BeOfType<BadRequestResult>();
        context.Result.Should().Be(badRequestResult);
    }

    [Fact]
    public void OnActionExecuted_WhenResultIsObjectResultWithStatusCode400_ShouldNotModify()
    {
        // Arrange
        var objectResult = new ObjectResult(new { error = "Bad request" }) { StatusCode = StatusCodes.Status400BadRequest };
        var context = CreateActionExecutedContext(objectResult);

        // Act
        _filter.OnActionExecuted(context);

        // Assert
        context.Result.Should().BeOfType<ObjectResult>();
        var result = context.Result as ObjectResult;
        result!.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        result.Value.Should().NotBeOfType<ApiResponse<object>>();
    }

    [Fact]
    public void OnActionExecuted_WhenResultIsNoContentResult_ShouldNotModify()
    {
        // Arrange
        var noContentResult = new NoContentResult();
        var context = CreateActionExecutedContext(noContentResult);

        // Act
        _filter.OnActionExecuted(context);

        // Assert
        context.Result.Should().BeOfType<NoContentResult>();
        context.Result.Should().Be(noContentResult);
    }

    private static ActionExecutedContext CreateActionExecutedContext(IActionResult result)
    {
        var httpContext = new DefaultHttpContext();
        var actionDescriptor = new ActionDescriptor
        {
            DisplayName = "TestController.TestAction"
        };
        var routeData = new RouteData();
        var actionContext = new ActionContext(httpContext, routeData, actionDescriptor);
        
        return new ActionExecutedContext(actionContext, new List<IFilterMetadata>(), null!)
        {
            Result = result
        };
    }
}
