# Subtask 05: Implementar LoginUseCase

## Descrição
Implementar o UseCase `LoginUseCase` no projeto Application que orquestra o fluxo de login: recebe LoginInput, chama ICognitoAuthService, obtém LoginOutput, chama LoginPresenter e retorna LoginResponseModel.

## Passos de Implementação
1. Criar classe `LoginUseCase` em `Application/UseCases/Auth/LoginUseCase.cs` com construtor primário injetando `ICognitoAuthService` e `ILogger<LoginUseCase>`
2. Criar método público:
   ```csharp
   public async Task<LoginResponseModel> ExecuteAsync(LoginInput input, CancellationToken cancellationToken = default)
   {
       _logger.LogInformation("Executing login use case for user {Username}", input.Username);
       
       var output = await _cognitoAuthService.LoginAsync(input.Username, input.Password, cancellationToken);
       
       _logger.LogInformation("Login successful for user {Username}", input.Username);
       
       return LoginPresenter.Present(output);
   }
   ```
3. Tratar exceções: deixar exceções do service (UnauthorizedAccessException) propagarem para serem capturadas pelo middleware global
4. Adicionar logs estruturados no início e fim da execução (sucesso); evitar log em caso de exceção (middleware já loga)
5. Registrar `LoginUseCase` no DI do `Program.cs` como `Scoped`: `builder.Services.AddScoped<LoginUseCase>()`

## Formas de Teste
1. Criar teste unitário `LoginUseCaseTests` com cenários:
   - **Sucesso:** mockar `ICognitoAuthService.LoginAsync` retornando `LoginOutput` válido; verificar que `ExecuteAsync` retorna `LoginResponseModel` correto
   - **Erro de autenticação:** mockar service lançando `UnauthorizedAccessException`; verificar que exceção propaga (UseCase não trata)
   - **Cancellation:** passar `CancellationToken` cancelado e verificar que `OperationCanceledException` é lançada (ou comportamento adequado)
2. Usar Moq para mockar `ICognitoAuthService` e `ILogger`
3. Usar FluentAssertions para verificar valores retornados
4. Executar `dotnet test` e verificar cobertura ≥ 80%

## Critérios de Aceite da Subtask
- [ ] Classe `LoginUseCase` criada com construtor primário injetando `ICognitoAuthService` e `ILogger`
- [ ] Método `ExecuteAsync` implementado: recebe LoginInput, chama service, chama presenter, retorna LoginResponseModel
- [ ] Logs estruturados adicionados (início e sucesso da execução); NUNCA logar password ou tokens
- [ ] Exceções do service propagam para middleware global (UseCase não trata exceções de negócio)
- [ ] `LoginUseCase` registrado no DI como Scoped
- [ ] Testes unitários criados cobrindo cenários de sucesso, erro e cancellation; cobertura ≥ 80%
- [ ] `dotnet build` e `dotnet test` executam sem erros
