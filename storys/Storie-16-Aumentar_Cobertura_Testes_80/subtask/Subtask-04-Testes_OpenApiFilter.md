# Subtask-04: Testes Unitários de OpenApiServerFromRequestFilter

## Descrição
Criar testes unitários para `OpenApiServerFromRequestFilter`, que possui 0% de cobertura (26 linhas não cobertas, 20 condições). O filtro popula a lista de `Servers` do documento OpenAPI a partir do request HTTP. Os testes devem cobrir todos os branches: HttpContext nulo, host vazio, stage presente/ausente, pathBase com/sem barra inicial.

## Passos de Implementação
1. Criar `tests/VideoProcessing.Auth.Tests.Unit/OpenApi/OpenApiServerFromRequestFilterTests.cs`.
2. Criar helper privado `BuildFilter(HttpContext? ctx)` que monta o mock de `IHttpContextAccessor` e retorna a instância do filtro.
3. Implementar os seguintes testes:
   - `Apply_WhenHttpContextIsNull_ShouldNotAddServer`: `httpContextAccessor.HttpContext = null`; verificar que `document.Servers` permanece vazio.
   - `Apply_WhenRequestIsNull_ShouldNotAddServer`: HttpContext com Request nulo (via mock `DefaultHttpContext` customizado); verificar que `document.Servers` permanece vazio.
   - `Apply_WhenHostIsEmpty_ShouldNotAddServer`: request com `Host = new HostString("")`; verificar que nenhum servidor é adicionado.
   - `Apply_WhenNoStageAndNoPathBase_ShouldBuildSimpleUrl`: request com scheme `https`, host `api.example.com`, sem stage, sem pathBase; verificar URL `https://api.example.com`.
   - `Apply_WhenStageIsSet_ShouldIncludeStageInUrl`: setar variável de ambiente `GATEWAY_STAGE=dev`, verificar que a URL contém `/dev`.
   - `Apply_WhenPathBaseIsSet_ShouldIncludePathBase`: request com `PathBase = "/auth"`, verificar que a URL contém `/auth`.
   - `Apply_WhenStageAndPathBaseAreSet_ShouldCombineCorrectly`: stage `dev`, pathBase `/auth`, verificar URL `https://host/dev/auth`.
   - `Apply_WhenStageHasLeadingSlash_ShouldNormalizeIt`: stage `/dev` (com barra), verificar que não duplica barra.
4. Usar `DefaultHttpContext` do ASP.NET Core para criar contextos de teste; mockar apenas `IHttpContextAccessor`.
5. Limpar/restaurar variáveis de ambiente nos testes com `try/finally` ou `IDisposable`.

## Formas de Teste
- Executar `dotnet test` e verificar que todos os novos testes passam.
- Verificar cobertura de `OpenApiServerFromRequestFilter.cs` ≥ 85% no relatório coverlet.
- Verificar que os valores de `document.Servers[0].Url` correspondem exatamente ao esperado em cada cenário.

## Critérios de Aceite
- Arquivo `OpenApiServerFromRequestFilterTests.cs` criado com ≥ 7 testes cobrindo todos os branches principais.
- `OpenApiServerFromRequestFilter.cs` com cobertura ≥ 85% no relatório do coverlet.
- Variáveis de ambiente usadas nos testes são restauradas ao estado original ao final de cada teste.
- `dotnet test` executa com 0 falhas.
