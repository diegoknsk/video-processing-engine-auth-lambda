# Subtask 06: Criar AuthController com Endpoint POST /auth/login

## Descrição
Criar o Controller `AuthController` no projeto Api com endpoint POST /auth/login que recebe LoginInput via [FromBody], chama LoginUseCase e retorna 200 OK com LoginResponseModel (encapsulado pelo filtro global).

## Passos de Implementação
1. Criar classe `AuthController` em `Api/Controllers/Auth/AuthController.cs` herdando de `ControllerBase` com atributo `[ApiController]` e `[Route("auth")]`
2. Adicionar construtor primário injetando `LoginUseCase` e `ILogger<AuthController>`
3. Criar método de ação:
   ```csharp
   /// <summary>
   /// Autentica usuário e retorna tokens JWT.
   /// </summary>
   /// <param name="input">Credenciais de login (username e password).</param>
   /// <param name="cancellationToken">Token de cancelamento.</param>
   /// <returns>Tokens de autenticação (AccessToken, IdToken, RefreshToken, ExpiresIn).</returns>
   /// <response code="200">Login realizado com sucesso.</response>
   /// <response code="400">Dados de entrada inválidos.</response>
   /// <response code="401">Credenciais inválidas.</response>
   [HttpPost("login")]
   [ProducesResponseType(typeof(LoginResponseModel), StatusCodes.Status200OK)]
   [ProducesResponseType(StatusCodes.Status400BadRequest)]
   [ProducesResponseType(StatusCodes.Status401Unauthorized)]
   public async Task<IActionResult> Login([FromBody] LoginInput input, CancellationToken cancellationToken)
   {
       _logger.LogInformation("POST /auth/login called for user {Username}", input.Username);
       
       var result = await _loginUseCase.ExecuteAsync(input, cancellationToken);
       
       return Ok(result);
   }
   ```
4. Adicionar XML comments completos para Swagger
5. Verificar que ModelState é validado automaticamente pelo FluentValidation (já configurado na Story 01)
6. Testar endpoint localmente com `dotnet run` e curl/Postman

## Formas de Teste
1. Executar `dotnet run` localmente e testar endpoint com Postman/curl:
   - Request válido: `POST /auth/login` com `{ "username": "test", "password": "Test@1234" }` → 200 OK (ou 401 se credenciais inválidas, dependendo do Cognito real)
   - Request inválido: `{ "username": "", "password": "123" }` → 400 Bad Request com erros de validação
2. Criar teste de integração (opcional) usando `WebApplicationFactory` para testar endpoint sem chamar Cognito real (mockar service no DI)
3. Verificar que resposta é encapsulada em `ApiResponse<LoginResponseModel>` pelo filtro global
4. Verificar logs estruturados no console (entrada no endpoint, chamada ao UseCase)

## Critérios de Aceite da Subtask
- [ ] Classe `AuthController` criada com atributos `[ApiController]` e `[Route("auth")]`
- [ ] Método `Login` implementado com rota `[HttpPost("login")]`, recebendo `[FromBody] LoginInput` e `CancellationToken`
- [ ] XML comments completos adicionados (summary, param, returns, response codes)
- [ ] Controller chama `LoginUseCase.ExecuteAsync` e retorna `Ok(result)`
- [ ] Validação automática de ModelState funciona (FluentValidation); 400 retornado quando input inválido
- [ ] Logs estruturados adicionados no início da ação; NUNCA logar password
- [ ] Teste manual com Postman/curl bem-sucedido (200 OK ou 401 dependendo de credenciais)
- [ ] Resposta encapsulada em `ApiResponse<T>` pelo filtro global
- [ ] `dotnet build` e `dotnet run` executam sem erros
