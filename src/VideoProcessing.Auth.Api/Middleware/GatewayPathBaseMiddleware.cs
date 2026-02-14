using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace VideoProcessing.Auth.Api.Middleware;

/// <summary>
/// Middleware que remove o prefixo de path configurado em GATEWAY_PATH_PREFIX do request,
/// definindo PathBase e Path para que a aplicação seja agnóstica ao prefixo do API Gateway.
/// Quando a variável não está definida ou está vazia, o path não é alterado.
/// A comparação do prefixo é case-insensitive.
/// </summary>
public class GatewayPathBaseMiddleware(RequestDelegate next, ILogger<GatewayPathBaseMiddleware> logger)
{
    private const string GatewayPathPrefixKey = "GATEWAY_PATH_PREFIX";

    public Task InvokeAsync(HttpContext context)
    {
        var prefix = Environment.GetEnvironmentVariable(GatewayPathPrefixKey)?.Trim();
        if (string.IsNullOrEmpty(prefix))
        {
            return next(context);
        }

        var prefixNormalized = prefix.StartsWith('/') ? prefix : "/" + prefix;
        if (prefixNormalized.Length > 1 && prefixNormalized.EndsWith('/'))
        {
            prefixNormalized = prefixNormalized.TrimEnd('/');
        }

        var path = context.Request.Path.Value ?? "/";
        if (path.Length < prefixNormalized.Length)
        {
            return next(context);
        }

        var pathSegment = path.Length == prefixNormalized.Length
            ? path
            : path[..prefixNormalized.Length];
        var rest = path.Length == prefixNormalized.Length
            ? "/"
            : path[prefixNormalized.Length..];

        if (!pathSegment.Equals(prefixNormalized, StringComparison.OrdinalIgnoreCase))
        {
            return next(context);
        }

        if (path.Length > prefixNormalized.Length && path[prefixNormalized.Length] != '/')
        {
            return next(context);
        }

        context.Request.PathBase = new PathString(pathSegment);
        context.Request.Path = new PathString(rest);
        logger.LogInformation(
            "GATEWAY_PATH_PREFIX applied: original path {OriginalPath} -> PathBase={PathBase}, Path={Path}",
            path, pathSegment, rest);

        return next(context);
    }
}
