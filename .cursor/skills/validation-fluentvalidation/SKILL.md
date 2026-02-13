---
name: validation-fluentvalidation
description: Guia para validação de InputModels com FluentValidation — regras, configuração, filtros e integração com Clean Architecture. Use quando a tarefa envolver validação, FluentValidation, validators, InputModels ou regras de validação.
---

# Validation — FluentValidation

## Quando Usar Esta Skill

Use quando a tarefa envolver:
- Criar ou modificar **validators**
- **FluentValidation**, validação de **InputModels**
- Regras de validação, mensagens de erro
- Palavras-chave: "validar", "validação", "FluentValidation", "validator", "InputModel", "regras"

## 1. Princípios de Validação

### O Que Validators Validam

- ✅ **Forma e consistência local** do input:
  - Required (NotEmpty, NotNull)
  - Tamanho (MinimumLength, MaximumLength)
  - Formato (EmailAddress, Matches)
  - Ranges (GreaterThan, LessThan)
  - Enums (IsInEnum)
  - Listas não vazias

### O Que Validators NÃO Validam

- ❌ **Não** acessar banco de dados
- ❌ **Não** chamar services externos
- ❌ **Não** validar route parameters ou headers (são validados pelo binding e filtros)
- ❌ **Não** conter regras de negócio complexas (ficam no UseCase/Domain)

**Regra de ouro:** Validators validam apenas propriedades do InputModel (dados do body).

## 2. Estrutura

```
Application/
  Validators/
    <Contexto>/
      CreateUserInputValidator.cs
      UpdateUserInputValidator.cs
```

## 3. Exemplo Completo: Validator de Criação de Usuário

### InputModel

```csharp
public record CreateUserInput(
    string Email,
    string Name,
    string Password,
    string PhoneNumber,
    UserRole Role
);

public enum UserRole
{
    Admin,
    User,
    Guest
}
```

### Validator

```csharp
using FluentValidation;

public class CreateUserInputValidator : AbstractValidator<CreateUserInput>
{
    public CreateUserInputValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email é obrigatório.")
            .EmailAddress()
            .WithMessage("Email em formato inválido.")
            .MaximumLength(200)
            .WithMessage("Email não pode ter mais de 200 caracteres.");

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Nome é obrigatório.")
            .MinimumLength(3)
            .WithMessage("Nome deve ter pelo menos 3 caracteres.")
            .MaximumLength(200)
            .WithMessage("Nome não pode ter mais de 200 caracteres.");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Senha é obrigatória.")
            .MinimumLength(8)
            .WithMessage("Senha deve ter pelo menos 8 caracteres.")
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)")
            .WithMessage("Senha deve conter pelo menos uma letra maiúscula, uma minúscula e um número.");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .WithMessage("Telefone é obrigatório.")
            .Matches(@"^\+?[1-9]\d{1,14}$")
            .WithMessage("Telefone em formato inválido (use formato E.164).");

        RuleFor(x => x.Role)
            .IsInEnum()
            .WithMessage("Role inválida.");
    }
}
```

## 4. Regras Comuns

### Obrigatório

```csharp
RuleFor(x => x.Email).NotEmpty();           // String não vazia
RuleFor(x => x.Id).NotEmpty();              // Guid não vazio
RuleFor(x => x.User).NotNull();             // Objeto não nulo
```

### Strings

```csharp
RuleFor(x => x.Name)
    .NotEmpty()
    .MinimumLength(3)
    .MaximumLength(200);

RuleFor(x => x.Email)
    .NotEmpty()
    .EmailAddress();

RuleFor(x => x.Url)
    .Must(BeAValidUrl)
    .WithMessage("URL inválida.");

private bool BeAValidUrl(string? url)
{
    return Uri.TryCreate(url, UriKind.Absolute, out _);
}
```

### Números

```csharp
RuleFor(x => x.Age)
    .GreaterThan(0)
    .LessThan(150);

RuleFor(x => x.Price)
    .GreaterThanOrEqualTo(0)
    .WithMessage("Preço não pode ser negativo.");

RuleFor(x => x.Quantity)
    .InclusiveBetween(1, 1000);
```

### Guid

```csharp
RuleFor(x => x.Id)
    .NotEmpty()
    .WithMessage("Id é obrigatório.");
```

### Enum

```csharp
RuleFor(x => x.Status)
    .IsInEnum()
    .WithMessage("Status inválido.");
```

### Listas

```csharp
RuleFor(x => x.Items)
    .NotEmpty()
    .WithMessage("Lista de items não pode ser vazia.");

RuleFor(x => x.Items)
    .Must(x => x.Count <= 100)
    .WithMessage("Máximo de 100 items permitidos.");

// Validar cada item da lista
RuleForEach(x => x.Items)
    .ChildRules(item =>
    {
        item.RuleFor(x => x.Name).NotEmpty();
        item.RuleFor(x => x.Quantity).GreaterThan(0);
    });
```

### Regex

```csharp
RuleFor(x => x.PhoneNumber)
    .Matches(@"^\+?[1-9]\d{1,14}$")
    .WithMessage("Telefone em formato inválido.");

RuleFor(x => x.ZipCode)
    .Matches(@"^\d{5}-\d{3}$")
    .WithMessage("CEP deve estar no formato 00000-000.");
```

### Datas

```csharp
RuleFor(x => x.BirthDate)
    .LessThan(DateTime.Now)
    .WithMessage("Data de nascimento não pode ser no futuro.");

RuleFor(x => x.StartDate)
    .GreaterThanOrEqualTo(DateTime.Today)
    .WithMessage("Data de início deve ser hoje ou no futuro.");

RuleFor(x => x.EndDate)
    .GreaterThan(x => x.StartDate)
    .WithMessage("Data de término deve ser após a data de início.");
```

### Condicionais

```csharp
// Validar apenas quando condição for verdadeira
RuleFor(x => x.CompanyName)
    .NotEmpty()
    .When(x => x.IsCompany);

// Validar de formas diferentes baseado em condição
RuleFor(x => x.TaxId)
    .Must(BeValidCpf).When(x => !x.IsCompany)
    .Must(BeValidCnpj).When(x => x.IsCompany);
```

### Custom Validators

```csharp
RuleFor(x => x.Email)
    .Must(NotBeAlreadyRegistered)
    .WithMessage("Email já está em uso.");

private bool NotBeAlreadyRegistered(string email)
{
    // ❌ NÃO fazer isso em validators (não acessar banco)
    // return !_repository.ExistsAsync(email).Result;
    
    // ✅ Regra de negócio complexa (ex.: email único) fica no UseCase
    return true;
}

// Melhor abordagem: validação de formato no Validator, validação de unicidade no UseCase
```

## 5. Configuração e Registro

### Program.cs

```csharp
using FluentValidation;

// Registrar FluentValidation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();

// Registrar todos os validators do assembly
builder.Services.AddValidatorsFromAssemblyContaining<CreateUserInputValidator>();

// Ou registrar assembly da Application
builder.Services.AddValidatorsFromAssembly(typeof(ApplicationAssemblyMarker).Assembly);
```

### Filtro de Validação (Opcional)

Criar filtro para converter erros de validação no formato padrão da API:

```csharp
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

public class ValidationFilter : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (!context.ModelState.IsValid)
        {
            var errors = context.ModelState
                .Where(x => x.Value?.Errors.Count > 0)
                .SelectMany(x => x.Value!.Errors.Select(e => new
                {
                    Field = x.Key,
                    Message = e.ErrorMessage
                }))
                .ToList();

            var response = new
            {
                Success = false,
                Message = "Validation failed",
                Errors = errors
            };

            context.Result = new BadRequestObjectResult(response);
            return;
        }

        await next();
    }
}

// Registrar
builder.Services.AddControllers(options =>
{
    options.Filters.Add<ValidationFilter>();
});
```

## 6. Validators para InputModels "Vazios"

Quando **todos** os dados vêm de rota/headers (não há body), ainda criar um validator:

```csharp
public record GetUserInput
{
    // Nenhuma propriedade — tudo vem de rota/headers
}

public class GetUserInputValidator : AbstractValidator<GetUserInput>
{
    public GetUserInputValidator()
    {
        // Validator vazio — route params e headers validados por binding e filtro
        // Este validator existe apenas para manter a convenção
    }
}
```

**Comentário explicativo** no validator deixa claro por que está vazio.

## 7. InputModels com Dados de Rota

Quando um campo vem da rota, use `[JsonIgnore]` para não deserializar do body:

```csharp
public record UpdateUserInput
{
    [JsonIgnore]
    public Guid Id { get; init; } // Preenchido pelo Controller (rota)

    public required string Name { get; init; }
    public required string Email { get; init; }
}

public class UpdateUserInputValidator : AbstractValidator<UpdateUserInput>
{
    public UpdateUserInputValidator()
    {
        // NÃO validar Id aqui — vem da rota, validado pelo binding
        
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(200);
    }
}

// Controller
[HttpPut("{id}")]
public async Task<IActionResult> UpdateUserAsync(Guid id, [FromBody] UpdateUserInput input)
{
    input = input with { Id = id }; // Preencher Id da rota
    var result = await _useCase.ExecuteAsync(input);
    return Ok(result);
}
```

## 8. Testes de Validators

```csharp
using FluentValidation.TestHelper;
using Xunit;

public class CreateUserInputValidatorTests
{
    private readonly CreateUserInputValidator _validator = new();

    [Fact]
    public void Should_Have_Error_When_Email_Is_Empty()
    {
        // Arrange
        var input = new CreateUserInput(
            Email: "",
            Name: "Test User",
            Password: "Pass123",
            PhoneNumber: "+5511999999999",
            Role: UserRole.User
        );

        // Act
        var result = _validator.TestValidate(input);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage("Email é obrigatório.");
    }

    [Fact]
    public void Should_Have_Error_When_Email_Is_Invalid()
    {
        var input = new CreateUserInput("invalid-email", "Test", "Pass123", "+5511999999999", UserRole.User);
        var result = _validator.TestValidate(input);
        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage("Email em formato inválido.");
    }

    [Fact]
    public void Should_Not_Have_Error_When_Input_Is_Valid()
    {
        var input = new CreateUserInput(
            "test@test.com",
            "Test User",
            "Password123",
            "+5511999999999",
            UserRole.User
        );
        
        var result = _validator.TestValidate(input);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("ab")]           // muito curto
    [InlineData("password")]     // sem maiúscula e número
    [InlineData("PASSWORD123")]  // sem minúscula
    [InlineData("Password")]     // sem número
    public void Should_Have_Error_When_Password_Is_Weak(string password)
    {
        var input = new CreateUserInput("test@test.com", "Test", password, "+5511999999999", UserRole.User);
        var result = _validator.TestValidate(input);
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }
}
```

## 9. Boas Práticas

- ✅ **Sempre** criar validator para cada InputModel (mesmo que vazio)
- ✅ **Sempre** validar apenas propriedades do InputModel (dados do body)
- ✅ **Sempre** usar mensagens de erro descritivas em português (ou idioma do projeto)
- ✅ **Sempre** usar `WithMessage()` para customizar mensagens
- ✅ **Sempre** escrever testes para os validators
- ✅ Usar validators para validações de formato e estrutura
- ✅ Deixar regras de negócio complexas para o UseCase/Domain
- ❌ **Nunca** acessar banco de dados no validator
- ❌ **Nunca** chamar services externos no validator
- ❌ **Nunca** validar route params ou headers no validator
- ❌ **Nunca** colocar lógica de negócio complexa no validator

## 10. Integração com Clean Architecture

### Fluxo de Validação

```
1. Request chega ao Controller
2. FluentValidation valida o InputModel automaticamente
3. Se inválido: retorna 400 com erros (via filtro)
4. Se válido: Controller chama UseCase
5. UseCase executa regras de negócio (ex.: email único)
6. Se regra de negócio falhar: UseCase lança exceção de domínio
7. Middleware de exceções converte em resposta apropriada
```

### Separação de Responsabilidades

| Camada | Responsabilidade |
|--------|-----------------|
| **Validator** | Formato, estrutura, ranges, required |
| **UseCase** | Regras de negócio, orquestração, validação com banco |
| **Domain** | Invariantes, validações de entidades |

**Exemplo:**

```csharp
// ✅ Validator: formato
RuleFor(x => x.Email).NotEmpty().EmailAddress();

// ✅ UseCase: regra de negócio (email único)
public async Task<CreateUserResponseModel> ExecuteAsync(CreateUserInput input, CancellationToken ct = default)
{
    // Input já validado (formato)
    
    // Validar regra de negócio
    if (await _userRepository.ExistsAsync(input.Email, ct))
    {
        throw new InvalidOperationException("Email já está em uso.");
    }

    var user = await _userRepository.CreateAsync(input, ct);
    return CreateUserPresenter.Present(user);
}
```

## 11. Mensagens de Erro Padronizadas

### Criar Resource File (Opcional)

```csharp
// ValidationMessages.resx
EmailRequired = "Email é obrigatório."
EmailInvalid = "Email em formato inválido."
NameRequired = "Nome é obrigatório."
// ...

// Validator
RuleFor(x => x.Email)
    .NotEmpty()
    .WithMessage(ValidationMessages.EmailRequired)
    .EmailAddress()
    .WithMessage(ValidationMessages.EmailInvalid);
```

### Placeholders

```csharp
RuleFor(x => x.Name)
    .MaximumLength(200)
    .WithMessage("Nome não pode ter mais de {MaxLength} caracteres."); // {MaxLength} é substituído automaticamente

RuleFor(x => x.Age)
    .InclusiveBetween(18, 120)
    .WithMessage("Idade deve estar entre {From} e {To}.");
```

---

## Resumo

Esta skill cobre:
- ✅ Princípios de validação (formato vs. regras de negócio)
- ✅ FluentValidation para InputModels
- ✅ Regras comuns (required, strings, números, enums, listas, datas, regex)
- ✅ Configuração e registro
- ✅ Testes de validators
- ✅ Integração com Clean Architecture
- ✅ Boas práticas e separação de responsabilidades

Sempre que trabalhar com **validação, FluentValidation, validators ou InputModels**, use esta skill como referência.
