# Subtask 06: Criar UserController com Endpoint POST /auth/users/create

## Descrição
Criar o Controller `UserController` no projeto Api com endpoint POST /auth/users/create que recebe CreateUserInput via [FromBody], chama CreateUserUseCase e retorna 201 Created com CreateUserResponseModel (encapsulado pelo filtro global).

## Passos de Implementação
1. Criar classe `UserController` em `Api/Controllers/Auth/UserController.cs` herdando de `ControllerBase` com atributo `[ApiController]` e `[Route("auth/users")]`
2. Adicionar construtor primário injetando `CreateUserUseCase` e `ILogger<UserController>`
3. Criar método de ação:
   ```csharp
   /// <summary>
   /// Cria novo usuário no sistema.
   /// </summary>
   /// <param name="input">Dados de criação (username, password, email).</param>
   /// <param name="cancellationToken">Token de cancelamento.</param>
   /// <returns>Informações do usuário criado (userId, username, userConfirmed, confirmationRequired).</returns>
   /// <response code="201">Usuário criado com sucesso.</response>
   /// <response code="400">Dados de entrada inválidos.</response>
   /// <response code="409">Usuário já existe.</response>
   /// <response code="422">Senha não atende aos requisitos de política do pool.</response>
   [HttpPost("create")]
   [ProducesResponseType(typeof(CreateUserResponseModel), StatusCodes.Status201Created)]
   [ProducesResponseType(StatusCodes.Status400BadRequest)]
   [ProducesResponseType(StatusCodes.Status409Conflict)]
   [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
   public async Task<IActionResult> Create([FromBody] CreateUserInput input, CancellationToken cancellationToken)
   {
       _logger.LogInformation("POST /auth/users/create called for username {Username}", input.Username);
       
       var result = await _createUserUseCase.ExecuteAsync(input, cancellationToken);
       
       return StatusCode(StatusCodes.Status201Created, result);
   }
   ```
4. Adicionar XML comments completos para Swagger
5. Verificar que ModelState é validado automaticamente pelo FluentValidation (já configurado na Story 01)
6. Testar endpoint localmente com `dotnet run` e curl/Postman

## Formas de Teste
1. Executar `dotnet run` localmente e testar endpoint com Postman/curl:
   - Request válido: `POST /auth/users/create` com `{ "username": "testuser", "password": "Test@1234", "email": "test@example.com" }` → 201 Created
   - Request inválido: `{ "username": "", "password": "123", "email": "notanemail" }` → 400 Bad Request com erros de validação
   - Request com usuário existente (se possível simular) → 409 Conflict
2. Criar teste de integração (opcional) usando `WebApplicationFactory` para testar endpoint sem chamar Cognito real (mockar service no DI)
3. Verificar que resposta é encapsulada em `ApiResponse<CreateUserResponseModel>` pelo filtro global
4. Verificar logs estruturados no console (entrada no endpoint, chamada ao UseCase)

## Critérios de Aceite da Subtask
- [ ] Classe `UserController` criada com atributos `[ApiController]` e `[Route("auth/users")]`
- [ ] Método `Create` implementado com rota `[HttpPost("create")]`, recebendo `[FromBody] CreateUserInput` e `CancellationToken`
- [ ] XML comments completos adicionados (summary, param, returns, response codes 201, 400, 409, 422)
- [ ] Controller chama `CreateUserUseCase.ExecuteAsync` e retorna `StatusCode(201, result)`
- [ ] Validação automática de ModelState funciona (FluentValidation); 400 retornado quando input inválido
- [ ] Logs estruturados adicionados no início da ação; NUNCA logar password
- [ ] Teste manual com Postman/curl bem-sucedido (201 Created)
- [ ] Resposta encapsulada em `ApiResponse<T>` pelo filtro global
- [ ] `dotnet build` e `dotnet run` executam sem erros
