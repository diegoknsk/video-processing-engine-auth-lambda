# Subtask 03: Implementar CognitoAuthService.SignUpAsync

## Descrição
Implementar o método `SignUpAsync` na classe `CognitoAuthService` no projeto Infra, incluindo cálculo condicional de SECRET_HASH, chamada ao SDK Cognito `SignUpAsync` com UserAttribute email, mapeamento da resposta (UserSub, UserConfirmed), e tratamento de exceções (UsernameExistsException, InvalidPasswordException).

## Passos de Implementação
1. Abrir classe `CognitoAuthService` em `Infra/Services/CognitoAuthService.cs`
2. Implementar método `SignUpAsync`:
   - Verificar se `COGNITO_APP_CLIENT_SECRET` está presente; se sim, calcular SECRET_HASH usando `SecretHashCalculator.ComputeSecretHash(username, clientId, clientSecret)`
   - Criar `SignUpRequest` com:
     - `ClientId` (de options)
     - `Username`
     - `Password`
     - `SecretHash` (se aplicável)
     - `UserAttributes` contendo `new AttributeType { Name = "email", Value = email }`
   - Chamar `await _cognitoClient.SignUpAsync(request, cancellationToken)`
   - Mapear resposta para `CreateUserOutput`:
     - `UserId = response.UserSub`
     - `Username = username`
     - `UserConfirmed = response.UserConfirmed`
     - `ConfirmationRequired = !response.UserConfirmed`
   - Tratar exceções:
     - `UsernameExistsException` → lançar exceção customizada (ex.: `ConflictException` ou mapear para 409)
     - `InvalidPasswordException` → lançar exceção customizada (ex.: `UnprocessableEntityException` ou mapear para 422)
     - `InvalidParameterException` → lançar `ArgumentException` ou equivalente (400)
3. Adicionar logs estruturados: `_logger.LogInformation("Attempting to create user {Username} with email {Email}", username, email)` antes da chamada; `_logger.LogInformation("User {Username} created successfully; UserSub: {UserSub}, UserConfirmed: {UserConfirmed}", username, userSub, userConfirmed)` após sucesso; `_logger.LogWarning("User creation failed for {Username}: {ErrorCode}", username, errorCode)` em caso de erro
4. **NUNCA** logar password ou secret

## Formas de Teste
1. Criar teste unitário `CognitoAuthServiceSignUpTests` mockando `IAmazonCognitoIdentityProvider`
2. Testar cenário de sucesso: mock retorna `SignUpResponse` válido com UserSub, UserConfirmed; verificar que `CreateUserOutput` é mapeado corretamente
3. Testar cenário UsernameExistsException: mock lança exceção; verificar que exceção apropriada (409 Conflict) é lançada
4. Testar cenário InvalidPasswordException: mock lança exceção; verificar que exceção apropriada (422 Unprocessable Entity) é lançada
5. Testar cálculo de SECRET_HASH: com e sem clientSecret presente (reutilizar `SecretHashCalculator` já testado)
6. Executar `dotnet test` e verificar cobertura ≥ 80%

## Critérios de Aceite da Subtask
- [ ] Método `SignUpAsync` implementado em `CognitoAuthService` com lógica completa (SECRET_HASH condicional, UserAttribute email, mapeamento de resposta)
- [ ] Chamada ao SDK Cognito `SignUpAsync` funciona corretamente com todos os parâmetros necessários
- [ ] Mapeamento de `SignUpResponse` para `CreateUserOutput` correto (UserSub → UserId, UserConfirmed, ConfirmationRequired)
- [ ] Exceções do Cognito (`UsernameExistsException`, `InvalidPasswordException`, `InvalidParameterException`) tratadas e mapeadas para exceções customizadas apropriadas
- [ ] Logs estruturados adicionados (tentativa de criação, sucesso, falha); NUNCA logar password ou secret
- [ ] Testes unitários criados com cobertura ≥ 80%; mock do SDK funciona corretamente
- [ ] `dotnet build` e `dotnet test` executam sem erros
