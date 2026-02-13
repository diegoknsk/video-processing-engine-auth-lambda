---
name: external-api-refit
description: Guia para integração com APIs externas usando Refit — type-safe, resilience patterns, HTTP/3 e tratamento de erros. Use quando a tarefa envolver APIs externas, HttpClient, Refit, consumir APIs ou integração com serviços externos.
---

# External API Integration — Refit

## Quando Usar Esta Skill

Use quando a tarefa envolver:
- Integração com **APIs externas**
- **Refit**, **HttpClient**, consumir APIs
- **Resilience patterns** (retry, circuit breaker, timeout)
- Palavras-chave: "API externa", "Refit", "HttpClient", "consumir API", "integração", "webhook"

## 1. Por Que Refit?

**Preferir Refit** para clientes HTTP de APIs externas:
- ✅ **Type-safe:** interfaces C# com atributos HTTP
- ✅ **Menos boilerplate:** não precisa serializar/deserializar manualmente
- ✅ **Fácil de mockar:** interfaces são fáceis de substituir em testes
- ✅ **Suporta resilience patterns** (Polly integrado)
- ✅ **Melhor manutenibilidade:** API contract explícito

## 2. Estrutura

```
<Projeto>.Infra/
  ExternalApis/
    <NomeApi>/
      Contracts/
        TokenRequest.cs
        TokenResponse.cs
        ErrorResponse.cs
      I<Nome>Api.cs              (interface Refit)
      <Nome>Service.cs           (wrapper/port implementation)
```

## 3. Exemplo Completo: API de Autenticação Externa

### Interface Refit

```csharp
using Refit;

public interface IExternalAuthApi
{
    [Post("/token")]
    [Headers("Content-Type: application/json")]
    Task<TokenResponse> GetTokenAsync(
        [Header("X-Api-Key")] string apiKey,
        [Header("X-Api-Secret")] string apiSecret,
        CancellationToken cancellationToken = default);

    [Post("/validate")]
    Task<ValidationResponse> ValidateTokenAsync(
        [Header("Authorization")] string bearerToken,
        [Body] ValidateTokenRequest request,
        CancellationToken cancellationToken = default);

    [Get("/users/{id}")]
    Task<UserResponse> GetUserAsync(
        string id,
        [Header("Authorization")] string bearerToken,
        CancellationToken cancellationToken = default);
}
```

### Contracts (Request/Response)

```csharp
public record TokenResponse(
    [property: JsonPropertyName("access_token")] string AccessToken,
    [property: JsonPropertyName("expires_in")] int ExpiresIn,
    [property: JsonPropertyName("token_type")] string TokenType
);

public record ValidateTokenRequest(
    [property: JsonPropertyName("token")] string Token
);

public record ValidationResponse(
    [property: JsonPropertyName("is_valid")] bool IsValid,
    [property: JsonPropertyName("user_id")] string? UserId
);

public record UserResponse(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("email")] string Email
);

public record ErrorResponse(
    [property: JsonPropertyName("error")] string Error,
    [property: JsonPropertyName("error_description")] string? ErrorDescription
);
```

### Service (Wrapper do Port)

```csharp
using Application.Ports;
using Refit;

public class ExternalAuthService(IExternalAuthApi api, ILogger<ExternalAuthService> logger) : IExternalAuthService
{
    public async Task<string> GetTokenAsync(string apiKey, string apiSecret, CancellationToken ct = default)
    {
        try
        {
            logger.LogInformation("Requesting token from external auth API");
            
            var response = await api.GetTokenAsync(apiKey, apiSecret, ct);
            
            logger.LogInformation("Token obtained successfully, expires in {ExpiresIn}s", response.ExpiresIn);
            
            return response.AccessToken;
        }
        catch (ApiException ex)
        {
            logger.LogError(ex, "Failed to get token: {StatusCode} - {Content}", ex.StatusCode, ex.Content);
            
            // Mapear erro da API externa para exceção do domínio
            throw ex.StatusCode switch
            {
                System.Net.HttpStatusCode.Unauthorized => new UnauthorizedAccessException("Invalid API credentials"),
                System.Net.HttpStatusCode.TooManyRequests => new InvalidOperationException("Rate limit exceeded"),
                _ => new InvalidOperationException($"External API error: {ex.StatusCode}")
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error calling external auth API");
            throw new InvalidOperationException("Failed to communicate with external auth API", ex);
        }
    }

    public async Task<bool> ValidateTokenAsync(string token, CancellationToken ct = default)
    {
        try
        {
            var request = new ValidateTokenRequest(token);
            var response = await api.ValidateTokenAsync($"Bearer {token}", request, ct);
            return response.IsValid;
        }
        catch (ApiException ex) when (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            return false;
        }
    }
}
```

## 4. Registro no DI com Resilience (.NET 8+)

```csharp
using Microsoft.Extensions.Http.Resilience;

// Configuração via Options
builder.Services.Configure<ExternalAuthApiOptions>(
    builder.Configuration.GetSection("ExternalAuthApi"));

// Registrar Refit Client com Resilience
builder.Services.AddRefitClient<IExternalAuthApi>()
    .ConfigureHttpClient((sp, client) =>
    {
        var options = sp.GetRequiredService<IOptions<ExternalAuthApiOptions>>().Value;
        client.BaseAddress = new Uri(options.BaseUrl);
        client.Timeout = TimeSpan.FromSeconds(options.TimeoutSeconds);
        client.DefaultRequestHeaders.Add("User-Agent", "MyApp/1.0");
    })
    .AddStandardResilienceHandler(options =>
    {
        // Retry
        options.Retry.MaxRetryAttempts = 3;
        options.Retry.BackoffType = Polly.DelayBackoffType.Exponential;
        options.Retry.UseJitter = true;
        
        // Circuit Breaker
        options.CircuitBreaker.FailureRatio = 0.5;
        options.CircuitBreaker.SamplingDuration = TimeSpan.FromSeconds(10);
        options.CircuitBreaker.MinimumThroughput = 5;
        options.CircuitBreaker.BreakDuration = TimeSpan.FromSeconds(30);
        
        // Timeout
        options.AttemptTimeout.Timeout = TimeSpan.FromSeconds(10);
        options.TotalRequestTimeout.Timeout = TimeSpan.FromSeconds(30);
    });

// Registrar o wrapper service
builder.Services.AddScoped<IExternalAuthService, ExternalAuthService>();
```

### Configuração (appsettings.json)

```json
{
  "ExternalAuthApi": {
    "BaseUrl": "",
    "TimeoutSeconds": 30
  }
}
```

```csharp
public class ExternalAuthApiOptions
{
    public required string BaseUrl { get; init; }
    public int TimeoutSeconds { get; init; } = 30;
}
```

## 5. Resilience Customizado com Polly (.NET 7 e anterior)

```csharp
using Polly;
using Polly.Extensions.Http;

builder.Services.AddRefitClient<IExternalAuthApi>()
    .ConfigureHttpClient((sp, client) =>
    {
        var options = sp.GetRequiredService<IOptions<ExternalAuthApiOptions>>().Value;
        client.BaseAddress = new Uri(options.BaseUrl);
        client.Timeout = TimeSpan.FromSeconds(options.TimeoutSeconds);
    })
    .AddPolicyHandler(GetRetryPolicy())
    .AddPolicyHandler(GetCircuitBreakerPolicy());

static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
{
    return HttpPolicyExtensions
        .HandleTransientHttpError() // 5xx e 408
        .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
        .WaitAndRetryAsync(
            retryCount: 3,
            sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
            onRetry: (outcome, timespan, retryAttempt, context) =>
            {
                var logger = context.GetLogger();
                logger?.LogWarning("Retry {RetryAttempt} after {Delay}s due to {StatusCode}",
                    retryAttempt, timespan.TotalSeconds, outcome.Result?.StatusCode);
            });
}

static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
{
    return HttpPolicyExtensions
        .HandleTransientHttpError()
        .CircuitBreakerAsync(
            handledEventsAllowedBeforeBreaking: 5,
            durationOfBreak: TimeSpan.FromSeconds(30),
            onBreak: (outcome, duration) =>
            {
                // Log circuit breaker opened
            },
            onReset: () =>
            {
                // Log circuit breaker reset
            });
}
```

## 6. Tratamento de Erros

### ApiException

```csharp
try
{
    var response = await api.GetTokenAsync(apiKey, apiSecret, ct);
    return response.AccessToken;
}
catch (ApiException ex)
{
    // ex.StatusCode: HTTP status code
    // ex.Content: response body (string)
    // ex.HasContent: se há conteúdo na resposta
    
    if (ex.HasContent)
    {
        // Tentar deserializar erro da API
        var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(ex.Content);
        logger.LogError("API Error: {Error} - {Description}",
            errorResponse?.Error, errorResponse?.ErrorDescription);
    }
    
    throw ex.StatusCode switch
    {
        HttpStatusCode.BadRequest => new ArgumentException("Invalid request"),
        HttpStatusCode.Unauthorized => new UnauthorizedAccessException("Invalid credentials"),
        HttpStatusCode.NotFound => new KeyNotFoundException("Resource not found"),
        HttpStatusCode.TooManyRequests => new InvalidOperationException("Rate limit exceeded"),
        _ => new InvalidOperationException($"External API error: {ex.StatusCode}")
    };
}
catch (HttpRequestException ex)
{
    logger.LogError(ex, "Network error calling external API");
    throw new InvalidOperationException("Failed to communicate with external API", ex);
}
```

## 7. HTTP/3 (.NET 6+)

```csharp
builder.Services.AddRefitClient<IExternalAuthApi>()
    .ConfigureHttpClient(client => client.BaseAddress = new Uri("https://api.external.com"))
    .ConfigurePrimaryHttpMessageHandler(() => new SocketsHttpHandler
    {
        EnableMultipleHttp2Connections = true,
        PooledConnectionLifetime = TimeSpan.FromMinutes(2),
        PooledConnectionIdleTimeout = TimeSpan.FromMinutes(1)
    });

// HTTP/3 (quando suportado pelo servidor)
builder.Services.AddRefitClient<IExternalAuthApi>()
    .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
    {
        AutomaticDecompression = System.Net.DecompressionMethods.All,
        // HTTP/3 habilitado automaticamente quando servidor suporta
    });
```

## 8. Testes

### Mockar Interface Refit

```csharp
public class ExternalAuthServiceTests
{
    private readonly Mock<IExternalAuthApi> _apiMock;
    private readonly Mock<ILogger<ExternalAuthService>> _loggerMock;
    private readonly ExternalAuthService _sut;

    public ExternalAuthServiceTests()
    {
        _apiMock = new Mock<IExternalAuthApi>();
        _loggerMock = new Mock<ILogger<ExternalAuthService>>();
        _sut = new ExternalAuthService(_apiMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task GetTokenAsync_WhenSuccessful_ReturnsAccessToken()
    {
        // Arrange
        var expectedToken = "test-token-123";
        var response = new TokenResponse(expectedToken, 3600, "Bearer");
        _apiMock.Setup(x => x.GetTokenAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        // Act
        var result = await _sut.GetTokenAsync("key", "secret");

        // Assert
        result.Should().Be(expectedToken);
    }

    [Fact]
    public async Task GetTokenAsync_WhenUnauthorized_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        var apiException = await ApiException.Create(
            new HttpRequestMessage(),
            HttpMethod.Post,
            new HttpResponseMessage(HttpStatusCode.Unauthorized),
            new RefitSettings());
        
        _apiMock.Setup(x => x.GetTokenAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(apiException);

        // Act & Assert
        await _sut.Invoking(x => x.GetTokenAsync("key", "secret"))
            .Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("Invalid API credentials");
    }
}
```

### Testes de Integração (Opcional)

```csharp
[Collection("Integration")]
public class ExternalAuthApiIntegrationTests
{
    [Fact]
    public async Task GetTokenAsync_RealEndpoint_ReturnsToken()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddRefitClient<IExternalAuthApi>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri("https://api.external.com"));
        
        var provider = services.BuildServiceProvider();
        var api = provider.GetRequiredService<IExternalAuthApi>();

        // Act
        var response = await api.GetTokenAsync("test-key", "test-secret");

        // Assert
        response.AccessToken.Should().NotBeNullOrEmpty();
    }
}
```

## 9. Boas Práticas

- ✅ **Sempre** usar Refit para APIs externas (evitar HttpClient manual)
- ✅ **Sempre** adicionar resilience (retry, circuit breaker, timeout)
- ✅ **Sempre** passar `CancellationToken` para todas as chamadas
- ✅ **Sempre** logar chamadas externas (início, sucesso, erro)
- ✅ **Sempre** mapear `ApiException` para exceções do domínio
- ✅ **Sempre** usar records para request/response (imutáveis, concisos)
- ✅ **Sempre** configurar timeout apropriado
- ✅ Considerar cache para endpoints idempotentes (IMemoryCache, Redis)
- ✅ Considerar rate limiting no lado do cliente
- ❌ Evitar HttpClient manual (usar Refit)
- ❌ Evitar expor `ApiException` para camadas superiores
- ❌ Evitar hardcoded URLs (usar IOptions)

## 10. Autenticação

### Bearer Token

```csharp
public interface IProtectedApi
{
    [Get("/users/{id}")]
    [Headers("Authorization: Bearer")]
    Task<UserResponse> GetUserAsync(
        string id,
        [Authorize] string token,
        CancellationToken ct = default);
}
```

### API Key

```csharp
public interface IApiKeyApi
{
    [Get("/data")]
    Task<DataResponse> GetDataAsync(
        [Header("X-Api-Key")] string apiKey,
        CancellationToken ct = default);
}
```

### OAuth 2.0 com DelegatingHandler

```csharp
public class OAuth2Handler(ITokenService tokenService) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken ct)
    {
        var token = await tokenService.GetAccessTokenAsync(ct);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return await base.SendAsync(request, ct);
    }
}

// Registro
builder.Services.AddTransient<OAuth2Handler>();
builder.Services.AddRefitClient<IProtectedApi>()
    .ConfigureHttpClient(c => c.BaseAddress = new Uri("https://api.external.com"))
    .AddHttpMessageHandler<OAuth2Handler>()
    .AddStandardResilienceHandler();
```

---

## Resumo

Esta skill cobre:
- ✅ Refit para APIs externas (type-safe, menos boilerplate)
- ✅ Resilience patterns (retry, circuit breaker, timeout)
- ✅ Tratamento de erros e mapeamento de exceções
- ✅ HTTP/3 e performance
- ✅ Testes (mocks e integração)
- ✅ Autenticação (Bearer, API Key, OAuth 2.0)

Sempre que trabalhar com **APIs externas, Refit, HttpClient ou integração com serviços externos**, use esta skill como referência.
