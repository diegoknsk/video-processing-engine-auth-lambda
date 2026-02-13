---
name: security
description: Guia para segurança — autenticação, autorização, secrets management, rate limiting, CORS, security headers e proteção de APIs. Use quando a tarefa envolver segurança, autenticação, JWT, secrets, proteção ou vulnerabilidades.
---

# Security — Autenticação, Autorização e Proteção

## Quando Usar Esta Skill

Use quando a tarefa envolver:
- **Autenticação**, **autorização**, **JWT**
- **Secrets management**, **variáveis de ambiente**
- **Rate limiting**, **CORS**, **security headers**
- **Proteção de APIs**, vulnerabilidades, sanitização
- Palavras-chave: "segurança", "autenticação", "JWT", "secrets", "proteção", "vulnerabilidade"

## 1. Autenticação JWT

### Instalação

```bash
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
```

### Configuração

```csharp
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

// Program.cs
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var jwtSettings = builder.Configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey not configured");

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
            ClockSkew = TimeSpan.Zero // Remove delay padrão de 5 minutos
        };

        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                logger.LogWarning("Authentication failed: {Error}", context.Exception.Message);
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                var userId = context.Principal?.FindFirst("sub")?.Value;
                logger.LogInformation("Token validated for user {UserId}", userId);
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();
```

### appsettings.json (Placeholders)

```json
{
  "JwtSettings": {
    "SecretKey": "",
    "Issuer": "",
    "Audience": "",
    "ExpirationMinutes": 60
  }
}
```

### User Secrets (Dev)

```bash
dotnet user-secrets set "JwtSettings:SecretKey" "your-256-bit-secret-key-min-32-chars"
dotnet user-secrets set "JwtSettings:Issuer" "https://your-domain.com"
dotnet user-secrets set "JwtSettings:Audience" "https://your-api.com"
```

### Geração de Token

```csharp
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

public class TokenService(IOptions<JwtSettings> jwtSettings) : ITokenService
{
    private readonly JwtSettings _settings = jwtSettings.Value;

    public string GenerateToken(User user)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Name, user.Name),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Role, user.Role.ToString()),
            new Claim("tenant_id", user.TenantId.ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.SecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _settings.Issuer,
            audience: _settings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_settings.ExpirationMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public ClaimsPrincipal? ValidateToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_settings.SecretKey);

        try
        {
            var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _settings.Issuer,
                ValidateAudience = true,
                ValidAudience = _settings.Audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            }, out _);

            return principal;
        }
        catch
        {
            return null;
        }
    }
}

public class JwtSettings
{
    public required string SecretKey { get; init; }
    public required string Issuer { get; init; }
    public required string Audience { get; init; }
    public int ExpirationMinutes { get; init; } = 60;
}
```

### Uso nos Controllers

```csharp
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    [HttpGet("me")]
    [Authorize] // Requer autenticação
    public IActionResult GetCurrentUser()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var email = User.FindFirst(ClaimTypes.Email)?.Value;
        var roles = User.FindAll(ClaimTypes.Role).Select(c => c.Value);

        return Ok(new { UserId = userId, Email = email, Roles = roles });
    }

    [HttpGet("admin")]
    [Authorize(Roles = "Admin")] // Requer role Admin
    public IActionResult AdminOnly()
    {
        return Ok("Admin access granted");
    }
}
```

## 2. Autorização Baseada em Políticas

### Políticas Customizadas

```csharp
// Program.cs
builder.Services.AddAuthorization(options =>
{
    // Política baseada em role
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));

    // Política baseada em claim
    options.AddPolicy("PremiumUser", policy => policy.RequireClaim("subscription", "premium"));

    // Política customizada
    options.AddPolicy("MinimumAge", policy => policy.Requirements.Add(new MinimumAgeRequirement(18)));

    // Política combinada
    options.AddPolicy("AdminOrManager", policy =>
        policy.RequireAssertion(context =>
            context.User.IsInRole("Admin") || context.User.IsInRole("Manager")));
});

// Requirement
public class MinimumAgeRequirement(int minimumAge) : IAuthorizationRequirement
{
    public int MinimumAge { get; } = minimumAge;
}

// Handler
public class MinimumAgeHandler(ILogger<MinimumAgeHandler> logger) : AuthorizationHandler<MinimumAgeRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        MinimumAgeRequirement requirement)
    {
        var birthDateClaim = context.User.FindFirst("birthdate");
        if (birthDateClaim == null)
        {
            logger.LogWarning("Birth date claim not found");
            return Task.CompletedTask;
        }

        if (DateTime.TryParse(birthDateClaim.Value, out var birthDate))
        {
            var age = DateTime.Today.Year - birthDate.Year;
            if (age >= requirement.MinimumAge)
            {
                context.Succeed(requirement);
            }
        }

        return Task.CompletedTask;
    }
}

// Registro
builder.Services.AddSingleton<IAuthorizationHandler, MinimumAgeHandler>();
```

### Uso

```csharp
[HttpGet("adult-content")]
[Authorize(Policy = "MinimumAge")]
public IActionResult GetAdultContent()
{
    return Ok("Access granted");
}
```

## 3. Secrets Management

### User Secrets (Desenvolvimento)

```bash
# Inicializar
dotnet user-secrets init

# Adicionar secrets
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost;..."
dotnet user-secrets set "JwtSettings:SecretKey" "your-secret-key"
dotnet user-secrets set "ExternalApi:ApiKey" "your-api-key"

# Listar
dotnet user-secrets list

# Remover
dotnet user-secrets remove "JwtSettings:SecretKey"

# Limpar todos
dotnet user-secrets clear
```

### Variáveis de Ambiente (Produção)

```bash
# Linux/macOS
export ConnectionStrings__DefaultConnection="Server=prod;..."
export JwtSettings__SecretKey="prod-secret-key"

# Windows (PowerShell)
$env:ConnectionStrings__DefaultConnection="Server=prod;..."
$env:JwtSettings__SecretKey="prod-secret-key"

# Docker
docker run -e ConnectionStrings__DefaultConnection="..." -e JwtSettings__SecretKey="..." myapp
```

### Azure Key Vault

```bash
dotnet add package Azure.Extensions.AspNetCore.Configuration.Secrets
dotnet add package Azure.Identity
```

```csharp
using Azure.Identity;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsProduction())
{
    var keyVaultEndpoint = builder.Configuration["KeyVaultEndpoint"];
    if (!string.IsNullOrEmpty(keyVaultEndpoint))
    {
        builder.Configuration.AddAzureKeyVault(
            new Uri(keyVaultEndpoint),
            new DefaultAzureCredential());
    }
}
```

### AWS Secrets Manager

```bash
dotnet add package AWSSDK.SecretsManager
```

```csharp
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;

public class AwsSecretsService
{
    private readonly IAmazonSecretsManager _client;

    public AwsSecretsService()
    {
        _client = new AmazonSecretsManagerClient();
    }

    public async Task<string> GetSecretAsync(string secretName)
    {
        var request = new GetSecretValueRequest { SecretId = secretName };
        var response = await _client.GetSecretValueAsync(request);
        return response.SecretString;
    }
}
```

## 4. Rate Limiting (.NET 7+)

### Configuração

```bash
dotnet add package Microsoft.AspNetCore.RateLimiting
```

```csharp
using System.Threading.RateLimiting;

// Program.cs
builder.Services.AddRateLimiter(options =>
{
    // Fixed Window: 100 requests por minuto
    options.AddFixedWindowLimiter("fixed", limiterOptions =>
    {
        limiterOptions.PermitLimit = 100;
        limiterOptions.Window = TimeSpan.FromMinutes(1);
        limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        limiterOptions.QueueLimit = 10;
    });

    // Sliding Window: mais suave que Fixed
    options.AddSlidingWindowLimiter("sliding", limiterOptions =>
    {
        limiterOptions.PermitLimit = 100;
        limiterOptions.Window = TimeSpan.FromMinutes(1);
        limiterOptions.SegmentsPerWindow = 6; // 10 segundos por segmento
        limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        limiterOptions.QueueLimit = 10;
    });

    // Token Bucket: burst permitido
    options.AddTokenBucketLimiter("token", limiterOptions =>
    {
        limiterOptions.TokenLimit = 100;
        limiterOptions.ReplenishmentPeriod = TimeSpan.FromMinutes(1);
        limiterOptions.TokensPerPeriod = 100;
        limiterOptions.QueueLimit = 10;
    });

    // Concurrency: limitar requisições concorrentes
    options.AddConcurrencyLimiter("concurrency", limiterOptions =>
    {
        limiterOptions.PermitLimit = 10;
        limiterOptions.QueueLimit = 20;
    });

    // Rate limit por IP
    options.AddPolicy("perIp", context =>
    {
        var clientIp = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        
        return RateLimitPartition.GetFixedWindowLimiter(clientIp, _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = 50,
            Window = TimeSpan.FromMinutes(1)
        });
    });

    // Comportamento quando limite é atingido
    options.OnRejected = async (context, token) =>
    {
        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        
        if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
        {
            context.HttpContext.Response.Headers.RetryAfter = retryAfter.TotalSeconds.ToString();
        }

        await context.HttpContext.Response.WriteAsJsonAsync(new
        {
            Error = "Too many requests",
            Message = "Rate limit exceeded. Please try again later."
        }, cancellationToken: token);
    };
});

var app = builder.Build();

app.UseRateLimiter();
```

### Uso

```csharp
[HttpGet]
[EnableRateLimiting("fixed")]
public IActionResult Get()
{
    return Ok("Success");
}

[HttpPost]
[EnableRateLimiting("perIp")]
public IActionResult Create([FromBody] CreateInput input)
{
    return Ok("Created");
}

// Desabilitar rate limiting para endpoint específico
[HttpGet("health")]
[DisableRateLimiting]
public IActionResult Health()
{
    return Ok("Healthy");
}
```

## 5. CORS (Cross-Origin Resource Sharing)

```csharp
// Program.cs
builder.Services.AddCors(options =>
{
    // Desenvolvimento: permissivo
    if (builder.Environment.IsDevelopment())
    {
        options.AddPolicy("Development", builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
    }

    // Produção: restritivo
    options.AddPolicy("Production", builder =>
    {
        builder.WithOrigins(
                "https://myapp.com",
                "https://www.myapp.com")
               .WithMethods("GET", "POST", "PUT", "DELETE")
               .WithHeaders("Content-Type", "Authorization")
               .AllowCredentials()
               .SetPreflightMaxAge(TimeSpan.FromMinutes(10));
    });

    // Política específica para API pública
    options.AddPolicy("PublicApi", builder =>
    {
        builder.AllowAnyOrigin()
               .WithMethods("GET")
               .WithHeaders("Content-Type");
    });
});

var app = builder.Build();

// Aplicar política globalmente
app.UseCors(app.Environment.IsDevelopment() ? "Development" : "Production");

// Ou aplicar por endpoint
app.MapGet("/public", () => "Public data")
   .RequireCors("PublicApi");
```

## 6. Security Headers

```csharp
// Middleware customizado
public class SecurityHeadersMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        // HSTS (HTTP Strict Transport Security)
        context.Response.Headers.Append("Strict-Transport-Security", "max-age=31536000; includeSubDomains");

        // Prevent clickjacking
        context.Response.Headers.Append("X-Frame-Options", "DENY");

        // XSS Protection
        context.Response.Headers.Append("X-Content-Type-Options", "nosniff");

        // Referrer Policy
        context.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");

        // Content Security Policy
        context.Response.Headers.Append("Content-Security-Policy",
            "default-src 'self'; script-src 'self'; style-src 'self'; img-src 'self' data:; font-src 'self'; connect-src 'self'");

        // Permissions Policy (antiga Feature-Policy)
        context.Response.Headers.Append("Permissions-Policy",
            "geolocation=(), microphone=(), camera=()");

        // Remove server header
        context.Response.Headers.Remove("Server");
        context.Response.Headers.Remove("X-Powered-By");

        await next(context);
    }
}

// Registro
app.UseMiddleware<SecurityHeadersMiddleware>();

// Alternativa: usar pacote
// dotnet add package NetEscapades.AspNetCore.SecurityHeaders
app.UseSecurityHeaders(policies => policies.AddDefaultSecurityHeaders());
```

## 7. Validação e Sanitização de Input

### Evitar SQL Injection

```csharp
// ❌ SQL Injection vulnerável
public async Task<User?> GetUserByEmailUnsafe(string email)
{
    var sql = $"SELECT * FROM Users WHERE Email = '{email}'"; // VULNERÁVEL!
    return await _context.Users.FromSqlRaw(sql).FirstOrDefaultAsync();
}

// ✅ Parametrizado (seguro)
public async Task<User?> GetUserByEmail(string email)
{
    return await _context.Users
        .FromSqlInterpolated($"SELECT * FROM Users WHERE Email = {email}")
        .FirstOrDefaultAsync();
}

// ✅ Melhor: usar LINQ (seguro)
public async Task<User?> GetUserByEmail(string email)
{
    return await _context.Users
        .Where(u => u.Email == email)
        .FirstOrDefaultAsync();
}
```

### Evitar XSS (Cross-Site Scripting)

```csharp
// ASP.NET Core automaticamente escapa HTML em Razor views
// Mas para APIs JSON, sanitize input quando necessário

using System.Text.RegularExpressions;

public class SanitizationService
{
    public string SanitizeHtml(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        // Remover tags HTML
        var noHtml = Regex.Replace(input, "<.*?>", string.Empty);

        // Remover caracteres perigosos
        var sanitized = noHtml
            .Replace("<", "&lt;")
            .Replace(">", "&gt;")
            .Replace("\"", "&quot;")
            .Replace("'", "&#x27;")
            .Replace("/", "&#x2F;");

        return sanitized;
    }

    public string SanitizeFileName(string fileName)
    {
        // Remover caracteres inválidos de nome de arquivo
        var invalidChars = Path.GetInvalidFileNameChars();
        return string.Join("_", fileName.Split(invalidChars));
    }
}
```

### Evitar Path Traversal

```csharp
public class FileService
{
    private readonly string _basePath = "/app/uploads";

    public async Task<byte[]> GetFileAsync(string fileName)
    {
        // ❌ Vulnerável a path traversal
        // var path = Path.Combine(_basePath, fileName); // fileName pode ser "../../../etc/passwd"

        // ✅ Validar e sanitizar
        var sanitizedFileName = Path.GetFileName(fileName); // Remove path components
        var fullPath = Path.Combine(_basePath, sanitizedFileName);

        // Verificar que o path resultante está dentro do diretório permitido
        var fullPathResolved = Path.GetFullPath(fullPath);
        var basePathResolved = Path.GetFullPath(_basePath);

        if (!fullPathResolved.StartsWith(basePathResolved))
        {
            throw new UnauthorizedAccessException("Invalid file path");
        }

        return await File.ReadAllBytesAsync(fullPath);
    }
}
```

## 8. Password Hashing

```csharp
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

public class PasswordService
{
    public string HashPassword(string password)
    {
        // Gerar salt aleatório
        byte[] salt = RandomNumberGenerator.GetBytes(128 / 8);

        // Hash usando PBKDF2
        string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: password,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 100000,
            numBytesRequested: 256 / 8));

        // Retornar salt + hash
        return $"{Convert.ToBase64String(salt)}.{hashed}";
    }

    public bool VerifyPassword(string password, string hashedPassword)
    {
        var parts = hashedPassword.Split('.');
        if (parts.Length != 2)
            return false;

        var salt = Convert.FromBase64String(parts[0]);
        var hash = parts[1];

        var testHash = Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: password,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 100000,
            numBytesRequested: 256 / 8));

        return hash == testHash;
    }
}

// Alternativa: usar ASP.NET Core Identity PasswordHasher
// dotnet add package Microsoft.AspNetCore.Identity
using Microsoft.AspNetCore.Identity;

public class PasswordService2
{
    private readonly IPasswordHasher<User> _passwordHasher = new PasswordHasher<User>();

    public string HashPassword(User user, string password)
    {
        return _passwordHasher.HashPassword(user, password);
    }

    public bool VerifyPassword(User user, string hashedPassword, string providedPassword)
    {
        var result = _passwordHasher.VerifyHashedPassword(user, hashedPassword, providedPassword);
        return result == PasswordVerificationResult.Success;
    }
}
```

## 9. Checklist de Segurança

### Autenticação/Autorização
- [ ] JWT configurado com chave secreta forte (≥256 bits)
- [ ] Tokens com expiração apropriada
- [ ] ClockSkew configurado (TimeSpan.Zero ou pequeno)
- [ ] Claims apropriados no token (não incluir dados sensíveis)
- [ ] Políticas de autorização implementadas

### Secrets
- [ ] Nenhum secret versionado no código ou appsettings.json
- [ ] User Secrets configurado para dev
- [ ] Variáveis de ambiente ou Key Vault para prod
- [ ] Connection strings não contêm credenciais hardcoded

### Rate Limiting
- [ ] Rate limiting configurado para endpoints públicos
- [ ] Limites apropriados (não muito permissivos)
- [ ] Respostas 429 com Retry-After header

### CORS
- [ ] Política restritiva em produção (não AllowAnyOrigin)
- [ ] Origens específicas whitelistadas
- [ ] Métodos e headers limitados ao necessário

### Headers
- [ ] HSTS habilitado (Strict-Transport-Security)
- [ ] X-Frame-Options: DENY ou SAMEORIGIN
- [ ] X-Content-Type-Options: nosniff
- [ ] Content-Security-Policy configurado
- [ ] Server header removido

### Validação
- [ ] FluentValidation para todos os inputs
- [ ] Queries parametrizadas (nunca concatenar SQL)
- [ ] Paths validados contra path traversal
- [ ] HTML sanitizado quando necessário

### Passwords
- [ ] Passwords hasheados com algoritmo forte (PBKDF2, bcrypt, Argon2)
- [ ] Nunca armazenar passwords em plaintext
- [ ] Salt único por password

### HTTPS
- [ ] HTTPS enforced em produção
- [ ] Certificados válidos e renovados
- [ ] TLS 1.2+ apenas

---

## Resumo

Esta skill cobre:
- ✅ Autenticação JWT (geração, validação)
- ✅ Autorização baseada em políticas
- ✅ Secrets management (User Secrets, Key Vault, variáveis de ambiente)
- ✅ Rate limiting (.NET 7+)
- ✅ CORS (configuração restritiva)
- ✅ Security headers (HSTS, CSP, X-Frame-Options)
- ✅ Validação e sanitização (SQL injection, XSS, path traversal)
- ✅ Password hashing (PBKDF2, Identity PasswordHasher)
- ✅ Checklist de segurança

Sempre que trabalhar com **segurança, autenticação, secrets ou proteção de APIs**, use esta skill como referência.
