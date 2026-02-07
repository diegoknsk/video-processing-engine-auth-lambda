# Subtask 02: Implementar ApiResponseFilter

## Descrição
Implementar o action filter `ApiResponseFilter` que intercepta respostas de sucesso (200, 201) dos controllers e automaticamente encapsula o resultado em `ApiResponse<T>`, evitando que controllers tenham que criar manualmente o envelope.

## Passos de Implementação
1. Criar classe `ApiResponseFilter` em `Api/Filters/ApiResponseFilter.cs` implementando `IActionFilter` (ou `IAsyncActionFilter` se necessário)
2. Implementar método `OnActionExecuted` (ou `OnActionExecutionAsync`):
   - Verificar se `context.Result` é `OkObjectResult` ou `ObjectResult` com status 200/201
   - Se sim, extrair o valor de `result.Value`
   - Criar `ApiResponse<object>` com `Success = true`, `Data = result.Value`, `Timestamp = DateTime.UtcNow`
   - Substituir `context.Result` por novo `ObjectResult` com o envelope
3. Registrar o filtro globalmente no `Program.cs`:
   ```csharp
   builder.Services.AddControllers(options =>
   {
       options.Filters.Add<ApiResponseFilter>();
   });
   ```
4. Adicionar logs estruturados (opcional) para debug: `_logger.LogDebug("ApiResponseFilter applied to action {ActionName}", context.ActionDescriptor.DisplayName)`
5. Considerar usar atributo `[ProducesResponseType]` nos controllers para documentar o tipo encapsulado no Swagger

## Formas de Teste
1. Criar teste unitário `ApiResponseFilterTests` que:
   - Cria um `ActionExecutedContext` mockado com `OkObjectResult` contendo um objeto de teste
   - Chama o filtro
   - Verifica que `context.Result` foi substituído por `ObjectResult` com `ApiResponse<T>` e propriedades corretas (Success = true, Data com objeto original, Timestamp presente)
2. Testar cenário onde `context.Result` é outro tipo (ex.: `BadRequestResult`) e verificar que filtro NÃO modifica
3. Executar aplicação localmente e chamar endpoint existente (ex.: POST /auth/login); verificar que resposta está encapsulada
4. Executar `dotnet test` e verificar que testes passam

## Critérios de Aceite da Subtask
- [ ] Classe `ApiResponseFilter` implementa `IActionFilter` ou `IAsyncActionFilter`
- [ ] Filtro encapsula respostas 200/201 em `ApiResponse<T>` automaticamente
- [ ] Filtro NÃO modifica outras respostas (400, 401, etc. — tratadas pelo middleware de exceções)
- [ ] Filtro registrado globalmente em `Program.cs` via `options.Filters.Add<ApiResponseFilter>()`
- [ ] Testes unitários criados cobrindo cenários de sucesso e não-aplicação do filtro
- [ ] Teste manual com endpoint real confirma encapsulamento correto
- [ ] `dotnet build` e `dotnet test` executam sem erros
