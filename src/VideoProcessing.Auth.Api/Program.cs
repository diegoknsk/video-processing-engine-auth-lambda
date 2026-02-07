using System.Text.Json;
using System.Text.Json.Serialization;
using Amazon.Lambda.AspNetCoreServer.Hosting;
using FluentValidation;
using FluentValidation.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Lambda Hosting
builder.Services.AddAWSLambdaHosting(LambdaEventSource.HttpApi);

// Controllers e JSON options
builder.Services.AddControllers(options =>
    {
        options.Filters.Add<VideoProcessing.Auth.Api.Filters.ValidationFilter>();
    })
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
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

var app = builder.Build();

// Middleware pipeline
app.UseHttpsRedirection();
app.UseCors();
app.UseAuthorization();
app.MapControllers();

app.Run();
