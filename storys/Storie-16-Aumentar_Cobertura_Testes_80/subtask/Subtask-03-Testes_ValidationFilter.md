# Subtask-03: Testes Unitários de ValidationFilter

## Descrição
Criar testes unitários para `ValidationFilter`, que possui 0% de cobertura (20 linhas não cobertas, 4 condições). O filtro intercepta o `ModelState` antes da execução da action e retorna `400 BadRequest` com a lista de erros quando inválido. Os testes devem cobrir ModelState válido, inválido com um erro e inválido com múltiplos erros.

## Passos de Implementação
1. Criar `tests/VideoProcessing.Auth.Tests.Unit/Filters/ValidationFilterTests.cs`.
2. Testar `OnActionExecuting_WhenModelStateIsValid_ShouldNotSetResult`:
   - Criar `ActionExecutingContext` com `ModelStateDictionary` válido.
   - Invocar `OnActionExecuting` e verificar que `context.Result` permanece `null`.
3. Testar `OnActionExecuting_WhenModelStateHasOneError_ShouldReturnBadRequest`:
   - Adicionar um erro ao `ModelStateDictionary` (campo "Email", mensagem "Email inválido").
   - Verificar que `context.Result` é `BadRequestObjectResult`.
   - Verificar que o corpo contém `success = false` e a lista de erros com o campo e mensagem corretos.
4. Testar `OnActionExecuting_WhenModelStateHasMultipleErrors_ShouldReturnAllErrors`:
   - Adicionar erros em múltiplos campos e verificar que todos aparecem na resposta.
5. Testar `OnActionExecuted_ShouldDoNothing`:
   - Verificar que o método não lança exceção e não altera o contexto.
6. Usar `ActionExecutingContext` com um `ActionContext` mínimo (não precisa de WebApplicationFactory).

## Formas de Teste
- Executar `dotnet test` e verificar que os novos testes passam.
- Verificar cobertura de `ValidationFilter.cs` ≥ 95% no relatório coverlet.
- Inspecionar o objeto de resultado da resposta para garantir que os campos `field` e `message` estão corretos nos erros.

## Critérios de Aceite
- Arquivo `ValidationFilterTests.cs` criado com ≥ 4 testes (válido, 1 erro, múltiplos erros, OnActionExecuted).
- `ValidationFilter.cs` com cobertura ≥ 95% no relatório do coverlet.
- Todos os testes passando: `dotnet test` sem falhas.
- Resposta de erro verificada: `success = false` e lista de erros com `field`/`message` corretos.
