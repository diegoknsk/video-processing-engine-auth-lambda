---
name: performance-optimization
description: Guia para otimização de performance — Span<T>, Memory<T>, ArrayPool, ValueTask, compiled queries e técnicas avançadas. Use quando a tarefa envolver performance, otimização, alocações, memory pooling ou hot paths.
---

# Performance Optimization — .NET

## Quando Usar Esta Skill

Use quando a tarefa envolver:
- **Otimização de performance**, redução de alocações
- **Span<T>**, **Memory<T>**, **ArrayPool<T>**, **ValueTask<T>**
- **Hot paths**, **zero-allocation**, queries compiladas
- Palavras-chave: "performance", "otimizar", "alocações", "Span", "ArrayPool", "rápido", "hot path"

⚠️ **Importante:** Otimizações devem ser guiadas por **profiling** (medição real). Código legível tem prioridade; otimize apenas quando houver necessidade comprovada.

## 1. Span<T> e Memory<T>

### Span<T> — Manipulação de Arrays sem Alocações

**Quando usar:** parsing, manipulação de strings, processamento de arrays em hot paths.

```csharp
// ❌ Alocações desnecessárias
public static string ExtractDomain(string email)
{
    var atIndex = email.IndexOf('@');
    return email.Substring(atIndex + 1); // Aloca nova string
}

// ✅ Zero alocações com Span
public static ReadOnlySpan<char> ExtractDomain(ReadOnlySpan<char> email)
{
    var atIndex = email.IndexOf('@');
    return email.Slice(atIndex + 1); // Não aloca
}

// Uso
var email = "user@example.com".AsSpan();
var domain = ExtractDomain(email); // "example.com" sem alocação
```

### Parsing com Span

```csharp
// ❌ Alocações com Substring
public static int[] ParseNumbers(string input)
{
    var parts = input.Split(','); // Aloca array de strings
    var numbers = new int[parts.Length];
    for (int i = 0; i < parts.Length; i++)
    {
        numbers[i] = int.Parse(parts[i]); // Cada Parse pode alocar
    }
    return numbers;
}

// ✅ Zero alocações com Span
public static void ParseNumbers(ReadOnlySpan<char> input, Span<int> numbers)
{
    int index = 0;
    int start = 0;
    
    for (int i = 0; i <= input.Length; i++)
    {
        if (i == input.Length || input[i] == ',')
        {
            if (i > start)
            {
                numbers[index++] = int.Parse(input.Slice(start, i - start));
            }
            start = i + 1;
        }
    }
}

// Uso
var input = "10,20,30,40".AsSpan();
Span<int> numbers = stackalloc int[4];
ParseNumbers(input, numbers);
```

### Memory<T> — Span assíncrono-friendly

```csharp
// Span não pode ser usado em métodos async (vive no stack)
// Memory<T> é a alternativa para contextos assíncronos

public async Task<int> ProcessDataAsync(Memory<byte> buffer, CancellationToken ct)
{
    // Memory pode ser passado para métodos async
    await ReadDataAsync(buffer, ct);
    
    // Converter para Span quando necessário processamento síncrono
    Span<byte> span = buffer.Span;
    return ProcessBytes(span);
}

private int ProcessBytes(Span<byte> data)
{
    int sum = 0;
    foreach (var b in data)
        sum += b;
    return sum;
}
```

## 2. ArrayPool<T> — Reutilização de Arrays

**Quando usar:** necessidade temporária de arrays, especialmente em loops ou hot paths.

```csharp
using System.Buffers;

// ❌ Alocação a cada chamada
public byte[] ProcessData(int size)
{
    var buffer = new byte[size]; // Aloca
    // ... processa
    return buffer;
}

// ✅ Reutilização com ArrayPool
public byte[] ProcessDataPooled(int size)
{
    var buffer = ArrayPool<byte>.Shared.Rent(size); // Pode reutilizar array existente
    
    try
    {
        // ... processa
        
        // Copiar para array do tamanho exato (se necessário retornar)
        var result = new byte[size];
        Array.Copy(buffer, result, size);
        return result;
    }
    finally
    {
        ArrayPool<byte>.Shared.Return(buffer); // Devolver ao pool
    }
}

// ✅ Melhor: processar in-place sem retornar
public void ProcessDataInPlace(Span<byte> destination)
{
    var size = destination.Length;
    var buffer = ArrayPool<byte>.Shared.Rent(size);
    
    try
    {
        var span = buffer.AsSpan(0, size);
        
        // ... processa em span
        
        span.CopyTo(destination);
    }
    finally
    {
        ArrayPool<byte>.Shared.Return(buffer);
    }
}
```

### ArrayPool em Cenários Reais

```csharp
public class CsvParser
{
    public async Task<List<User>> ParseCsvAsync(Stream stream, CancellationToken ct)
    {
        var users = new List<User>();
        var buffer = ArrayPool<byte>.Shared.Rent(8192); // 8KB buffer

        try
        {
            int bytesRead;
            while ((bytesRead = await stream.ReadAsync(buffer, ct)) > 0)
            {
                var span = buffer.AsSpan(0, bytesRead);
                ParseLines(span, users);
            }

            return users;
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer);
        }
    }

    private void ParseLines(ReadOnlySpan<byte> data, List<User> users)
    {
        // Parse CSV lines from span
        // ...
    }
}
```

## 3. ValueTask<T> — Async sem Alocações

**Quando usar:** operações que frequentemente completam de forma síncrona (cache, fast path).

```csharp
// ❌ Task sempre aloca
public async Task<User?> GetUserAsync(string id)
{
    if (_cache.TryGetValue(id, out var user))
        return user; // Mesmo com cache hit, aloca Task

    return await LoadUserFromDatabaseAsync(id);
}

// ✅ ValueTask evita alocação no cache hit
public ValueTask<User?> GetUserAsync(string id)
{
    if (_cache.TryGetValue(id, out var user))
        return ValueTask.FromResult(user); // Sem alocação

    return new ValueTask<User?>(LoadUserFromDatabaseAsync(id));
}

// ✅ Padrão completo
public async ValueTask<User?> GetUserAsync(string id, CancellationToken ct)
{
    // Cache hit: sem alocação
    if (_cache.TryGetValue(id, out var user))
        return user;

    // Cache miss: aloca Task apenas quando necessário
    user = await LoadUserFromDatabaseAsync(id, ct);
    
    if (user != null)
        _cache.Set(id, user);
    
    return user;
}
```

### Quando NÃO Usar ValueTask

```csharp
// ❌ NÃO usar ValueTask quando:
// - Método sempre é assíncrono (sem fast path síncrono)
// - Resultado é await múltiplas vezes
// - Resultado é armazenado em campo/propriedade

public class Example
{
    // ❌ Nunca armazenar ValueTask
    private ValueTask<int> _task; // ERRADO!

    // ✅ Task é apropriado aqui
    public async Task<int> AlwaysAsyncMethod()
    {
        await Task.Delay(100);
        return 42;
    }
}
```

## 4. StringBuilder — Concatenação Eficiente

```csharp
// ❌ Concatenação em loop: O(n²)
public string BuildReport(List<string> lines)
{
    string result = "";
    foreach (var line in lines)
    {
        result += line + "\n"; // Aloca nova string a cada iteração
    }
    return result;
}

// ✅ StringBuilder: O(n)
public string BuildReport(List<string> lines)
{
    var sb = new StringBuilder(capacity: lines.Count * 50); // Pré-alocar capacidade estimada
    
    foreach (var line in lines)
    {
        sb.AppendLine(line);
    }
    
    return sb.ToString();
}

// ✅ Alternativa: String.Join para casos simples
public string BuildReport(List<string> lines)
{
    return string.Join("\n", lines);
}
```

## 5. EF Core — Queries Compiladas

**Quando usar:** queries executadas frequentemente com mesma estrutura.

```csharp
// ❌ Query normal: parsed a cada execução
public async Task<User?> GetUserByIdAsync(Guid id, CancellationToken ct)
{
    return await _context.Users
        .AsNoTracking()
        .FirstOrDefaultAsync(u => u.Id == id, ct);
}

// ✅ Compiled Query: parsed uma vez, reutilizado
private static readonly Func<AppDbContext, Guid, Task<User?>> GetUserByIdQuery =
    EF.CompileAsyncQuery((AppDbContext context, Guid id) =>
        context.Users.AsNoTracking().FirstOrDefault(u => u.Id == id));

public async Task<User?> GetUserByIdAsync(Guid id, CancellationToken ct)
{
    return await GetUserByIdQuery(_context, id);
}

// ✅ Compiled Query com parâmetros múltiplos
private static readonly Func<AppDbContext, string, UserRole, IAsyncEnumerable<User>> GetUsersByEmailAndRoleQuery =
    EF.CompileAsyncQuery((AppDbContext context, string emailPattern, UserRole role) =>
        context.Users
            .AsNoTracking()
            .Where(u => u.Email.Contains(emailPattern) && u.Role == role));

public IAsyncEnumerable<User> GetUsersByEmailAndRoleAsync(string emailPattern, UserRole role)
{
    return GetUsersByEmailAndRoleQuery(_context, emailPattern, role);
}
```

## 6. Caching

### IMemoryCache

```csharp
public class UserService(IMemoryCache cache, IUserRepository repository)
{
    public async Task<User?> GetUserAsync(Guid id, CancellationToken ct)
    {
        var cacheKey = $"user:{id}";

        // Tentar obter do cache
        if (cache.TryGetValue(cacheKey, out User? user))
            return user;

        // Cache miss: buscar do banco
        user = await repository.GetByIdAsync(id, ct);

        if (user != null)
        {
            // Armazenar no cache com expiração
            var cacheOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(10))
                .SetSlidingExpiration(TimeSpan.FromMinutes(2));

            cache.Set(cacheKey, user, cacheOptions);
        }

        return user;
    }
}
```

### Output Caching (.NET 7+)

```csharp
// Program.cs
builder.Services.AddOutputCache(options =>
{
    options.AddBasePolicy(builder => builder
        .Expire(TimeSpan.FromMinutes(5))
        .Tag("api"));

    options.AddPolicy("users", builder => builder
        .Expire(TimeSpan.FromMinutes(10))
        .Tag("users"));
});

app.UseOutputCache();

// Controller
[HttpGet("{id}")]
[OutputCache(PolicyName = "users")]
public async Task<IActionResult> GetUserAsync(Guid id)
{
    var user = await _useCase.ExecuteAsync(id);
    return Ok(user);
}

// Invalidar cache por tag
[HttpPost]
public async Task<IActionResult> CreateUserAsync([FromBody] CreateUserInput input)
{
    var result = await _useCase.ExecuteAsync(input);
    
    // Invalidar cache de users
    await _cache.EvictByTagAsync("users", HttpContext.RequestAborted);
    
    return Ok(result);
}
```

## 7. Async Streams — IAsyncEnumerable<T>

**Quando usar:** streaming de grandes volumes de dados.

```csharp
// ❌ Carregar tudo na memória
public async Task<List<Order>> GetOrdersAsync(CancellationToken ct)
{
    return await _context.Orders.ToListAsync(ct); // Carrega todos os pedidos
}

// ✅ Streaming com IAsyncEnumerable
public async IAsyncEnumerable<Order> GetOrdersStreamAsync(
    [EnumeratorCancellation] CancellationToken ct = default)
{
    await foreach (var order in _context.Orders.AsAsyncEnumerable().WithCancellation(ct))
    {
        // Processar um por vez, sem carregar tudo na memória
        yield return order;
    }
}

// Uso
await foreach (var order in orderService.GetOrdersStreamAsync(ct))
{
    await ProcessOrderAsync(order, ct);
}
```

## 8. Parallel Processing

### Parallel.ForEachAsync (.NET 6+)

```csharp
// ❌ Sequencial: lento
public async Task ProcessOrdersAsync(List<Order> orders, CancellationToken ct)
{
    foreach (var order in orders)
    {
        await ProcessOrderAsync(order, ct);
    }
}

// ✅ Paralelo: rápido (controlar grau de paralelismo)
public async Task ProcessOrdersAsync(List<Order> orders, CancellationToken ct)
{
    var options = new ParallelOptions
    {
        MaxDegreeOfParallelism = 10, // Limitar paralelismo
        CancellationToken = ct
    };

    await Parallel.ForEachAsync(orders, options, async (order, ct) =>
    {
        await ProcessOrderAsync(order, ct);
    });
}
```

### Task.WhenAll para Batch Processing

```csharp
public async Task<List<User>> GetUsersAsync(List<Guid> ids, CancellationToken ct)
{
    var tasks = ids.Select(id => GetUserAsync(id, ct));
    var users = await Task.WhenAll(tasks);
    return users.Where(u => u != null).ToList()!;
}
```

## 9. Lazy Initialization

```csharp
// ❌ Inicialização pesada no construtor
public class ExpensiveService
{
    private readonly HeavyResource _resource;

    public ExpensiveService()
    {
        _resource = new HeavyResource(); // Bloqueia construtor
    }
}

// ✅ Lazy initialization
public class ExpensiveService
{
    private readonly Lazy<HeavyResource> _resource = new(() => new HeavyResource());

    public void DoWork()
    {
        // Inicializa apenas quando acessado pela primeira vez
        _resource.Value.Process();
    }
}

// ✅ Lazy thread-safe com async
public class ExpensiveService
{
    private readonly SemaphoreSlim _lock = new(1, 1);
    private HeavyResource? _resource;

    public async Task<HeavyResource> GetResourceAsync()
    {
        if (_resource != null)
            return _resource;

        await _lock.WaitAsync();
        try
        {
            if (_resource == null)
                _resource = await HeavyResource.CreateAsync();
            
            return _resource;
        }
        finally
        {
            _lock.Release();
        }
    }
}
```

## 10. Profiling e Medição

### BenchmarkDotNet

```bash
dotnet add package BenchmarkDotNet
```

```csharp
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

[MemoryDiagnoser]
public class ParsingBenchmarks
{
    private const string Input = "10,20,30,40,50";

    [Benchmark(Baseline = true)]
    public int[] ParseWithSplit()
    {
        var parts = Input.Split(',');
        var numbers = new int[parts.Length];
        for (int i = 0; i < parts.Length; i++)
            numbers[i] = int.Parse(parts[i]);
        return numbers;
    }

    [Benchmark]
    public void ParseWithSpan()
    {
        Span<int> numbers = stackalloc int[5];
        ParseNumbers(Input.AsSpan(), numbers);
    }

    private void ParseNumbers(ReadOnlySpan<char> input, Span<int> numbers)
    {
        // Implementação com Span (exemplo anterior)
    }
}

// Program.cs
public class Program
{
    public static void Main(string[] args)
    {
        BenchmarkRunner.Run<ParsingBenchmarks>();
    }
}
```

### Stopwatch para Medições Simples

```csharp
var sw = Stopwatch.StartNew();

// Operação a medir
await ProcessDataAsync();

sw.Stop();
logger.LogInformation("Operation took {ElapsedMs}ms", sw.Elapsed.TotalMilliseconds);
```

## 11. Boas Práticas

### Quando Otimizar

- ✅ **Após** profiling (não otimize prematuramente)
- ✅ Hot paths identificados (loops internos, operações frequentes)
- ✅ Alocações excessivas detectadas (memory profiler)
- ✅ Latências mensuradas acima do aceitável

### Prioridades

1. **Correto** > Rápido
2. **Legível** > Performático (a menos que seja hot path crítico)
3. **Medido** > Assumido (sempre profile antes e depois)

### Trade-offs

- **Span<T>:** performance vs. complexidade (stack-only, não async)
- **ValueTask:** performance vs. complexidade (não reutilizável)
- **ArrayPool:** performance vs. gerenciamento manual (lembrar de Return)
- **Caching:** performance vs. consistência (cache invalidation é difícil)

### Checklist de Otimização

- [ ] Profiling executado e hot paths identificados?
- [ ] Benchmark criado para medir impacto da otimização?
- [ ] Otimização resulta em melhoria mensurável (>10%)?
- [ ] Código otimizado ainda é legível e testável?
- [ ] Testes cobrem o código otimizado?
- [ ] Documentação explica por que a otimização é necessária?

---

## Resumo

Esta skill cobre:
- ✅ Span<T> e Memory<T> para zero-allocation
- ✅ ArrayPool<T> para reutilização de buffers
- ✅ ValueTask<T> para async eficiente
- ✅ StringBuilder para concatenação
- ✅ Compiled queries no EF Core
- ✅ Caching (IMemoryCache, Output Cache)
- ✅ IAsyncEnumerable<T> para streaming
- ✅ Parallel processing
- ✅ Profiling e benchmarking
- ✅ Boas práticas e trade-offs

**Lembre-se:** Otimize apenas quando necessário, sempre guiado por medição real (profiling). Código legível e correto tem prioridade.

Sempre que trabalhar com **performance, otimização, alocações ou hot paths**, use esta skill como referência.
