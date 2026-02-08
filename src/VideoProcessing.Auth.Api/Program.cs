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
    
    // Configuração de base path para API Gateway (pode ser configurado via variável de ambiente)
    // Exemplo comentado para referência:
    // var baseUrl = builder.Configuration["API_BASE_URL"] ?? "http://localhost:5000";
    // options.AddServer(new OpenApiServer 
    // { 
    //     Url = baseUrl,
    //     Description = "API Gateway endpoint"
    // });
    // Para múltiplos stages:
    // options.AddServer(new OpenApiServer { Url = "https://api.example.com/prod", Description = "Production (API Gateway stage: prod)" });
    // options.AddServer(new OpenApiServer { Url = "https://api.example.com/dev", Description = "Development (API Gateway stage: dev)" });
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

// Middleware pipeline
app.UseMiddleware<VideoProcessing.Auth.Api.Middleware.GlobalExceptionMiddleware>();
app.UseHttpsRedirection();
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
