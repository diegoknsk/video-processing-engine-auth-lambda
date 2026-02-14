using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace VideoProcessing.Auth.Api.OpenApi;

/// <summary>
/// Preenche o server do OpenAPI a partir do request atual quando disponível (Scheme + Host + PathBase).
/// Assim o Scalar UI monta as URLs do "Try it" corretamente atrás do gateway sem precisar de API_PUBLIC_BASE_URL.
/// </summary>
public sealed class OpenApiServerFromRequestFilter(IHttpContextAccessor httpContextAccessor) : IDocumentFilter
{
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

        var serverUrl = $"{scheme}://{host}{pathBase}".TrimEnd('/');
        document.Servers.Clear();
        document.Servers.Add(new OpenApiServer
        {
            Url = serverUrl,
            Description = "API (derivado do request)"
        });
    }
}
