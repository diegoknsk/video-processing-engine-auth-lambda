# Subtask-02: Testes Unitários de AuthController e UserController

## Descrição
Criar testes unitários para `AuthController` (endpoint `POST /login`) e `UserController` (endpoint `POST /users/create`), que atualmente possuem 0% de cobertura (12 linhas não cobertas cada). Os testes devem verificar que os controllers delegam corretamente para os UseCases e retornam os status HTTP esperados.

## Passos de Implementação
1. Criar `tests/VideoProcessing.Auth.Tests.Unit/Controllers/Auth/AuthControllerTests.cs`:
   - Mockar `LoginUseCase` e `ILogger<AuthController>` com Moq.
   - Testar `Login_WhenInputIsValid_ShouldReturnOkWithResult`: setup do mock retornando `LoginResponseModel`, verificar que retorna `OkObjectResult` com o resultado correto e que o mock foi chamado `Times.Once`.
   - Testar `Login_WhenUseCaseThrows_ShouldPropagateException`: setup do mock lançando exceção, verificar que ela se propaga (o middleware cuida do tratamento).
2. Criar `tests/VideoProcessing.Auth.Tests.Unit/Controllers/Auth/UserControllerTests.cs`:
   - Mockar `CreateUserUseCase` e `ILogger<UserController>` com Moq.
   - Testar `Create_WhenInputIsValid_ShouldReturn201WithResult`: verificar `ObjectResult` com `StatusCode == 201` e conteúdo correto.
   - Testar `Create_WhenUseCaseThrows_ShouldPropagateException`: verificar propagação de exceção.
3. Garantir que todos os testes seguem o padrão AAA e nomenclatura `MethodName_Scenario_ExpectedResult`.

## Formas de Teste
- Executar `dotnet test` e verificar que os novos testes passam sem falhas.
- Verificar via cobertura que `AuthController.cs` e `UserController.cs` atingem ≥ 90%.
- Verificar com `_mock.Verify(...)` que os UseCases são chamados exatamente uma vez por ação.

## Critérios de Aceite
- Arquivo `AuthControllerTests.cs` criado com ≥ 2 testes cobrindo sucesso e propagação de exceção.
- Arquivo `UserControllerTests.cs` criado com ≥ 2 testes cobrindo sucesso (status 201) e propagação de exceção.
- `AuthController.cs` e `UserController.cs` com cobertura ≥ 90% no relatório do coverlet.
- `dotnet test` executa com 0 falhas.
