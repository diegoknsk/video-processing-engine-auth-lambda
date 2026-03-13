# Subtask-05: Complementar Testes de Middleware e Infra para Fechar Lacunas

## Descrição
Complementar os testes já existentes de `GatewayPathBaseMiddleware` (83.7%, 7 linhas/8 condições), `CognitoAuthService` (90.4%, 4 linhas/7 condições), `ApiResponseFilter` (94.6%, 2 linhas) e `GlobalExceptionMiddleware` (97.1%, 1 linha/1 condição) para elevar cada um a ≥ 95%. Esses ganhos complementares, combinados com as subtasks anteriores, consolidam a cobertura total acima de 80%.

## Passos de Implementação
1. **GatewayPathBaseMiddleware** — analisar o relatório de cobertura para identificar os 7 branches descobertos (provavelmente condições de `GATEWAY_PATH_PREFIX` vazio, com barra, sem barra, e next delegate). Adicionar testes para os cenários faltantes em `GatewayPathBaseMiddlewareTests.cs`.
2. **CognitoAuthService** — identificar os 4 branches descobertos (provavelmente exceptions específicas do Cognito ou condições de resposta não testadas). Adicionar os cenários faltantes em `CognitoAuthServiceTests.cs` e/ou `CognitoAuthServiceSignUpTests.cs`.
3. **ApiResponseFilter** — identificar as 2 linhas descobertas (provavelmente um branch de `context.Result` nulo ou tipo não esperado). Adicionar teste específico em `ApiResponseFilterTests.cs`.
4. **GlobalExceptionMiddleware** — identificar a 1 linha/condição descoberta (provavelmente uma exception específica não mockada). Adicionar teste em `GlobalExceptionMiddlewareTests.cs`.
5. Para cada item: seguir padrão AAA, usar Moq para mocks e FluentAssertions para assertions.

## Formas de Teste
- Executar `dotnet test --collect:"XPlat Code Coverage"` após as adições e verificar o relatório XML.
- Confirmar que cada arquivo atinge ≥ 95% no relatório de cobertura.
- Verificar que a cobertura total do projeto atinge ≥ 80%.

## Critérios de Aceite
- `GatewayPathBaseMiddleware.cs` com cobertura ≥ 95%.
- `CognitoAuthService.cs` com cobertura ≥ 95%.
- `ApiResponseFilter.cs` com cobertura ≥ 97%.
- `GlobalExceptionMiddleware.cs` com cobertura ≥ 99%.
- Cobertura total do projeto ≥ 80% medida pelo coverlet/SonarCloud.
- `dotnet test` executa com 0 falhas (todos os testes novos e existentes passando).
