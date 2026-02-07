# Storie-04: Filtros Globais, Tratamento de Erros e Health Check

## Status
- **Estado:** 🔄 Em desenvolvimento
- **Data de Conclusão:** —

## Descrição
Como desenvolvedor do sistema, quero implementar filtros globais para padronizar respostas de sucesso e erro da API, middleware de exceções para capturar e mapear erros do Cognito e exceções gerais, e endpoint de health check, para garantir consistência, observabilidade e fácil monitoramento da aplicação.

## Objetivo
Implementar ApiResponseFilter para encapsular automaticamente todas as respostas de sucesso no formato `{ "success": true, "data": {...}, "timestamp": "..." }`, GlobalExceptionFilter (middleware) para capturar exceções (incluindo exceções do Cognito SDK) e mapear para códigos HTTP apropriados com formato `{ "success": false, "error": {...}, "timestamp": "..." }`, e endpoint GET /health retornando status "Healthy".

## Escopo Técnico
- Tecnologias: .NET 10, ASP.NET Core, middleware customizado, action filters
- Arquivos afetados:
  - `src/VideoProcessing.Auth.Api/Models/ApiResponse.cs` (novo)
  - `src/VideoProcessing.Auth.Api/Models/ApiErrorResponse.cs` (novo)
  - `src/VideoProcessing.Auth.Api/Filters/ApiResponseFilter.cs` (novo)
  - `src/VideoProcessing.Auth.Api/Middleware/GlobalExceptionMiddleware.cs` (novo)
  - `src/VideoProcessing.Auth.Api/Controllers/HealthController.cs` (novo)
  - `src/VideoProcessing.Auth.Api/Program.cs` (atualizar para registrar filtros e middleware)
  - `tests/VideoProcessing.Auth.Tests.Unit/Filters/ApiResponseFilterTests.cs` (novo)
  - `tests/VideoProcessing.Auth.Tests.Unit/Middleware/GlobalExceptionMiddlewareTests.cs` (novo)
  - `tests/VideoProcessing.Auth.Tests.Unit/Controllers/HealthControllerTests.cs` (novo)
- Componentes/Recursos: ApiResponse<T>, ApiErrorResponse, ApiResponseFilter, GlobalExceptionMiddleware, HealthController
- Pacotes/Dependências (já instalados na Story 01): Nenhum adicional necessário

## Dependências e Riscos (para estimativa)
- Dependências: Story 01 concluída (estrutura base); Stories 02 e 03 desejáveis (mas não bloqueantes, pois filtros são cross-cutting)
- Riscos/Pré-condições: Conhecimento de action filters e middleware no ASP.NET Core; familiaridade com exceções do AWS SDK (AmazonCognitoIdentityProviderException)

## Subtasks
- [Subtask 01: Criar modelos ApiResponse e ApiErrorResponse](./subtask/Subtask-01-Criar_ApiResponse_Models.md)
- [Subtask 02: Implementar ApiResponseFilter](./subtask/Subtask-02-Implementar_ApiResponseFilter.md)
- [Subtask 03: Implementar GlobalExceptionMiddleware](./subtask/Subtask-03-Implementar_GlobalExceptionMiddleware.md)
- [Subtask 04: Criar endpoint GET health](./subtask/Subtask-04-Criar_Endpoint_Health.md)
- [Subtask 05: Testes unitários dos filtros e health](./subtask/Subtask-05-Testes_Unitarios_Filtros_Health.md)

## Critérios de Aceite da História
- [ ] Todas as respostas de sucesso (200, 201) são encapsuladas automaticamente em `{ "success": true, "data": {...}, "timestamp": "..." }` pelo ApiResponseFilter
- [ ] GlobalExceptionMiddleware captura exceções do Cognito (`NotAuthorizedException` → 401, `UsernameExistsException` → 409, `InvalidPasswordException` → 422, etc.) e retorna `{ "success": false, "error": { "code": "...", "message": "..." }, "timestamp": "..." }`
- [ ] Mensagens de erro são amigáveis e não vazam detalhes internos (ex.: "Credenciais inválidas" para `UserNotFoundException` e `NotAuthorizedException`)
- [ ] GET /health retorna 200 OK com `{ "status": "Healthy", "timestamp": "..." }`
- [ ] Logs estruturados registram exceções capturadas pelo middleware global (sem dados sensíveis)
- [ ] Testes unitários: cobertura dos filtros (simular respostas e exceções), middleware (simular diferentes exceções e verificar resposta HTTP) e endpoint de health
- [ ] `dotnet build` e `dotnet test` executam sem erros; todos os testes passando

## Rastreamento (dev tracking)
- **Início:** —
- **Fim:** —
- **Tempo total de desenvolvimento:** —
