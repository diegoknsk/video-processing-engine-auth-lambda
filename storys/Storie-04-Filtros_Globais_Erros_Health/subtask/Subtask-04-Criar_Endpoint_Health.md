# Subtask 04: Criar Endpoint GET /health

## Descrição
Criar o controller `HealthController` com endpoint GET /health que retorna status "Healthy" e timestamp UTC, permitindo fácil monitoramento e health check da aplicação (útil para Load Balancers, Kubernetes probes, etc.).

## Passos de Implementação
1. Criar classe `HealthController` em `Api/Controllers/HealthController.cs` herdando de `ControllerBase` com atributo `[ApiController]` e `[Route("health")]`
2. Criar método de ação:
   ```csharp
   /// <summary>
   /// Health check endpoint.
   /// </summary>
   /// <returns>Status da aplicação.</returns>
   /// <response code="200">Aplicação está saudável.</response>
   [HttpGet]
   [ProducesResponseType(StatusCodes.Status200OK)]
   public IActionResult Health()
   {
       return Ok(new { status = "Healthy", timestamp = DateTime.UtcNow });
   }
   ```
3. Endpoint NÃO deve exigir autenticação (público); útil para health checks externos
4. Adicionar XML comments para documentação Swagger
5. (Opcional, evolução futura) Adicionar health checks de dependências (ex.: verificar conectividade com Cognito, variáveis de ambiente necessárias) usando `Microsoft.Extensions.Diagnostics.HealthChecks`
6. Testar endpoint localmente com `dotnet run` e curl/Postman

## Formas de Teste
1. Executar `dotnet run` localmente e chamar `GET /health` com curl/Postman; verificar 200 OK com `{ "status": "Healthy", "timestamp": "..." }`
2. Criar teste unitário `HealthControllerTests` que:
   - Instancia o controller
   - Chama método `Health()`
   - Verifica que retorno é `OkObjectResult` com status code 200
   - Verifica que valor contém `status = "Healthy"` e `timestamp` presente
3. Verificar que resposta NÃO é encapsulada em `ApiResponse<T>` (health check geralmente retorna formato simples; se encapsulado, ajustar filtro para excluir /health, ou aceitar encapsulamento)
4. Executar `dotnet test` e verificar que teste passa

## Formas de Teste (continuação)
5. Verificar que endpoint está documentado no Swagger/Scalar UI
6. (Opcional) Testar com ferramentas de monitoramento (ex.: configurar probe em Kubernetes ou ELB health check apontando para /health)

## Critérios de Aceite da Subtask
- [ ] Classe `HealthController` criada com atributo `[ApiController]` e `[Route("health")]`
- [ ] Método `Health` implementado com rota `[HttpGet]` retornando `{ "status": "Healthy", "timestamp": "..." }`
- [ ] Endpoint público (não requer autenticação)
- [ ] XML comments adicionados para documentação Swagger
- [ ] Teste unitário criado verificando resposta 200 OK com campos corretos
- [ ] Teste manual com curl/Postman bem-sucedido
- [ ] Endpoint aparece no Swagger/Scalar UI
- [ ] `dotnet build` e `dotnet test` executam sem erros
