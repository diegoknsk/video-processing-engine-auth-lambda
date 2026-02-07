# Subtask 05: Implementar CreateUserUseCase

## Descrição
Implementar o UseCase `CreateUserUseCase` no projeto Application que orquestra o fluxo de criação de usuário: recebe CreateUserInput, chama ICognitoAuthService.SignUpAsync, obtém CreateUserOutput, chama CreateUserPresenter e retorna CreateUserResponseModel.

## Passos de Implementação
1. Criar classe `CreateUserUseCase` em `Application/UseCases/Auth/CreateUserUseCase.cs` com construtor primário injetando `ICognitoAuthService` e `ILogger<CreateUserUseCase>`
2. Criar método público:
   ```csharp
   public async Task<CreateUserResponseModel> ExecuteAsync(CreateUserInput input, CancellationToken cancellationToken = default)
   {
       _logger.LogInformation("Executing create user use case for username {Username} and email {Email}", input.Username, input.Email);
       
       var output = await _cognitoAuthService.SignUpAsync(input.Username, input.Password, input.Email, cancellationToken);
       
       _logger.LogInformation("User {Username} created successfully; UserId: {UserId}, UserConfirmed: {UserConfirmed}", input.Username, output.UserId, output.UserConfirmed);
       
       return CreateUserPresenter.Present(output);
   }
   ```
3. Tratar exceções: deixar exceções do service (ConflictException, UnprocessableEntityException, ArgumentException) propagarem para serem capturadas pelo middleware global
4. Adicionar logs estruturados no início e fim da execução (sucesso); evitar log em caso de exceção (middleware já loga)
5. Registrar `CreateUserUseCase` no DI do `Program.cs` como `Scoped`: `builder.Services.AddScoped<CreateUserUseCase>()`

## Formas de Teste
1. Criar teste unitário `CreateUserUseCaseTests` com cenários:
   - **Sucesso:** mockar `ICognitoAuthService.SignUpAsync` retornando `CreateUserOutput` válido; verificar que `ExecuteAsync` retorna `CreateUserResponseModel` correto
   - **Erro de conflito (usuário já existe):** mockar service lançando exceção apropriada (ex.: ConflictException); verificar que exceção propaga
   - **Erro de senha inválida:** mockar service lançando exceção apropriada (ex.: UnprocessableEntityException); verificar que exceção propaga
   - **Cancellation:** passar `CancellationToken` cancelado e verificar que `OperationCanceledException` é lançada
2. Usar Moq para mockar `ICognitoAuthService` e `ILogger`
3. Usar FluentAssertions para verificar valores retornados
4. Executar `dotnet test` e verificar cobertura ≥ 80%

## Critérios de Aceite da Subtask
- [ ] Classe `CreateUserUseCase` criada com construtor primário injetando `ICognitoAuthService` e `ILogger`
- [ ] Método `ExecuteAsync` implementado: recebe CreateUserInput, chama SignUpAsync, chama presenter, retorna CreateUserResponseModel
- [ ] Logs estruturados adicionados (início e sucesso da execução); NUNCA logar password
- [ ] Exceções do service propagam para middleware global (UseCase não trata exceções de negócio)
- [ ] `CreateUserUseCase` registrado no DI como Scoped
- [ ] Testes unitários criados cobrindo cenários de sucesso, erros (conflito, senha inválida) e cancellation; cobertura ≥ 80%
- [ ] `dotnet build` e `dotnet test` executam sem erros
