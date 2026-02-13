---
name: observability
description: Guia para observabilidade — logging estruturado, métricas, tracing distribuído, health checks e instrumentação. Use quando a tarefa envolver logging, métricas, OpenTelemetry, tracing, health checks ou monitoramento.
---

# Observability — Logging, Métricas e Tracing

## Quando Usar Esta Skill

Use quando a tarefa envolver:
- **Logging estruturado**, **métricas**, **tracing distribuído**
- **OpenTelemetry**, **Serilog**, **health checks**
- **Instrumentação**, monitoramento, diagnóstico
- Palavras-chave: "logging", "métricas", "tracing", "observabilidade", "OpenTelemetry", "health check", "monitorar"

## 1. Os Três Pilares da Observabilidade

### 1. Logs
Eventos discretos com contexto (timestamp, nível, mensagem, dados estruturados)

### 2. Métricas
Agregações numéricas ao longo do tempo (counters, gauges, histograms)

### 3. Traces
Rastreamento de requisições através de sistemas distribuídos

## 2. Logging Estruturado

### ILogger (Built-in .NET)

```csharp
public class CreateUserUseCase(
    IUserRepository repository,
    ILogger<CreateUserUseCase> logger) : ICreateUserUseCase
{
    public async Task<CreateUserResponseModel> ExecuteAsync(CreateUserInput input, CancellationToken ct = default)
    {
        logger.LogInformation("Creating user with email {Email}", input.Email);

        try
        {
            if (await repository.ExistsAsync(input.Email, ct))
            {
                logger.LogWarning("Attempt to create user with existing email {Email}", input.Email);
                throw new InvalidOperationException("Email já está em uso.");
            }

            var user = await repository.CreateAsync(input, ct);
            
            logger.LogInformation("User created successfully with ID {UserId}", user.Id);
            
            return CreateUserPresenter.Present(user);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to create user with email {Email}", input.Email);
            throw;
        }
    }
}
```

### LoggerMessage Source Generators (.NET 6+)

**Alta performance** — evita boxing e alocações:

```csharp
public static partial class Log
{
    [LoggerMessage(
        EventId = 1001,
        Level = LogLevel.Information,
        Message = "Creating user with email {Email}")]
    public static partial void CreatingUser(ILogger logger, string email);

    [LoggerMessage(
        EventId = 1002,
        Level = LogLevel.Information,
        Message = "User created successfully with ID {UserId}")]
    public static partial void UserCreated(ILogger logger, Guid userId);

    [LoggerMessage(
        EventId = 1003,
        Level = LogLevel.Warning,
        Message = "Attempt to create user with existing email {Email}")]
    public static partial void EmailAlreadyExists(ILogger logger, string email);

    [LoggerMessage(
        EventId = 1004,
        Level = LogLevel.Error,
        Message = "Failed to create user with email {Email}")]
    public static partial void UserCreationFailed(ILogger logger, string email, Exception exception);
}

// Uso
public class CreateUserUseCase(IUserRepository repository, ILogger<CreateUserUseCase> logger)
{
    public async Task<CreateUserResponseModel> ExecuteAsync(CreateUserInput input, CancellationToken ct = default)
    {
        Log.CreatingUser(logger, input.Email);

        try
        {
            if (await repository.ExistsAsync(input.Email, ct))
            {
                Log.EmailAlreadyExists(logger, input.Email);
                throw new InvalidOperationException("Email já está em uso.");
            }

            var user = await repository.CreateAsync(input, ct);
            Log.UserCreated(logger, user.Id);
            
            return CreateUserPresenter.Present(user);
        }
        catch (Exception ex)
        {
            Log.UserCreationFailed(logger, input.Email, ex);
            throw;
        }
    }
}
```

### Serilog (Opcional — Logging Avançado)

```bash
dotnet add package Serilog.AspNetCore
dotnet add package Serilog.Sinks.Console
dotnet add package Serilog.Sinks.File
dotnet add package Serilog.Enrichers.Environment
dotnet add package Serilog.Enrichers.Process
dotnet add package Serilog.Enrichers.Thread
```

```csharp
using Serilog;

// Program.cs
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .Enrich.WithMachineName()
    .Enrich.WithEnvironmentName()
    .Enrich.WithThreadId()
    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
    .WriteTo.File(
        path: "logs/log-.txt",
        rollingInterval: RollingInterval.Day,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
    .CreateLogger();

try
{
    Log.Information("Starting application");

    var builder = WebApplication.CreateBuilder(args);
    
    // Usar Serilog
    builder.Host.UseSerilog();

    // ... resto da configuração
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
```

### Logging com Contexto

```csharp
using Microsoft.Extensions.Logging;

// Adicionar contexto ao escopo
using (logger.BeginScope(new Dictionary<string, object>
{
    ["UserId"] = userId,
    ["TenantId"] = tenantId,
    ["RequestId"] = requestId
}))
{
    logger.LogInformation("Processing order");
    // Todos os logs neste escopo incluirão UserId, TenantId, RequestId
}

// Serilog
using (LogContext.PushProperty("UserId", userId))
using (LogContext.PushProperty("TenantId", tenantId))
{
    logger.LogInformation("Processing order");
}
```

## 3. Métricas — System.Diagnostics.Metrics (.NET 8+)

### Definir Métricas

```csharp
using System.Diagnostics.Metrics;

public class OrderMetrics
{
    private readonly Counter<int> _ordersProcessed;
    private readonly Histogram<double> _orderProcessingTime;
    private readonly ObservableGauge<int> _activeOrders;
    private int _activeOrderCount;

    public OrderMetrics(IMeterFactory meterFactory)
    {
        var meter = meterFactory.Create("Orders");

        // Counter: valor sempre crescente
        _ordersProcessed = meter.CreateCounter<int>(
            name: "orders.processed",
            unit: "orders",
            description: "Total number of orders processed");

        // Histogram: distribuição de valores (latências, tamanhos)
        _orderProcessingTime = meter.CreateHistogram<double>(
            name: "orders.processing_time",
            unit: "ms",
            description: "Order processing time in milliseconds");

        // ObservableGauge: valor que muda ao longo do tempo (observado callback)
        _activeOrders = meter.CreateObservableGauge<int>(
            name: "orders.active",
            observeValue: () => _activeOrderCount,
            unit: "orders",
            description: "Current number of active orders");
    }

    public void RecordOrderProcessed() => _ordersProcessed.Add(1);

    public void RecordProcessingTime(double milliseconds) => _orderProcessingTime.Record(milliseconds);

    public void IncrementActiveOrders() => Interlocked.Increment(ref _activeOrderCount);

    public void DecrementActiveOrders() => Interlocked.Decrement(ref _activeOrderCount);
}
```

### Uso em UseCases

```csharp
public class CreateOrderUseCase(
    IOrderRepository repository,
    OrderMetrics metrics,
    ILogger<CreateOrderUseCase> logger)
{
    public async Task<CreateOrderResponseModel> ExecuteAsync(CreateOrderInput input, CancellationToken ct = default)
    {
        var sw = Stopwatch.StartNew();
        metrics.IncrementActiveOrders();

        try
        {
            logger.LogInformation("Creating order for user {UserId}", input.UserId);

            var order = await repository.CreateAsync(input, ct);

            metrics.RecordOrderProcessed();
            logger.LogInformation("Order created successfully with ID {OrderId}", order.Id);

            return CreateOrderPresenter.Present(order);
        }
        finally
        {
            sw.Stop();
            metrics.RecordProcessingTime(sw.Elapsed.TotalMilliseconds);
            metrics.DecrementActiveOrders();
        }
    }
}
```

### Registro no DI

```csharp
// Program.cs
builder.Services.AddSingleton<OrderMetrics>();
```

### Exportar Métricas (Prometheus)

```bash
dotnet add package OpenTelemetry.Exporter.Prometheus.AspNetCore
```

```csharp
using OpenTelemetry.Metrics;

builder.Services.AddOpenTelemetry()
    .WithMetrics(metrics => metrics
        .AddMeter("Orders")
        .AddMeter("Users")
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddPrometheusExporter());

var app = builder.Build();

// Endpoint para Prometheus scraping
app.MapPrometheusScrapingEndpoint(); // /metrics

app.Run();
```

## 4. Tracing Distribuído — OpenTelemetry (.NET 7+)

### Instalação

```bash
dotnet add package OpenTelemetry.Exporter.Console
dotnet add package OpenTelemetry.Exporter.OpenTelemetryProtocol
dotnet add package OpenTelemetry.Extensions.Hosting
dotnet add package OpenTelemetry.Instrumentation.AspNetCore
dotnet add package OpenTelemetry.Instrumentation.Http
dotnet add package OpenTelemetry.Instrumentation.SqlClient
```

### Configuração

```csharp
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource
        .AddService(
            serviceName: "MyApp",
            serviceVersion: "1.0.0"))
    .WithTracing(tracing => tracing
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddSqlClientInstrumentation(options =>
        {
            options.SetDbStatementForText = true;
            options.RecordException = true;
        })
        .AddSource("MyApp.*") // Custom sources
        .AddConsoleExporter() // Dev
        .AddOtlpExporter(options => // Produção (Jaeger, Tempo, etc.)
        {
            options.Endpoint = new Uri("http://localhost:4317");
        }));
```

### Instrumentação Customizada

```csharp
using System.Diagnostics;

public class OrderService
{
    private static readonly ActivitySource ActivitySource = new("MyApp.Orders");

    public async Task<Order> ProcessOrderAsync(Guid orderId)
    {
        using var activity = ActivitySource.StartActivity("ProcessOrder");
        activity?.SetTag("order.id", orderId);

        try
        {
            // Simular etapas
            await ValidateOrderAsync(orderId);
            await CalculateTotalAsync(orderId);
            await SaveOrderAsync(orderId);

            activity?.SetTag("order.status", "completed");
            return new Order { Id = orderId };
        }
        catch (Exception ex)
        {
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            activity?.RecordException(ex);
            throw;
        }
    }

    private async Task ValidateOrderAsync(Guid orderId)
    {
        using var activity = ActivitySource.StartActivity("ValidateOrder");
        activity?.SetTag("order.id", orderId);
        await Task.Delay(10); // Simular validação
    }

    private async Task CalculateTotalAsync(Guid orderId)
    {
        using var activity = ActivitySource.StartActivity("CalculateTotal");
        activity?.SetTag("order.id", orderId);
        await Task.Delay(20);
    }

    private async Task SaveOrderAsync(Guid orderId)
    {
        using var activity = ActivitySource.StartActivity("SaveOrder");
        activity?.SetTag("order.id", orderId);
        await Task.Delay(30);
    }
}
```

### Propagação de Contexto

```csharp
// Automática para:
// - ASP.NET Core (via middleware)
// - HttpClient (via instrumentation)
// - EF Core / SqlClient (via instrumentation)

// Manual (se necessário):
var activity = Activity.Current;
var traceId = activity?.TraceId.ToString();
var spanId = activity?.SpanId.ToString();

// Incluir em logs
logger.LogInformation("Processing order {OrderId} [TraceId: {TraceId}, SpanId: {SpanId}]",
    orderId, traceId, spanId);
```

## 5. Health Checks

### Configuração Básica

```csharp
// Program.cs
builder.Services.AddHealthChecks()
    .AddCheck("self", () => HealthCheckResult.Healthy())
    .AddDbContextCheck<AppDbContext>()
    .AddUrlGroup(new Uri("https://api.external.com/health"), name: "external-api")
    .AddCheck<RedisHealthCheck>("redis")
    .AddCheck<QueueHealthCheck>("message-queue");

var app = builder.Build();

// Endpoints
app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";
        var result = JsonSerializer.Serialize(new
        {
            status = report.Status.ToString(),
            checks = report.Entries.Select(e => new
            {
                name = e.Key,
                status = e.Value.Status.ToString(),
                description = e.Value.Description,
                duration = e.Value.Duration.TotalMilliseconds
            }),
            totalDuration = report.TotalDuration.TotalMilliseconds
        });
        await context.Response.WriteAsync(result);
    }
});

// Liveness (apenas verifica se o app está rodando)
app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = _ => false // Não executa checks, apenas retorna 200
});

// Readiness (verifica dependências)
app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready")
});
```

### Health Check Customizado

```csharp
using Microsoft.Extensions.Diagnostics.HealthChecks;

public class DatabaseHealthCheck(AppDbContext context) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Executar query simples
            await context.Database.CanConnectAsync(cancellationToken);
            
            return HealthCheckResult.Healthy("Database is reachable");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Database is unreachable", ex);
        }
    }
}

public class RedisHealthCheck(IConnectionMultiplexer redis) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var db = redis.GetDatabase();
            await db.PingAsync();
            
            return HealthCheckResult.Healthy("Redis is reachable");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Redis is unreachable", ex);
        }
    }
}

// Registro
builder.Services.AddHealthChecks()
    .AddCheck<DatabaseHealthCheck>("database", tags: ["ready"])
    .AddCheck<RedisHealthCheck>("redis", tags: ["ready"]);
```

## 6. Correlação de Logs, Métricas e Traces

### Middleware de Correlação

```csharp
public class CorrelationMiddleware(RequestDelegate next, ILogger<CorrelationMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = context.Request.Headers["X-Correlation-ID"].FirstOrDefault()
            ?? Guid.NewGuid().ToString();

        context.Items["CorrelationId"] = correlationId;
        context.Response.Headers.Append("X-Correlation-ID", correlationId);

        // Adicionar ao Activity (tracing)
        Activity.Current?.SetTag("correlation.id", correlationId);

        // Adicionar ao log context
        using (logger.BeginScope(new Dictionary<string, object> { ["CorrelationId"] = correlationId }))
        {
            await next(context);
        }
    }
}

// Registro
app.UseMiddleware<CorrelationMiddleware>();
```

## 7. Configuração para Diferentes Ambientes

### appsettings.Development.json

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft": "Information",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "OpenTelemetry": {
    "Tracing": {
      "Exporter": "Console"
    },
    "Metrics": {
      "Exporter": "Console"
    }
  }
}
```

### appsettings.Production.json

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "OpenTelemetry": {
    "Tracing": {
      "Exporter": "OTLP",
      "Endpoint": "http://collector:4317"
    },
    "Metrics": {
      "Exporter": "Prometheus"
    }
  }
}
```

## 8. Boas Práticas

### Logging

- ✅ **Sempre** usar logging estruturado (placeholders `{Property}`, não string interpolation)
- ✅ **Sempre** incluir contexto relevante (IDs, tenant, user)
- ✅ **Sempre** usar níveis apropriados (Trace, Debug, Information, Warning, Error, Critical)
- ✅ **Sempre** logar exceções com `logger.LogError(ex, "message")`
- ✅ Usar LoggerMessage Source Generators para performance crítica
- ❌ Evitar logar dados sensíveis (senhas, tokens, PII)
- ❌ Evitar logging excessivo (impacta performance e custo)

### Métricas

- ✅ **Sempre** usar unidades consistentes (ms, bytes, requests)
- ✅ **Sempre** incluir tags relevantes (endpoint, status, tenant)
- ✅ Usar Counter para contadores crescentes (total de requests)
- ✅ Usar Histogram para distribuições (latências, tamanhos)
- ✅ Usar Gauge para valores instantâneos (conexões ativas, memória)
- ❌ Evitar cardinalidade alta em tags (não usar IDs únicos)

### Tracing

- ✅ **Sempre** propagar contexto de trace entre serviços
- ✅ **Sempre** adicionar tags relevantes aos spans
- ✅ **Sempre** gravar exceções em spans (`activity.RecordException(ex)`)
- ✅ Criar spans customizados para operações importantes
- ❌ Evitar spans excessivamente granulares (impacta performance)

### Health Checks

- ✅ **Sempre** implementar `/health/live` (liveness) e `/health/ready` (readiness)
- ✅ **Sempre** verificar dependências críticas (DB, cache, APIs externas)
- ✅ Usar timeouts curtos para health checks (1-2 segundos)
- ❌ Evitar lógica complexa em health checks

---

## Resumo

Esta skill cobre:
- ✅ Logging estruturado (ILogger, LoggerMessage Source Generators, Serilog)
- ✅ Métricas (System.Diagnostics.Metrics, Prometheus)
- ✅ Tracing distribuído (OpenTelemetry, Activity, custom spans)
- ✅ Health checks (liveness, readiness, custom checks)
- ✅ Correlação de logs, métricas e traces
- ✅ Boas práticas de observabilidade

Sempre que trabalhar com **logging, métricas, tracing, OpenTelemetry ou health checks**, use esta skill como referência.
