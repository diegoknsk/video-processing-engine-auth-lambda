# Subtask 03: Implementar CognitoAuthService.LoginAsync

## Descrição
Implementar a classe `CognitoAuthService` no projeto Infra que implementa `ICognitoAuthService`, incluindo inicialização do SDK do Cognito, cálculo condicional de SECRET_HASH, chamada ao `InitiateAuthAsync` com USER_PASSWORD_AUTH, e mapeamento de exceções do Cognito.

## Passos de Implementação
1. Criar classe `SecretHashCalculator` em `Infra/Models/` com método estático:
   ```csharp
   public static string ComputeSecretHash(string username, string clientId, string clientSecret)
   {
       var message = username + clientId;
       using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(clientSecret));
       var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(message));
       return Convert.ToBase64String(hash);
   }
   ```
2. Criar classe `CognitoAuthService` em `Infra/Services/CognitoAuthService.cs` implementando `ICognitoAuthService` com construtor primário injetando `IOptions<CognitoOptions>` e `ILogger<CognitoAuthService>`
3. Criar campo privado `IAmazonCognitoIdentityProvider _cognitoClient` inicializado no construtor com `new AmazonCognitoIdentityProviderClient(RegionEndpoint.GetBySystemName(options.Value.Region))`
4. Implementar `LoginAsync`:
   - Verificar se `COGNITO_APP_CLIENT_SECRET` está presente em options; se sim, calcular SECRET_HASH usando `SecretHashCalculator`
   - Criar `InitiateAuthRequest` com `ClientId`, `AuthFlow = AuthFlowType.USER_PASSWORD_AUTH`, `AuthParameters` contendo USERNAME, PASSWORD e SECRET_HASH (se aplicável)
   - Chamar `await _cognitoClient.InitiateAuthAsync(request, cancellationToken)`
   - Mapear `AuthenticationResult` para `LoginOutput` (tokens, expiresIn, tokenType)
   - Tratar exceções: capturar `NotAuthorizedException` e `UserNotFoundException` e lançar `UnauthorizedAccessException` com mensagem genérica "Credenciais inválidas"
5. Adicionar logs estruturados: `_logger.LogInformation("Attempting login for user {Username}", username)` antes da chamada; `_logger.LogWarning("Login failed for user {Username}", username)` em caso de erro
6. Registrar `CognitoAuthService` como implementação de `ICognitoAuthService` no DI do `Program.cs`: `builder.Services.AddScoped<ICognitoAuthService, CognitoAuthService>()`

## Formas de Teste
1. Criar teste unitário `CognitoAuthServiceTests` mockando `IAmazonCognitoIdentityProvider` (criar interface wrapper ou usar Moq com classe concreta)
2. Testar cenário de sucesso: mock retorna `InitiateAuthResponse` válido; verificar que `LoginOutput` é mapeado corretamente
3. Testar cenário de erro: mock lança `NotAuthorizedException`; verificar que `UnauthorizedAccessException` é lançada
4. Testar cálculo de SECRET_HASH: criar teste unitário para `SecretHashCalculator.ComputeSecretHash` com valores conhecidos e verificar hash esperado
5. Executar `dotnet test` e verificar cobertura ≥ 80%

## Critérios de Aceite da Subtask
- [ ] Classe `SecretHashCalculator` criada com método `ComputeSecretHash` funcionando corretamente (teste com valores conhecidos)
- [ ] Classe `CognitoAuthService` implementa `ICognitoAuthService` com construtor primário injetando `IOptions<CognitoOptions>` e `ILogger`
- [ ] Método `LoginAsync` inicializa SDK, calcula SECRET_HASH condicionalmente, chama `InitiateAuthAsync` com USER_PASSWORD_AUTH
- [ ] Mapeamento de `AuthenticationResult` para `LoginOutput` funciona corretamente (tokens, expiresIn, tokenType)
- [ ] Exceções do Cognito (`NotAuthorizedException`, `UserNotFoundException`) mapeadas para `UnauthorizedAccessException` com mensagem genérica
- [ ] Logs estruturados adicionados (tentativa de login, falha); NUNCA logar password, secret ou tokens
- [ ] Testes unitários criados com cobertura ≥ 80%; mock do SDK funciona corretamente
- [ ] `CognitoAuthService` registrado no DI como `Scoped`
- [ ] `dotnet build` e `dotnet test` executam sem erros
