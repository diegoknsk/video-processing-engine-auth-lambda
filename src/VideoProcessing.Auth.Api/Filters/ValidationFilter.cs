using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace VideoProcessing.Auth.Api.Filters;

/// <summary>
/// Filtro global para validação de ModelState e conversão de erros para formato padronizado da API.
/// </summary>
public class ValidationFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ModelState.IsValid)
        {
            var errors = context.ModelState
                .Where(x => x.Value?.Errors.Count > 0)
                .SelectMany(x => x.Value!.Errors.Select(error => new
                {
                    field = x.Key,
                    message = error.ErrorMessage
                }))
                .ToList();

            context.Result = new BadRequestObjectResult(new
            {
                success = false,
                errors = errors
            });
        }
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        // Não há ação necessária após a execução
    }
}
