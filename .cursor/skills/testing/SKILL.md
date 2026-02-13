---
name: testing
description: Guia para testes (unitários, BDD, integração), cobertura, build e qualidade. Use quando a tarefa envolver testes, xUnit, BDD, SpecFlow, cobertura, mutation testing ou validação de build.
---

# Testing — Qualidade e Cobertura

## Quando Usar Esta Skill

Use quando a tarefa envolver:
- Criar ou modificar **testes** (unitários, BDD, integração)
- **xUnit**, **SpecFlow**, **BDD**, cobertura de código
- **Build**, **CI/CD**, validação de qualidade
- Palavras-chave: "teste", "test", "BDD", "cobertura", "coverage", "xUnit", "build", "validar"

## 1. Estrutura de Projetos de Teste

```
<Projeto>.Tests.Unit/
  Domain/
    Entities/
      UserTests.cs
    ValueObjects/
      EmailTests.cs
  Application/
    UseCases/
      User/
        CreateUserUseCaseTests.cs
    Validators/
      User/
        CreateUserInputValidatorTests.cs
  Infra/
    Repositories/
      UserRepositoryTests.cs

<Projeto>.Tests.Bdd/
  Features/
    User/
      CreateUser.feature
      CreateUser.steps.cs
  Hooks/
    TestHooks.cs
  Support/
    TestContext.cs

<Projeto>.Tests.Integration/
  Api/
    Controllers/
      UserControllerTests.cs
  Setup/
    WebApplicationFactoryFixture.cs
```

## 2. Testes Unitários — xUnit

### Estrutura Básica

```csharp
using Xunit;
using FluentAssertions;
using Moq;

public class CreateUserUseCaseTests
{
    private readonly Mock<IUserRepository> _repositoryMock;
    private readonly Mock<ITokenService> _tokenServiceMock;
    private readonly CreateUserUseCase _sut; // System Under Test

    public CreateUserUseCaseTests()
    {
        _repositoryMock = new Mock<IUserRepository>();
        _tokenServiceMock = new Mock<ITokenService>();
        _sut = new CreateUserUseCase(_repositoryMock.Object, _tokenServiceMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_WhenInputIsValid_ShouldCreateUser()
    {
        // Arrange
        var input = new CreateUserInput("test@test.com", "Test User", "Password123", "+5511999999999", UserRole.User);
        var expectedUser = new User { Id = Guid.NewGuid(), Email = input.Email, Name = input.Name };
        
        _repositoryMock.Setup(x => x.ExistsAsync(input.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        
        _repositoryMock.Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedUser);

        // Act
        var result = await _sut.ExecuteAsync(input);

        // Assert
        result.Should().NotBeNull();
        result.Email.Should().Be(input.Email);
        result.Name.Should().Be(input.Name);
        
        _repositoryMock.Verify(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WhenEmailAlreadyExists_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var input = new CreateUserInput("existing@test.com", "Test", "Pass123", "+5511999999999", UserRole.User);
        _repositoryMock.Setup(x => x.ExistsAsync(input.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act & Assert
        await _sut.Invoking(x => x.ExecuteAsync(input))
            .Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Email já está em uso.");
        
        _repositoryMock.Verify(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData(null)]
    public async Task ExecuteAsync_WhenEmailIsNullOrEmpty_ShouldThrowArgumentException(string? email)
    {
        // Arrange
        var input = new CreateUserInput(email!, "Test", "Pass123", "+5511999999999", UserRole.User);

        // Act & Assert
        await _sut.Invoking(x => x.ExecuteAsync(input))
            .Should().ThrowAsync<ArgumentException>();
    }
}
```

### FluentAssertions

```csharp
// Comparações
result.Should().Be(expected);
result.Should().NotBe(unexpected);
result.Should().BeNull();
result.Should().NotBeNull();

// Strings
result.Should().StartWith("prefix");
result.Should().EndWith("suffix");
result.Should().Contain("substring");
result.Should().BeNullOrEmpty();

// Números
result.Should().BeGreaterThan(0);
result.Should().BeLessThan(100);
result.Should().BeInRange(1, 10);

// Coleções
items.Should().HaveCount(3);
items.Should().Contain(expectedItem);
items.Should().NotContain(unexpectedItem);
items.Should().BeEmpty();
items.Should().ContainSingle();
items.Should().OnlyHaveUniqueItems();

// Exceções
action.Should().Throw<InvalidOperationException>()
    .WithMessage("Expected message");

await asyncAction.Should().ThrowAsync<ArgumentException>()
    .WithMessage("Parameter cannot be null");

// Tipos
result.Should().BeOfType<User>();
result.Should().BeAssignableTo<IUser>();

// Objetos
user.Should().BeEquivalentTo(expectedUser);
user.Should().BeEquivalentTo(expectedUser, options => options.Excluding(x => x.Id));
```

### Moq

```csharp
// Setup
_mock.Setup(x => x.Method(It.IsAny<string>())).Returns("result");
_mock.Setup(x => x.Method(It.Is<int>(i => i > 0))).Returns(true);
_mock.Setup(x => x.MethodAsync(It.IsAny<CancellationToken>())).ReturnsAsync(result);

// Setup com callback
_mock.Setup(x => x.Method(It.IsAny<string>()))
    .Callback<string>(param => Console.WriteLine(param))
    .Returns("result");

// Setup exceção
_mock.Setup(x => x.Method()).Throws<InvalidOperationException>();
_mock.Setup(x => x.MethodAsync()).ThrowsAsync(new InvalidOperationException());

// Verify
_mock.Verify(x => x.Method(), Times.Once);
_mock.Verify(x => x.Method(), Times.Never);
_mock.Verify(x => x.Method(), Times.Exactly(3));
_mock.Verify(x => x.Method(It.IsAny<string>()), Times.AtLeastOnce);

// VerifyAll (verifica todos os setups foram chamados)
_mock.VerifyAll();

// VerifyNoOtherCalls
_mock.VerifyNoOtherCalls();
```

## 3. Testes BDD — SpecFlow

### Feature File

```gherkin
# Features/User/CreateUser.feature
Feature: Create User
  As a system administrator
  I want to create new users
  So that they can access the system

Background:
  Given the user repository is available
  And the token service is available

Scenario: Create user with valid data
  Given I have a valid user input with email "test@test.com"
  And the email is not already registered
  When I execute the create user use case
  Then the user should be created successfully
  And the response should contain the user email "test@test.com"
  And the user repository create method should be called once

Scenario: Attempt to create user with existing email
  Given I have a valid user input with email "existing@test.com"
  And the email is already registered
  When I execute the create user use case
  Then an InvalidOperationException should be thrown
  And the exception message should be "Email já está em uso."
  And the user repository create method should not be called

Scenario Outline: Attempt to create user with invalid email
  Given I have a user input with email "<email>"
  When I execute the create user use case
  Then an ArgumentException should be thrown

  Examples:
    | email |
    |       |
    |       |
    | invalid-email |
```

### Step Definitions

```csharp
using TechTalk.SpecFlow;
using FluentAssertions;
using Moq;

[Binding]
public class CreateUserSteps
{
    private readonly ScenarioContext _context;
    private readonly Mock<IUserRepository> _repositoryMock;
    private readonly Mock<ITokenService> _tokenServiceMock;
    private CreateUserUseCase _useCase;
    private CreateUserInput _input;
    private CreateUserResponseModel? _result;
    private Exception? _exception;

    public CreateUserSteps(ScenarioContext context)
    {
        _context = context;
        _repositoryMock = new Mock<IUserRepository>();
        _tokenServiceMock = new Mock<ITokenService>();
    }

    [Given(@"the user repository is available")]
    public void GivenTheUserRepositoryIsAvailable()
    {
        _useCase = new CreateUserUseCase(_repositoryMock.Object, _tokenServiceMock.Object);
    }

    [Given(@"the token service is available")]
    public void GivenTheTokenServiceIsAvailable()
    {
        // Setup básico do token service
        _tokenServiceMock.Setup(x => x.GenerateTokenAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("token");
    }

    [Given(@"I have a valid user input with email ""(.*)""")]
    public void GivenIHaveAValidUserInputWithEmail(string email)
    {
        _input = new CreateUserInput(email, "Test User", "Password123", "+5511999999999", UserRole.User);
    }

    [Given(@"the email is not already registered")]
    public void GivenTheEmailIsNotAlreadyRegistered()
    {
        _repositoryMock.Setup(x => x.ExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        
        _repositoryMock.Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User user, CancellationToken ct) => user);
    }

    [Given(@"the email is already registered")]
    public void GivenTheEmailIsAlreadyRegistered()
    {
        _repositoryMock.Setup(x => x.ExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
    }

    [When(@"I execute the create user use case")]
    public async Task WhenIExecuteTheCreateUserUseCase()
    {
        try
        {
            _result = await _useCase.ExecuteAsync(_input);
        }
        catch (Exception ex)
        {
            _exception = ex;
        }
    }

    [Then(@"the user should be created successfully")]
    public void ThenTheUserShouldBeCreatedSuccessfully()
    {
        _result.Should().NotBeNull();
        _exception.Should().BeNull();
    }

    [Then(@"the response should contain the user email ""(.*)""")]
    public void ThenTheResponseShouldContainTheUserEmail(string expectedEmail)
    {
        _result!.Email.Should().Be(expectedEmail);
    }

    [Then(@"the user repository create method should be called once")]
    public void ThenTheUserRepositoryCreateMethodShouldBeCalledOnce()
    {
        _repositoryMock.Verify(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Then(@"an InvalidOperationException should be thrown")]
    public void ThenAnInvalidOperationExceptionShouldBeThrown()
    {
        _exception.Should().NotBeNull();
        _exception.Should().BeOfType<InvalidOperationException>();
    }

    [Then(@"the exception message should be ""(.*)""")]
    public void ThenTheExceptionMessageShouldBe(string expectedMessage)
    {
        _exception!.Message.Should().Be(expectedMessage);
    }

    [Then(@"the user repository create method should not be called")]
    public void ThenTheUserRepositoryCreateMethodShouldNotBeCalled()
    {
        _repositoryMock.Verify(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
```

## 4. Testes de Integração — WebApplicationFactory

### Fixture

```csharp
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

public class WebApplicationFactoryFixture : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remover DbContext real
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            // Adicionar DbContext in-memory
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseInMemoryDatabase(Guid.NewGuid().ToString());
            });

            // Build service provider e criar banco
            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            context.Database.EnsureCreated();
        });
    }
}
```

### Testes

```csharp
public class UserControllerIntegrationTests : IClassFixture<WebApplicationFactoryFixture>
{
    private readonly HttpClient _client;

    public UserControllerIntegrationTests(WebApplicationFactoryFixture factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CreateUser_WithValidData_ReturnsOkAndUser()
    {
        // Arrange
        var request = new CreateUserInput("test@test.com", "Test User", "Password123", "+5511999999999", UserRole.User);
        var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/users", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var responseBody = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<ApiResponse<CreateUserResponseModel>>(responseBody, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        
        result.Should().NotBeNull();
        result!.Success.Should().BeTrue();
        result.Data.Email.Should().Be("test@test.com");
    }

    [Fact]
    public async Task CreateUser_WithInvalidEmail_ReturnsBadRequest()
    {
        // Arrange
        var request = new CreateUserInput("invalid-email", "Test", "Pass123", "+5511999999999", UserRole.User);
        var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/users", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
```

## 5. Cobertura de Código

### Executar Testes com Cobertura

```bash
# Rodar testes e coletar cobertura
dotnet test --collect:"XPlat Code Coverage"

# Gerar relatório HTML (instalar ReportGenerator)
dotnet tool install -g dotnet-reportgenerator-globaltool

reportgenerator \
  -reports:"**/coverage.cobertura.xml" \
  -targetdir:"coveragereport" \
  -reporttypes:Html

# Abrir relatório
start coveragereport/index.html  # Windows
open coveragereport/index.html   # macOS
xdg-open coveragereport/index.html  # Linux
```

### Configuração no .csproj

```xml
<PropertyGroup>
  <CollectCoverage>true</CollectCoverage>
  <CoverletOutputFormat>cobertura</CoverletOutputFormat>
  <CoverletOutput>./coverage/</CoverletOutput>
  <Exclude>[*.Tests.*]*</Exclude>
  <ExcludeByAttribute>Obsolete,GeneratedCodeAttribute,CompilerGeneratedAttribute</ExcludeByAttribute>
</PropertyGroup>
```

### Meta de Cobertura

- **≥ 80% de cobertura de linha** para o projeto
- **≥ 90% para Application/Domain** (lógica de negócio)
- **≥ 70% para Infra** (mais dependente de integração)

## 6. Mutation Testing — Stryker.NET

### Instalação

```bash
dotnet tool install -g dotnet-stryker
```

### Executar

```bash
# Na pasta do projeto de teste
dotnet stryker

# Gerar relatório HTML
dotnet stryker --reporter html

# Abrir relatório
start mutation-report.html
```

### Configuração (stryker-config.json)

```json
{
  "stryker-config": {
    "project": "../<Projeto>.Application/<Projeto>.Application.csproj",
    "test-projects": ["<Projeto>.Tests.Unit.csproj"],
    "reporters": ["html", "progress"],
    "thresholds": {
      "high": 80,
      "low": 60,
      "break": 60
    }
  }
}
```

## 7. Build e Validação (Pré-Finalização)

**Obrigatório antes de finalizar atividades que envolvem código:**

### Checklist

1. ✅ **Build sem erros:** `dotnet build`
2. ✅ **Todos os testes passando:** `dotnet test`
3. ✅ **Cobertura ≥ 80%:** `dotnet test --collect:"XPlat Code Coverage"`
4. ✅ **Linter sem erros críticos** (se aplicável)

### Script de Validação

```bash
#!/bin/bash
# validate.sh

echo "🔨 Building..."
dotnet build
if [ $? -ne 0 ]; then
    echo "❌ Build failed!"
    exit 1
fi

echo "🧪 Running tests..."
dotnet test --collect:"XPlat Code Coverage"
if [ $? -ne 0 ]; then
    echo "❌ Tests failed!"
    exit 1
fi

echo "📊 Checking coverage..."
reportgenerator -reports:"**/coverage.cobertura.xml" -targetdir:"coveragereport" -reporttypes:Html

echo "✅ All checks passed!"
```

## 8. CI/CD — GitHub Actions

```yaml
name: CI

on:
  push:
    branches: [ main, dev ]
  pull_request:
    branches: [ main, dev ]

jobs:
  build-and-test:
    runs-on: ubuntu-latest

    steps:
    - uses: actions/checkout@v3
    
    - name: Setup .NET
      uses: actions/setup-dotnet@v3
      with:
        dotnet-version: '10.0.x'
    
    - name: Restore dependencies
      run: dotnet restore
    
    - name: Build
      run: dotnet build --no-restore --configuration Release
    
    - name: Test
      run: dotnet test --no-build --configuration Release --collect:"XPlat Code Coverage"
    
    - name: Code Coverage Report
      uses: irongut/CodeCoverageSummary@v1.3.0
      with:
        filename: '**/coverage.cobertura.xml'
        badge: true
        fail_below_min: true
        format: markdown
        hide_branch_rate: false
        hide_complexity: true
        indicators: true
        output: both
        thresholds: '80 90'
    
    - name: Add Coverage PR Comment
      uses: marocchino/sticky-pull-request-comment@v2
      if: github.event_name == 'pull_request'
      with:
        recreate: true
        path: code-coverage-results.md
```

## 9. Boas Práticas

### Nomenclatura

- ✅ **Classe de teste:** `<ClasseTestada>Tests`
- ✅ **Método de teste:** `<Metodo>_When<Condicao>_Should<Resultado>`
- ✅ **BDD steps:** Given/When/Then claro e descritivo

### Estrutura AAA

```csharp
[Fact]
public async Task Method_WhenCondition_ShouldBehavior()
{
    // Arrange — preparar dados e mocks
    var input = new Input();
    _mock.Setup(...).Returns(...);

    // Act — executar o método testado
    var result = await _sut.MethodAsync(input);

    // Assert — verificar resultado
    result.Should().Be(expected);
    _mock.Verify(...);
}
```

### O Que Testar

- ✅ **Domain:** entidades, value objects, invariantes
- ✅ **Application:** UseCases, validators, presenters
- ✅ **Infra:** repositórios (in-memory ou test containers)
- ✅ **API:** controllers (integração com WebApplicationFactory)

### O Que NÃO Testar

- ❌ Getters/setters triviais
- ❌ Construtores vazios
- ❌ Classes geradas automaticamente
- ❌ Configuração de DI (testar via integração)

### Isolamento

- ✅ **Cada teste é independente** (não dependem de ordem)
- ✅ **Limpar estado** entre testes (fixtures, in-memory DB)
- ✅ **Mockar dependências externas** (banco, APIs, clock)

### Performance

- ✅ **Testes rápidos:** unitários devem rodar em milissegundos
- ✅ **Testes de integração separados:** usar `[Trait("Category", "Integration")]`
- ✅ **Evitar Thread.Sleep:** usar mocks ou timeouts controlados

## 10. Exemplos Completos

### Testar Domain Entity

```csharp
public class UserTests
{
    [Fact]
    public void Create_WithValidData_ShouldCreateUser()
    {
        // Act
        var user = new User("test@test.com", "Test User");

        // Assert
        user.Email.Should().Be("test@test.com");
        user.Name.Should().Be("Test User");
        user.IsActive.Should().BeTrue();
        user.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData(null)]
    public void Create_WithInvalidEmail_ShouldThrowArgumentException(string? email)
    {
        // Act & Assert
        Action act = () => new User(email!, "Test");
        act.Should().Throw<ArgumentException>().WithMessage("*email*");
    }
}
```

### Testar Validator

```csharp
public class CreateUserInputValidatorTests
{
    private readonly CreateUserInputValidator _validator = new();

    [Fact]
    public void Should_Not_Have_Error_When_Input_Is_Valid()
    {
        var input = new CreateUserInput("test@test.com", "Test", "Pass123", "+5511999999999", UserRole.User);
        var result = _validator.TestValidate(input);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_Have_Error_When_Email_Is_Invalid()
    {
        var input = new CreateUserInput("invalid", "Test", "Pass123", "+5511999999999", UserRole.User);
        var result = _validator.TestValidate(input);
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }
}
```

---

## Resumo

Esta skill cobre:
- ✅ Testes unitários (xUnit, FluentAssertions, Moq)
- ✅ Testes BDD (SpecFlow, Gherkin)
- ✅ Testes de integração (WebApplicationFactory)
- ✅ Cobertura de código (≥ 80%)
- ✅ Mutation testing (Stryker.NET)
- ✅ Build e validação pré-finalização
- ✅ CI/CD (GitHub Actions)
- ✅ Boas práticas e nomenclatura

Sempre que trabalhar com **testes, cobertura, build ou validação de qualidade**, use esta skill como referência.
