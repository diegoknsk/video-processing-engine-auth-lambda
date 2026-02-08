# Subtask 01: Criar CreateUserInput e CreateUserInputValidator

## Descrição
Criar o InputModel `CreateUserInput` com propriedades Username, Password e Email, e implementar o validator `CreateUserInputValidator` usando FluentValidation para validar obrigatoriedade, formatos (email, username alfanumérico) e tamanhos.

## Passos de Implementação
1. Criar classe `CreateUserInput` em `Application/InputModels/Auth/CreateUserInput.cs` com propriedades:
   - `string Username { get; init; }`
   - `string Password { get; init; }`
   - `string Email { get; init; }`
2. Criar classe `CreateUserInputValidator` em `Application/Validators/Auth/CreateUserInputValidator.cs` herdando de `AbstractValidator<CreateUserInput>`
3. Implementar regras de validação:
   - Username: `NotEmpty()` ("Username é obrigatório"), `MaximumLength(128)` ("no máximo 128 caracteres"), `Matches("^[a-zA-Z0-9_-]+$")` ("pode conter apenas letras, números, _ e -")
   - Password: `NotEmpty()` ("Password é obrigatório"), `MinimumLength(8)` ("pelo menos 8 caracteres"), `MaximumLength(256)` ("no máximo 256 caracteres")
   - Email: `NotEmpty()` ("Email é obrigatório"), `EmailAddress()` ("Email em formato inválido"), `MaximumLength(256)` ("no máximo 256 caracteres")
4. Registrar validator no DI (já feito via assembly scanning na Story 01)
5. Adicionar XML comments em CreateUserInput para documentação Swagger

## Formas de Teste
1. Criar teste unitário `CreateUserInputValidatorTests` que verifica cada regra de validação:
   - Username vazio → erro
   - Username com caracteres inválidos (ex.: "user@123") → erro
   - Username válido (ex.: "user_123") → sem erro
   - Password curto (ex.: "1234567") → erro
   - Email vazio → erro
   - Email inválido (ex.: "notanemail") → erro
   - Email válido (ex.: "user@example.com") → sem erro
   - Input completamente válido → sem erros
2. Executar `dotnet test` e verificar que testes passam
3. Testar via endpoint temporário com payloads inválidos (verificar 400 Bad Request com mensagens corretas)

## Critérios de Aceite da Subtask
- [ ] Classe `CreateUserInput` criada com propriedades Username, Password, Email (record ou class com init)
- [ ] `CreateUserInputValidator` implementado com 8 regras de validação (NotEmpty, MaximumLength, Matches para username; NotEmpty, MinimumLength, MaximumLength para password; NotEmpty, EmailAddress, MaximumLength para email)
- [ ] Testes unitários criados cobrindo todas as regras de validação; cobertura ≥ 80%
- [ ] Validator é descoberto automaticamente pelo FluentValidation (assembly scanning)
- [ ] Mensagens de erro claras e em português brasileiro
- [ ] `dotnet build` e `dotnet test` executam sem erros
