# Subtask 01: Criar LoginInput e LoginInputValidator

## Descrição
Criar o InputModel `LoginInput` com propriedades Username e Password, e implementar o validator `LoginInputValidator` usando FluentValidation para validar obrigatoriedade e tamanhos.

## Passos de Implementação
1. Criar classe `LoginInput` em `Application/InputModels/Auth/LoginInput.cs` com propriedades:
   - `string Username { get; init; }`
   - `string Password { get; init; }`
2. Criar classe `LoginInputValidator` em `Application/Validators/Auth/LoginInputValidator.cs` herdando de `AbstractValidator<LoginInput>`
3. Implementar regras de validação:
   - Username: `NotEmpty()` com mensagem "Username é obrigatório.", `MaximumLength(128)` com mensagem "Username deve ter no máximo 128 caracteres."
   - Password: `NotEmpty()` com mensagem "Password é obrigatório.", `MinimumLength(8)` com mensagem "Password deve ter pelo menos 8 caracteres.", `MaximumLength(256)` com mensagem "Password deve ter no máximo 256 caracteres."
4. Registrar validator no DI (já feito via assembly scanning na Story 01; verificar que está funcionando)
5. Adicionar XML comments em LoginInput para documentação Swagger (se aplicável)

## Formas de Teste
1. Criar teste unitário `LoginInputValidatorTests` que verifica cada regra de validação (username vazio → erro, password curto → erro, input válido → sem erros)
2. Executar `dotnet test` e verificar que testes passam
3. Criar endpoint temporário que aceita LoginInput e testar via Postman/curl com payloads inválidos (verificar 400 Bad Request com mensagens corretas)
4. Inspecionar resposta de erro e confirmar formato: `{ "success": false, "errors": [{ "field": "username", "message": "..." }] }`

## Critérios de Aceite da Subtask
- [ ] Classe `LoginInput` criada com propriedades Username e Password (record ou class com init)
- [ ] `LoginInputValidator` implementado com 5 regras de validação (NotEmpty, MaximumLength para username; NotEmpty, MinimumLength, MaximumLength para password)
- [ ] Testes unitários criados cobrindo todas as regras de validação; cobertura ≥ 80%
- [ ] Validator é descoberto automaticamente pelo FluentValidation (assembly scanning); testado via endpoint temporário ou teste de integração
- [ ] Mensagens de erro claras e em português brasileiro
- [ ] `dotnet build` e `dotnet test` executam sem erros
