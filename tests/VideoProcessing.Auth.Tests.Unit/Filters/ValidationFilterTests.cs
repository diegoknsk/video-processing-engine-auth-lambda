using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using VideoProcessing.Auth.Api.Filters;

namespace VideoProcessing.Auth.Tests.Unit.Filters;

public class ValidationFilterTests
{
    private readonly ValidationFilter _sut = new();

    [Fact]
    public void OnActionExecuting_WhenModelStateIsValid_ShouldNotSetResult()
    {
        // Arrange
        var context = CreateActionExecutingContext(valid: true);

        // Act
        _sut.OnActionExecuting(context);

        // Assert
        context.Result.Should().BeNull();
    }

    [Fact]
    public void OnActionExecuting_WhenModelStateHasOneError_ShouldReturnBadRequest()
    {
        // Arrange
        var context = CreateActionExecutingContext(valid: false);
        context.ModelState.AddModelError("Email", "Email inválido");

        // Act
        _sut.OnActionExecuting(context);

        // Assert
        context.Result.Should().BeOfType<BadRequestObjectResult>();
        var badRequest = (BadRequestObjectResult)context.Result;
        badRequest.Value.Should().NotBeNull();

        var valueType = badRequest.Value!.GetType();
        var successProp = valueType.GetProperty("success");
        successProp.Should().NotBeNull();
        successProp!.GetValue(badRequest.Value).Should().Be(false);

        var errorsProp = valueType.GetProperty("errors");
        errorsProp.Should().NotBeNull();
        var errors = errorsProp!.GetValue(badRequest.Value!) as System.Collections.IEnumerable;
        errors.Should().NotBeNull();
        var errorsList = errors!.Cast<object>().ToList();
        errorsList.Should().HaveCount(1);

        var firstError = errorsList[0];
        var fieldProp = firstError.GetType().GetProperty("field");
        var messageProp = firstError.GetType().GetProperty("message");
        fieldProp.Should().NotBeNull();
        messageProp.Should().NotBeNull();
        fieldProp!.GetValue(firstError).Should().Be("Email");
        messageProp!.GetValue(firstError).Should().Be("Email inválido");
    }

    [Fact]
    public void OnActionExecuting_WhenModelStateHasMultipleErrors_ShouldReturnAllErrors()
    {
        // Arrange
        var context = CreateActionExecutingContext(valid: false);
        context.ModelState.AddModelError("Email", "Email inválido");
        context.ModelState.AddModelError("Password", "Senha é obrigatória");
        context.ModelState.AddModelError("Name", "Nome deve ter no mínimo 2 caracteres");

        // Act
        _sut.OnActionExecuting(context);

        // Assert
        context.Result.Should().BeOfType<BadRequestObjectResult>();
        var badRequest = (BadRequestObjectResult)context.Result;
        badRequest.Value.Should().NotBeNull();

        var valueType = badRequest.Value!.GetType();
        var errorsProp = valueType.GetProperty("errors");
        errorsProp.Should().NotBeNull();
        var errors = errorsProp!.GetValue(badRequest.Value!) as System.Collections.IEnumerable;
        errors.Should().NotBeNull();
        var errorsList = errors!.Cast<object>().ToList();
        errorsList.Should().HaveCount(3);

        var fields = errorsList
            .Select(e => e.GetType().GetProperty("field")!.GetValue(e)?.ToString())
            .ToList();
        var messages = errorsList
            .Select(e => e.GetType().GetProperty("message")!.GetValue(e)?.ToString())
            .ToList();
        fields.Should().Contain("Email").And.Contain("Password").And.Contain("Name");
        messages.Should().Contain("Email inválido").And.Contain("Senha é obrigatória").And.Contain("Nome deve ter no mínimo 2 caracteres");
    }

    [Fact]
    public void OnActionExecuted_ShouldDoNothing()
    {
        // Arrange
        var context = CreateActionExecutedContext();

        // Act
        var act = () => _sut.OnActionExecuted(context);

        // Assert
        act.Should().NotThrow();
        context.Result.Should().BeNull();
    }

    private static ActionExecutingContext CreateActionExecutingContext(bool valid)
    {
        var httpContext = new DefaultHttpContext();
        var routeData = new RouteData();
        var actionDescriptor = new ActionDescriptor();
        var actionContext = new ActionContext(httpContext, routeData, actionDescriptor);
        var context = new ActionExecutingContext(
            actionContext,
            new List<IFilterMetadata>(),
            new Dictionary<string, object?>(),
            null!);
        if (!valid)
        {
            // ModelState inválido: não alteramos, o dictionary já está vazio e IsValid será true até adicionarmos erros
            // então adicionamos um erro no teste que precisa de erros
        }
        return context;
    }

    private static ActionExecutedContext CreateActionExecutedContext()
    {
        var httpContext = new DefaultHttpContext();
        var routeData = new RouteData();
        var actionDescriptor = new ActionDescriptor();
        var actionContext = new ActionContext(httpContext, routeData, actionDescriptor);
        return new ActionExecutedContext(actionContext, new List<IFilterMetadata>(), null!)
        {
            Result = null
        };
    }
}
