using System.Net;
using System.Text.Json;
using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Microsoft.AspNetCore.Http;
using VideoProcessing.Auth.Api.Models;

namespace VideoProcessing.Auth.Api.Middleware;

/// <summary>
/// Middleware global para capturar e tratar exceções não tratadas, mapeando-as para respostas HTTP apropriadas.
/// </summary>
public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception: {ExceptionType} - {Message}", ex.GetType().Name, ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, code, message) = MapException(exception);

        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";

        var errorResponse = ApiErrorResponse.Create(code, message);

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        };

        var json = JsonSerializer.Serialize(errorResponse, options);
        await context.Response.WriteAsync(json);
    }

    private static (int statusCode, string code, string message) MapException(Exception exception)
    {
        return exception switch
        {
            // Exceções do Cognito
            NotAuthorizedException => (StatusCodes.Status401Unauthorized, "InvalidCredentials", "Credenciais inválidas."),
            UserNotFoundException => (StatusCodes.Status401Unauthorized, "InvalidCredentials", "Credenciais inválidas."),
            UserNotConfirmedException => (StatusCodes.Status403Forbidden, "UserNotConfirmed", "Conta não confirmada. Verifique seu e-mail e confirme o cadastro."),
            UsernameExistsException => (StatusCodes.Status409Conflict, "UserAlreadyExists", "Usuário já existe."),
            InvalidPasswordException => (StatusCodes.Status422UnprocessableEntity, "InvalidPassword", "Senha não atende aos requisitos de política."),
            TooManyRequestsException => (StatusCodes.Status429TooManyRequests, "TooManyRequests", "Limite de requisições excedido."),
            InvalidParameterException => (StatusCodes.Status400BadRequest, "InvalidParameter", "Parâmetro inválido."),
            AmazonCognitoIdentityProviderException => (StatusCodes.Status502BadGateway, "ExternalServiceError", "Erro ao comunicar com serviço de autenticação."),

            // Exceções .NET
            UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, "Unauthorized", "Acesso não autorizado."),
            ArgumentException => (StatusCodes.Status400BadRequest, "BadRequest", "Requisição inválida."),

            // Exceção genérica
            _ => (StatusCodes.Status500InternalServerError, "InternalServerError", "Erro interno do servidor.")
        };
    }
}
