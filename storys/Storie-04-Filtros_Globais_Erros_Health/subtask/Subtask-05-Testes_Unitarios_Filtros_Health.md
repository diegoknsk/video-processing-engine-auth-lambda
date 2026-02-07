# Subtask 05: Testes Unitários dos Filtros e Health

## Descrição
Criar suite completa de testes unitários para ApiResponseFilter, GlobalExceptionMiddleware e HealthController, garantindo cobertura ≥ 80% e validando todos os cenários de aplicação de filtros, mapeamento de exceções e resposta de health check.

## Passos de Implementação
1. **ApiResponseFilterTests:**
   - Testar cenário onde `ActionResult` é `OkObjectResult` com objeto de teste; verificar que filtro encapsula em `ApiResponse<T>` com Success = true, Data correto, Timestamp presente
   - Testar cenário onde `ActionResult` é `ObjectResult` com StatusCode 201; verificar encapsulamento
   - Testar cenário onde `ActionResult` é `BadRequestResult` (400); verificar que filtro NÃO modifica (deixa para middleware)
   - Testar cenário onde `ActionResult` é `NoContentResult` (204); verificar comportamento (pode ou não encapsular, dependendo de decisão de design)
2. **GlobalExceptionMiddlewareTests:**
   - Testar mapeamento de `NotAuthorizedException` → 401, code "InvalidCredentials", mensagem genérica
   - Testar mapeamento de `UserNotFoundException` → 401, code "InvalidCredentials", mensagem genérica (não vazar)
   - Testar mapeamento de `UsernameExistsException` → 409, code "UserAlreadyExists"
   - Testar mapeamento de `InvalidPasswordException` → 422, code "InvalidPassword"
   - Testar mapeamento de `TooManyRequestsException` → 429, code "TooManyRequests"
   - Testar mapeamento de `InvalidParameterException` → 400, code "InvalidParameter"
   - Testar mapeamento de `AmazonCognitoIdentityProviderException` genérica → 502, code "ExternalServiceError"
   - Testar mapeamento de `UnauthorizedAccessException` → 401, code "Unauthorized"
   - Testar mapeamento de `ArgumentException` → 400, code "BadRequest"
   - Testar mapeamento de `Exception` genérica → 500, code "InternalServerError"
   - Verificar que resposta contém `ApiErrorResponse` serializado corretamente
   - Verificar que logs estruturados são emitidos (mock de ILogger)
3. **HealthControllerTests:**
   - Testar método `Health()` retorna `OkObjectResult` com StatusCode 200
   - Verificar que valor contém `status = "Healthy"` e `timestamp` (DateTime) presente
   - (Opcional) Testar serialização JSON do retorno
4. Executar `dotnet test --collect:"XPlat Code Coverage"` e verificar cobertura ≥ 80%

## Formas de Teste
1. Executar `dotnet test` e verificar que todos os testes passam
2. Executar `dotnet test --collect:"XPlat Code Coverage"` e gerar relatório de cobertura
3. Analisar relatório de cobertura e identificar linhas não cobertas (se houver)
4. Adicionar testes adicionais para cenários edge case se cobertura < 80%

## Critérios de Aceite da Subtask
- [ ] Suite completa de testes criada para ApiResponseFilter (mínimo 4 cenários)
- [ ] Suite completa de testes criada para GlobalExceptionMiddleware (mínimo 10 mapeamentos de exceção)
- [ ] Testes de HealthController criados (resposta 200 OK com campos corretos)
- [ ] Todos os testes passando; `dotnet test` executa sem erros
- [ ] Cobertura de código ≥ 80% medida com coverlet; relatório gerado e revisado
- [ ] Logs estruturados verificados nos testes do middleware (mock de ILogger confirma chamadas LogError)
- [ ] Nenhum warning ou erro de build nos projetos de teste
