using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Amazon.Lambda.AspNetCoreServer.Hosting;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Lambda Hosting
builder.Services.AddAWSLambdaHosting(LambdaEventSource.HttpApi);

// Controllers e JSON options
builder.Services.AddControllers(options =>
    {
        options.Filters.Add<VideoProcessing.Auth.Api.Filters.ValidationFilter>();
        options.Filters.Add<VideoProcessing.Auth.Api.Filters.ApiResponseFilter>();
    })
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

// OpenAPI / Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Video Processing Auth API",
        Version = "v1",
        Description = "API de autenticação para Video Processing Engine usando Amazon Cognito",
        Contact = new OpenApiContact
        {
            Name = "Equipe Video Processing",
            Email = "team@videoprocessing.example.com"
        }
    });
    
    // Incluir XML comments
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
    
    // Server do OpenAPI: preenchido pelo filter a partir do request (Scheme + Host + PathBase) quando
    // o doc é gerado no pipeline. Assim atrás do gateway não é necessário API_PUBLIC_BASE_URL.
    // Fallback: se API_PUBLIC_BASE_URL estiver definida, usa como server (útil se o doc for gerado sem request).
    var apiPublicBaseUrl = builder.Configuration["API_PUBLIC_BASE_URL"];
    if (!string.IsNullOrWhiteSpace(apiPublicBaseUrl))
    {
        options.AddServer(new OpenApiServer
        {
            Url = apiPublicBaseUrl.Trim().TrimEnd('/'),
            Description = "API Gateway (stage + path prefix)"
        });
    }
    options.DocumentFilter<VideoProcessing.Auth.Api.OpenApi.OpenApiServerFromRequestFilter>();
});

// FluentValidation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();
builder.Services.AddValidatorsFromAssemblyContaining(typeof(VideoProcessing.Auth.Application.AssemblyReference));

// CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Options Pattern - Cognito
builder.Services.Configure<VideoProcessing.Auth.Infra.Models.CognitoOptions>(
    builder.Configuration.GetSection("Cognito"));

// AWS Services
builder.Services.AddSingleton<Amazon.CognitoIdentityProvider.IAmazonCognitoIdentityProvider>(sp =>
{
    var options = sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<VideoProcessing.Auth.Infra.Models.CognitoOptions>>();
    return new Amazon.CognitoIdentityProvider.AmazonCognitoIdentityProviderClient(
        Amazon.RegionEndpoint.GetBySystemName(options.Value.Region));
});

// Services
builder.Services.AddScoped<VideoProcessing.Auth.Application.Ports.ICognitoAuthService, VideoProcessing.Auth.Infra.Services.CognitoAuthService>();
builder.Services.AddScoped<VideoProcessing.Auth.Application.UseCases.Auth.LoginUseCase>();
builder.Services.AddScoped<VideoProcessing.Auth.Application.UseCases.Auth.CreateUserUseCase>();

var app = builder.Build();

// Middleware pipeline — PathBase do gateway primeiro (quando GATEWAY_PATH_PREFIX definida).
// UseRouting() deve vir logo após alterar o path para que o endpoint seja selecionado com o path já reescrito (ver aspnetcore#49454).
app.UseMiddleware<VideoProcessing.Auth.Api.Middleware.GatewayPathBaseMiddleware>();
app.UseRouting();
app.UseMiddleware<VideoProcessing.Auth.Api.Middleware.GlobalExceptionMiddleware>();

// HTTPS redirection apenas local: no Lambda o API Gateway já faz HTTPS
if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("AWS_LAMBDA_FUNCTION_NAME")))
{
    app.UseHttpsRedirection();
}

app.UseCors();
app.UseAuthorization();

// Swagger / OpenAPI
app.UseSwagger();

// Scalar UI - Documentação interativa (aponta para o JSON gerado pelo Swashbuckle)
app.MapScalarApiReference(options =>
{
    options.Title = "Video Processing Auth API";
    options.Theme = ScalarTheme.Default;
    options.WithOpenApiRoutePattern("/swagger/v1/swagger.json");
});

app.MapControllers();

app.Run();
