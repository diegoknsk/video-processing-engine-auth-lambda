# Storie-16: Aumentar Cobertura de Testes para 80%

## Status
- **Estado:** 🔄 Em desenvolvimento
- **Data de Conclusão:** [DD/MM/AAAA]

## Descrição
Como desenvolvedor do projeto, quero aumentar a cobertura de testes unitários do projeto de 65.5% para ≥ 80%, para garantir maior confiabilidade no código, reduzir o risco de regressões e atingir o Quality Gate do SonarCloud.

## Objetivo
Criar testes unitários para os componentes sem cobertura (AuthController, UserController, ValidationFilter, OpenApiServerFromRequestFilter) e complementar os testes parciais existentes (GatewayPathBaseMiddleware, CognitoAuthService, ApiResponseFilter, GlobalExceptionMiddleware), além de excluir arquivos que não devem contar no cálculo de cobertura (Program.cs, AssemblyReference.cs).

## Escopo Técnico
- Tecnologias: .NET 10, C# 13, xUnit 2.9.2, Moq 4.20.72, FluentAssertions 7.0.0, coverlet.collector 6.0.2
- Arquivos afetados (produção — apenas para leitura/análise):
  - `src/VideoProcessing.Auth.Api/Controllers/Auth/AuthController.cs`
  - `src/VideoProcessing.Auth.Api/Controllers/Auth/UserController.cs`
  - `src/VideoProcessing.Auth.Api/Filters/ValidationFilter.cs`
  - `src/VideoProcessing.Auth.Api/OpenApi/OpenApiServerFromRequestFilter.cs`
  - `src/VideoProcessing.Auth.Api/Middleware/GatewayPathBaseMiddleware.cs`
  - `src/VideoProcessing.Auth.Infra/Services/CognitoAuthService.cs`
  - `src/VideoProcessing.Auth.Api/Filters/ApiResponseFilter.cs`
  - `src/VideoProcessing.Auth.Api/Middleware/GlobalExceptionMiddleware.cs`
- Arquivos afetados (testes — criação/atualização):
  - `tests/VideoProcessing.Auth.Tests.Unit/Controllers/Auth/AuthControllerTests.cs` (novo)
  - `tests/VideoProcessing.Auth.Tests.Unit/Controllers/Auth/UserControllerTests.cs` (novo)
  - `tests/VideoProcessing.Auth.Tests.Unit/Filters/ValidationFilterTests.cs` (novo)
  - `tests/VideoProcessing.Auth.Tests.Unit/OpenApi/OpenApiServerFromRequestFilterTests.cs` (novo)
  - `tests/VideoProcessing.Auth.Tests.Unit/Middleware/GatewayPathBaseMiddlewareTests.cs` (complementar)
  - `tests/VideoProcessing.Auth.Tests.Unit/Services/CognitoAuthServiceTests.cs` (complementar)
  - `tests/VideoProcessing.Auth.Tests.Unit/Filters/ApiResponseFilterTests.cs` (complementar)
  - `tests/VideoProcessing.Auth.Tests.Unit/Middleware/GlobalExceptionMiddlewareTests.cs` (complementar)
  - `tests/VideoProcessing.Auth.Tests.Unit/VideoProcessing.Auth.Tests.Unit.csproj` (configurar exclusões)
- Componentes/Recursos: AuthController, UserController, ValidationFilter, OpenApiServerFromRequestFilter, GatewayPathBaseMiddleware, CognitoAuthService, ApiResponseFilter, GlobalExceptionMiddleware
- Pacotes/Dependências (já existentes, sem pacotes novos):
  - xunit (2.9.2)
  - Moq (4.20.72)
  - FluentAssertions (7.0.0)
  - coverlet.collector (6.0.2)
  - coverlet.msbuild (6.0.2)

## Dependências e Riscos (para estimativa)
- Dependências: Stories 02, 03, 04, 06, 08, 09 concluídas (código de produção estabilizado).
- Riscos/Pré-condições:
  - `Program.cs` concentra 92 linhas sem cobertura — excluir via atributo `[ExcludeFromCodeCoverage]` ou configuração do coverlet é a abordagem mais segura.
  - `OpenApiServerFromRequestFilter` tem 26 linhas e 20 condições; requer mock de `IHttpContextAccessor` e `HttpContext` com valores variados (host vazio, stage presente/ausente, path base com/sem barra).
  - Nenhum pacote novo necessário.

## Subtasks
- [ ] [Subtask 01: Configurar exclusões de cobertura e excluir Program.cs e AssemblyReference.cs](./subtask/Subtask-01-Configurar_Exclusoes_Cobertura.md)
- [ ] [Subtask 02: Testes unitários de AuthController e UserController](./subtask/Subtask-02-Testes_Controllers.md)
- [ ] [Subtask 03: Testes unitários de ValidationFilter](./subtask/Subtask-03-Testes_ValidationFilter.md)
- [ ] [Subtask 04: Testes unitários de OpenApiServerFromRequestFilter](./subtask/Subtask-04-Testes_OpenApiFilter.md)
- [ ] [Subtask 05: Complementar testes de Middleware e Infra para fechar lacunas](./subtask/Subtask-05-Complementar_Testes_Middleware_Infra.md)

## Critérios de Aceite da História
- [ ] Cobertura total do projeto medida pelo SonarCloud/coverlet ≥ 80% após a execução de `dotnet test`.
- [ ] `AuthController` e `UserController` com cobertura ≥ 90% (ação de sucesso + delegação ao UseCase verificada via Moq).
- [ ] `ValidationFilter` com cobertura ≥ 95% (cenário ModelState válido e inválido com erros múltiplos cobertos).
- [ ] `OpenApiServerFromRequestFilter` com cobertura ≥ 85% (host vazio retorna early, stage presente/ausente, pathBase com/sem barra).
- [ ] `Program.cs` e `AssemblyReference.cs` excluídos do cálculo de cobertura (via atributo ou configuração do coverlet), não impactando a meta de 80%.
- [ ] `GatewayPathBaseMiddleware`, `CognitoAuthService`, `ApiResponseFilter` e `GlobalExceptionMiddleware` com cobertura ≥ 95% cada.
- [ ] Todos os testes novos e existentes passando: `dotnet test` retorna saída sem falhas.
- [ ] Nenhum pacote novo adicionado ao projeto de testes; apenas pacotes já existentes utilizados.

## Rastreamento (dev tracking)
- **Início:** 13/03/2026, às 02:12 (Brasília)
- **Fim:** —
- **Tempo total de desenvolvimento:** —
