# Subtask 04: Criar CreateUserOutput, CreateUserResponseModel e CreateUserPresenter

## Descrição
Criar o OutputModel `CreateUserOutput` (contrato interno do UseCase), o ResponseModel `CreateUserResponseModel` (contrato da API), e o Presenter `CreateUserPresenter` que transforma OutputModel em ResponseModel.

## Passos de Implementação
1. Criar `CreateUserOutput` em `Application/OutputModels/Auth/CreateUserOutput.cs` com propriedades:
   - `string UserId` (UserSub do Cognito)
   - `string Username`
   - `bool UserConfirmed`
   - `bool ConfirmationRequired`
2. Criar `CreateUserResponseModel` em `Api/Models/CreateUserResponseModel.cs` (ou `Application/ResponseModels/Auth/` se preferir) com as mesmas propriedades do OutputModel (pode usar record para imutabilidade)
3. Criar `CreateUserPresenter` em `Application/Presenters/Auth/CreateUserPresenter.cs` como classe estática com método:
   ```csharp
   public static CreateUserResponseModel Present(CreateUserOutput output)
   {
       return new CreateUserResponseModel
       {
           UserId = output.UserId,
           Username = output.Username,
           UserConfirmed = output.UserConfirmed,
           ConfirmationRequired = output.ConfirmationRequired
       };
   }
   ```
4. Adicionar XML comments em CreateUserResponseModel para documentação Swagger (descrição de cada campo)
5. Atualizar `ICognitoAuthService` e `CognitoAuthService` (Subtask 03) para retornar `CreateUserOutput` (se ainda não feito)

## Formas de Teste
1. Criar teste unitário `CreateUserPresenterTests` que:
   - Cria um `CreateUserOutput` com valores de teste
   - Chama `CreateUserPresenter.Present(output)`
   - Verifica que `CreateUserResponseModel` retornado contém os mesmos valores
   - Usa FluentAssertions para asserções claras: `result.Should().BeEquivalentTo(expected)`
2. Testar cenário com `UserConfirmed = true` (ConfirmationRequired = false)
3. Testar cenário com `UserConfirmed = false` (ConfirmationRequired = true)
4. Executar `dotnet test` e verificar que teste passa

## Critérios de Aceite da Subtask
- [ ] Classe `CreateUserOutput` criada com propriedades UserId, Username, UserConfirmed, ConfirmationRequired
- [ ] Classe `CreateUserResponseModel` criada com as mesmas propriedades (record ou class)
- [ ] `CreateUserPresenter` implementado como classe estática com método `Present` que transforma OutputModel em ResponseModel
- [ ] Testes unitários do Presenter criados; cobertura 100% (método simples de mapeamento)
- [ ] XML comments adicionados em CreateUserResponseModel para documentação Swagger
- [ ] `dotnet build` e `dotnet test` executam sem erros
