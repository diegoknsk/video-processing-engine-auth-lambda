using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using VideoProcessing.Auth.Api.Models;

namespace VideoProcessing.Auth.Api.Filters;

/// <summary>
/// Filtro global que encapsula automaticamente respostas de sucesso (200, 201) em ApiResponse&lt;T&gt;.
/// </summary>
public class ApiResponseFilter : IActionFilter
{
    private readonly ILogger<ApiResponseFilter> _logger;

    public ApiResponseFilter(ILogger<ApiResponseFilter> logger)
    {
        _logger = logger;
    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        // Não há ação necessária antes da execução
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        // Aplica o filtro apenas para respostas de sucesso (200, 201)
        if (context.Result is OkObjectResult okResult)
        {
            var apiResponse = ApiResponse<object>.CreateSuccess(okResult.Value!);
            context.Result = new ObjectResult(apiResponse)
            {
                StatusCode = StatusCodes.Status200OK
            };

            _logger.LogDebug("ApiResponseFilter applied to action {ActionName}", context.ActionDescriptor.DisplayName);
        }
        else if (context.Result is ObjectResult objectResult && 
                 (objectResult.StatusCode == StatusCodes.Status200OK || 
                  objectResult.StatusCode == StatusCodes.Status201Created))
        {
            var apiResponse = ApiResponse<object>.CreateSuccess(objectResult.Value!);
            context.Result = new ObjectResult(apiResponse)
            {
                StatusCode = objectResult.StatusCode
            };

            _logger.LogDebug("ApiResponseFilter applied to action {ActionName} with status {StatusCode}", 
                context.ActionDescriptor.DisplayName, objectResult.StatusCode);
        }
        // Outras respostas (400, 401, etc.) não são modificadas - serão tratadas pelo middleware de exceções
    }
}
