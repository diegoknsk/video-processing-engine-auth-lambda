using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace VideoProcessing.Auth.Api.OpenApi;

/// <summary>
/// Preenche o server do OpenAPI a partir do request (Scheme + Host + stage + PathBase).
/// O middleware define PathBase só com o prefixo (/auth); o stage (/dev) já foi removido do path.
/// Por isso lemos GATEWAY_STAGE e preprendemos ao path do server para o Scalar montar a URL correta.
/// </summary>
public sealed class OpenApiServerFromRequestFilter(IHttpContextAccessor httpContextAccessor) : IDocumentFilter
{
    private const string GatewayStageKey = "GATEWAY_STAGE";

    public void Apply(OpenApiDocument document, DocumentFilterContext context)
    {
        var httpContext = httpContextAccessor.HttpContext;
        if (httpContext?.Request == null)
            return;

        var request = httpContext.Request;
        var pathBase = request.PathBase.Value ?? "";
        var scheme = request.Scheme;
        var host = request.Host.Value ?? "";

        if (string.IsNullOrEmpty(host))
            return;

        // PathBase no request é só o prefixo (/auth); o stage (/dev) foi removido pelo middleware.
        // Incluímos o stage no server URL para o Scalar gerar .../dev/auth/login.
        var stage = Environment.GetEnvironmentVariable(GatewayStageKey)?.Trim();
        var stageSegment = string.IsNullOrEmpty(stage)
            ? ""
            : "/" + (stage.StartsWith('/') ? stage.TrimStart('/') : stage).TrimEnd('/');

        var pathPart = $"{stageSegment}{pathBase}".TrimEnd('/');
        if (!string.IsNullOrEmpty(pathPart) && !pathPart.StartsWith('/'))
            pathPart = "/" + pathPart;

        var serverUrl = $"{scheme}://{host}{pathPart}".TrimEnd('/');
        document.Servers.Clear();
        document.Servers.Add(new OpenApiServer
        {
            Url = serverUrl,
            Description = "API (derivado do request)"
        });
    }
}
