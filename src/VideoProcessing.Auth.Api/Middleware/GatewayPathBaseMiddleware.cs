using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace VideoProcessing.Auth.Api.Middleware;

/// <summary>
/// Middleware que remove o prefixo de path configurado em GATEWAY_PATH_PREFIX do request,
/// definindo PathBase e Path para que a aplicação seja agnóstica ao prefixo do API Gateway.
/// Quando a variável não está definida ou está vazia, o path não é alterado.
/// A comparação do prefixo é case-insensitive.
/// Opcionalmente remove o segmento de stage do API Gateway (GATEWAY_STAGE) quando o stage não é $default.
/// </summary>
public class GatewayPathBaseMiddleware(RequestDelegate next, ILogger<GatewayPathBaseMiddleware> logger)
{
    private const string GatewayPathPrefixKey = "GATEWAY_PATH_PREFIX";
    private const string GatewayStageKey = "GATEWAY_STAGE";

    public Task InvokeAsync(HttpContext context)
    {
        var path = context.Request.Path.Value ?? "/";

        // 1) Remover stage do path quando API Gateway usa stage nomeado (não $default).
        // Ex.: rawPath "/default/health" com GATEWAY_STAGE=default → path "/health".
        var stage = Environment.GetEnvironmentVariable(GatewayStageKey)?.Trim();
        if (!string.IsNullOrEmpty(stage))
        {
            var stageSegment = (stage.StartsWith('/') ? stage : "/" + stage).TrimEnd('/');
            if (stageSegment.Length > 1 && path.StartsWith(stageSegment, StringComparison.OrdinalIgnoreCase))
            {
                if (path.Length == stageSegment.Length || (path.Length > stageSegment.Length && path[stageSegment.Length] == '/'))
                {
                    path = path.Length == stageSegment.Length ? "/" : path[stageSegment.Length..];
                    context.Request.Path = new PathString(path);
                    logger.LogDebug(
                        "GATEWAY_STAGE applied: path after strip = {Path}",
                        path);
                }
            }
        }

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
