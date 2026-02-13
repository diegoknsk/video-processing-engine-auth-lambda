---
name: database-persistence
description: Guia para persistência com EF Core, repositórios, DbContext, migrations e queries eficientes. Use quando a tarefa envolver banco de dados, repositórios, EF Core, DbContext, migrations, queries ou persistência.
---

# Database Persistence — EF Core e Repositórios

## Quando Usar Esta Skill

Use quando a tarefa envolver:
- Criar ou modificar **repositórios**
- Trabalhar com **EF Core**, **DbContext**, **migrations**
- Queries e persistência de dados
- Palavras-chave: "banco", "database", "repositório", "EF Core", "migration", "DbContext", "query"

## 1. Estrutura de Persistência

- **Camada:** `<Projeto>.Infra.Persistence`
- **DbContext** restrito a esta camada; não vaza para Application ou API.
- **Entidades de persistência** separadas das entidades de domínio quando necessário.

```
<Projeto>.Infra.Persistence/
  Context/
    AppDbContext.cs
  Repositories/
    <Contexto>/
      UserRepository.cs
      OrderRepository.cs
  Configurations/           (EntityTypeConfiguration)
    UserConfiguration.cs
  Migrations/               (geradas automaticamente)
```

## 2. Repository Pattern

### Interface (Application/Ports)

```csharp
public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<User>> GetAllAsync(int page, int pageSize, CancellationToken ct = default);
    Task<User> CreateAsync(User user, CancellationToken ct = default);
    Task UpdateAsync(User user, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}
```

### Implementação (Infra.Persistence)

```csharp
public class UserRepository(AppDbContext context) : IUserRepository
{
    public async Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await context.Users
            .AsNoTracking() // Read-only
            .FirstOrDefaultAsync(u => u.Id == id, ct);
    }

    public async Task<IEnumerable<User>> GetAllAsync(int page, int pageSize, CancellationToken ct = default)
    {
        return await context.Users
            .AsNoTracking()
            .OrderBy(u => u.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);
    }

    public async Task<User> CreateAsync(User user, CancellationToken ct = default)
    {
        context.Users.Add(user);
        await context.SaveChangesAsync(ct);
        return user;
    }

    public async Task UpdateAsync(User user, CancellationToken ct = default)
    {
        context.Users.Update(user);
        await context.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var user = await context.Users.FindAsync([id], ct);
        if (user != null)
        {
            context.Users.Remove(user);
            await context.SaveChangesAsync(ct);
        }
    }
}
```

## 3. DbContext

```csharp
public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Order> Orders => Set<Order>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Aplicar todas as configurações do assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
```

## 4. Entity Configuration (Fluent API)

```csharp
public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        
        builder.HasKey(u => u.Id);
        
        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(200);
        
        builder.Property(u => u.Name)
            .IsRequired()
            .HasMaxLength(200);
        
        builder.HasIndex(u => u.Email).IsUnique();
        
        builder.Property(u => u.CreatedAt)
            .IsRequired();
    }
}
```

## 5. Migrations

### Criar Migration

```bash
dotnet ef migrations add InitialCreate --project <Projeto>.Infra.Persistence --startup-project <Projeto>.Api
```

### Aplicar Migration

```bash
# Dev
dotnet ef database update --project <Projeto>.Infra.Persistence --startup-project <Projeto>.Api

# Produção (via código)
public static async Task ApplyMigrationsAsync(this IServiceProvider services)
{
    using var scope = services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await context.Database.MigrateAsync();
}

// Em Program.cs
if (app.Environment.IsProduction())
{
    await app.Services.ApplyMigrationsAsync();
}
```

## 6. Queries Eficientes

### Evitar N+1

```csharp
// ❌ N+1 Problem
var orders = await context.Orders.ToListAsync();
foreach (var order in orders)
{
    var user = await context.Users.FindAsync(order.UserId); // N queries
}

// ✅ Eager Loading
var orders = await context.Orders
    .Include(o => o.User)
    .ToListAsync();
```

### Projeções

```csharp
// ✅ Projetar apenas o necessário
var users = await context.Users
    .AsNoTracking()
    .Select(u => new UserSummaryDto
    {
        Id = u.Id,
        Name = u.Name,
        Email = u.Email
    })
    .ToListAsync(ct);
```

### AsNoTracking para Read-Only

```csharp
// ✅ Read-only queries
var users = await context.Users
    .AsNoTracking()
    .Where(u => u.IsActive)
    .ToListAsync(ct);
```

### Paginação

```csharp
public async Task<PagedResult<User>> GetPagedAsync(int page, int pageSize, CancellationToken ct = default)
{
    var totalCount = await context.Users.CountAsync(ct);
    
    var items = await context.Users
        .AsNoTracking()
        .OrderBy(u => u.CreatedAt)
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync(ct);
    
    return new PagedResult<User>(items, totalCount, page, pageSize);
}
```

## 7. Registro no DI (Program.cs)

```csharp
// DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    options.UseSqlServer(connectionString); // ou UseNpgsql, UseSqlite, etc.
    
    if (builder.Environment.IsDevelopment())
    {
        options.EnableSensitiveDataLogging();
        options.EnableDetailedErrors();
    }
});

// Repositórios
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
```

## 8. Testes

### Testes Unitários (In-Memory)

```csharp
public class UserRepositoryTests
{
    private AppDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    [Fact]
    public async Task GetByIdAsync_WhenUserExists_ReturnsUser()
    {
        // Arrange
        await using var context = CreateInMemoryContext();
        var repository = new UserRepository(context);
        var user = new User { Id = Guid.NewGuid(), Email = "test@test.com", Name = "Test" };
        await repository.CreateAsync(user);

        // Act
        var result = await repository.GetByIdAsync(user.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Email.Should().Be("test@test.com");
    }
}
```

### Testes de Integração (Test Containers)

```csharp
public class DatabaseFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _container = new PostgreSqlBuilder()
        .WithDatabase("testdb")
        .Build();

    public string ConnectionString => _container.GetConnectionString();

    public async Task InitializeAsync() => await _container.StartAsync();
    public async Task DisposeAsync() => await _container.DisposeAsync();
}

public class UserRepositoryIntegrationTests(DatabaseFixture fixture) : IClassFixture<DatabaseFixture>
{
    [Fact]
    public async Task CreateAsync_ShouldPersistUser()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(fixture.ConnectionString)
            .Options;
        
        await using var context = new AppDbContext(options);
        await context.Database.MigrateAsync();
        
        var repository = new UserRepository(context);
        var user = new User { Id = Guid.NewGuid(), Email = "test@test.com", Name = "Test" };

        // Act
        await repository.CreateAsync(user);

        // Assert
        var result = await repository.GetByIdAsync(user.Id);
        result.Should().NotBeNull();
    }
}
```

## 9. Boas Práticas

- ✅ **AsNoTracking()** para queries read-only
- ✅ **Include()** para eager loading (evitar N+1)
- ✅ **Projeções** (Select) para buscar apenas dados necessários
- ✅ **Paginação** para listagens grandes
- ✅ **CancellationToken** em todas as operações assíncronas
- ✅ **Entity Configuration** (Fluent API) para configuração de entidades
- ❌ Evitar queries síncronas (ToList(), First(), etc.)
- ❌ Evitar expor DbContext para camadas superiores
- ❌ Evitar lógica de negócio nos repositórios (apenas persistência)

## 10. Performance

### Compiled Queries (.NET 7+)

```csharp
private static readonly Func<AppDbContext, Guid, Task<User?>> GetUserByIdQuery =
    EF.CompileAsyncQuery((AppDbContext context, Guid id) =>
        context.Users.AsNoTracking().FirstOrDefault(u => u.Id == id));

public async Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default)
{
    return await GetUserByIdQuery(context, id);
}
```

### Batch Operations

```csharp
// ✅ Batch insert
public async Task CreateManyAsync(IEnumerable<User> users, CancellationToken ct = default)
{
    context.Users.AddRange(users);
    await context.SaveChangesAsync(ct);
}

// ✅ Bulk delete (EF Core 7+)
public async Task DeleteInactiveUsersAsync(CancellationToken ct = default)
{
    await context.Users
        .Where(u => !u.IsActive)
        .ExecuteDeleteAsync(ct);
}
```

## 11. Connection Strings

### appsettings.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": ""
  }
}
```

### User Secrets (Dev)

```bash
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost;Database=mydb;User Id=user;Password=pass;"
```

### Variáveis de Ambiente (Prod)

```bash
export ConnectionStrings__DefaultConnection="Server=prod;Database=mydb;..."
```

---

## Resumo

Esta skill cobre:
- ✅ Repository pattern com EF Core
- ✅ DbContext e configuração
- ✅ Migrations
- ✅ Queries eficientes e otimizações
- ✅ Testes (unitários e integração)
- ✅ Boas práticas de performance

Sempre que trabalhar com **banco de dados, repositórios, EF Core ou migrations**, use esta skill como referência.
