# Subtask 03: Implementar GlobalExceptionMiddleware

## Descrição
Implementar o middleware `GlobalExceptionMiddleware` que captura todas as exceções não tratadas (incluindo exceções do Cognito SDK), mapeia para códigos HTTP apropriados, e retorna `ApiErrorResponse` padronizado com mensagens amigáveis e sem vazar detalhes internos.

## Passos de Implementação
1. Criar classe `GlobalExceptionMiddleware` em `Api/Middleware/GlobalExceptionMiddleware.cs` com construtor recebendo `RequestDelegate next` e `ILogger<GlobalExceptionMiddleware>`
2. Implementar método `InvokeAsync(HttpContext context)`:
   - Usar try-catch em torno de `await _next(context)`
   - Capturar exceções e mapear para `(int statusCode, string code, string message)` baseado no tipo:
     - `NotAuthorizedException` (Cognito) → 401, "InvalidCredentials", "Credenciais inválidas."
     - `UserNotFoundException` (Cognito) → 401, "InvalidCredentials", "Credenciais inválidas." (não vazar que usuário não existe)
     - `UsernameExistsException` (Cognito) → 409, "UserAlreadyExists", "Usuário já existe."
     - `InvalidPasswordException` (Cognito) → 422, "InvalidPassword", "Senha não atende aos requisitos de política."
     - `TooManyRequestsException` (Cognito) → 429, "TooManyRequests", "Limite de requisições excedido."
     - `InvalidParameterException` (Cognito) → 400, "InvalidParameter", "Parâmetro inválido."
     - `AmazonCognitoIdentityProviderException` (genérico) → 502, "ExternalServiceError", "Erro ao comunicar com serviço de autenticação."
     - `UnauthorizedAccessException` (.NET) → 401, "Unauthorized", "Acesso não autorizado."
     - `ArgumentException` (.NET) → 400, "BadRequest", "Requisição inválida."
     - Exceção genérica → 500, "InternalServerError", "Erro interno do servidor."
   - Criar `ApiErrorResponse` com o mapeamento acima
   - Definir `context.Response.StatusCode = statusCode` e `context.Response.ContentType = "application/json"`
   - Serializar `ApiErrorResponse` para JSON e escrever em `context.Response.Body` usando `JsonSerializer.SerializeAsync`
   - Adicionar log estruturado: `_logger.LogError(exception, "Unhandled exception: {ExceptionType} - {Message}", exception.GetType().Name, exception.Message)`
3. Registrar middleware no `Program.cs` ANTES de `UseRouting()`:
   ```csharp
   app.UseMiddleware<GlobalExceptionMiddleware>();
   ```
4. Importar namespace `Amazon.CognitoIdentityProvider.Model` para exceções do Cognito
5. Considerar usar `IExceptionHandler` (.NET 8+) como alternativa ao middleware customizado (decisão de design; middleware é mais flexível)

## Formas de Teste
1. Criar teste unitário `GlobalExceptionMiddlewareTests` que:
   - Mocka `HttpContext`, `HttpRequest`, `HttpResponse`, `RequestDelegate`
   - Simula `RequestDelegate` lançando diferentes exceções (NotAuthorizedException, UsernameExistsException, Exception genérica)
   - Chama `InvokeAsync` do middleware
   - Verifica que `HttpContext.Response.StatusCode` está correto
   - Verifica que corpo da resposta contém `ApiErrorResponse` serializado com code e message esperados
2. Testar todos os mapeamentos de exceção (mínimo 10 cenários)
3. Executar aplicação localmente e forçar erro em controller (ex.: lançar exceção manualmente); verificar resposta 500 com formato correto
4. Executar `dotnet test` e verificar que testes passam

## Critérios de Aceite da Subtask
- [ ] Classe `GlobalExceptionMiddleware` criada com método `InvokeAsync` capturando exceções
- [ ] Mapeamento completo de exceções do Cognito (NotAuthorizedException, UsernameExistsException, InvalidPasswordException, TooManyRequestsException, InvalidParameterException, genérica AmazonCognitoIdentityProviderException) e exceções .NET (UnauthorizedAccessException, ArgumentException, Exception)
- [ ] Respostas de erro retornam `ApiErrorResponse` com formato `{ "success": false, "error": { "code": "...", "message": "..." }, "timestamp": "..." }`
- [ ] Mensagens de erro são amigáveis e não vazam detalhes internos (ex.: stack trace, mensagens técnicas do Cognito)
- [ ] Logs estruturados adicionados para cada exceção capturada (LogError com tipo e mensagem)
- [ ] Middleware registrado no `Program.cs` antes de `UseRouting()`
- [ ] Testes unitários criados cobrindo todos os mapeamentos de exceção; cobertura ≥ 80%
- [ ] Teste manual com exceção forçada confirma funcionamento correto
- [ ] `dotnet build` e `dotnet test` executam sem erros
