# Subtask 04: Criar LoginOutput, LoginResponseModel e LoginPresenter

## Descrição
Criar o OutputModel `LoginOutput` (contrato interno do UseCase), o ResponseModel `LoginResponseModel` (contrato da API), e o Presenter `LoginPresenter` que transforma OutputModel em ResponseModel.

## Passos de Implementação
1. Criar `LoginOutput` em `Application/OutputModels/Auth/LoginOutput.cs` com propriedades:
   - `string AccessToken`
   - `string IdToken`
   - `string? RefreshToken` (nullable, pois pode não vir dependendo do pool)
   - `int ExpiresIn`
   - `string TokenType`
2. Criar `LoginResponseModel` em `Api/Models/LoginResponseModel.cs` (ou `Application/ResponseModels/Auth/` se preferir) com as mesmas propriedades do OutputModel (pode usar record para imutabilidade)
3. Criar `LoginPresenter` em `Application/Presenters/Auth/LoginPresenter.cs` como classe estática com método:
   ```csharp
   public static LoginResponseModel Present(LoginOutput output)
   {
       return new LoginResponseModel
       {
           AccessToken = output.AccessToken,
           IdToken = output.IdToken,
           RefreshToken = output.RefreshToken,
           ExpiresIn = output.ExpiresIn,
           TokenType = output.TokenType
       };
   }
   ```
4. Adicionar XML comments em LoginResponseModel para documentação Swagger (descrição de cada token)
5. Atualizar `ICognitoAuthService` e `CognitoAuthService` (Subtask 03) para retornar `LoginOutput` (se ainda não feito)

## Formas de Teste
1. Criar teste unitário `LoginPresenterTests` que:
   - Cria um `LoginOutput` com valores de teste
   - Chama `LoginPresenter.Present(output)`
   - Verifica que `LoginResponseModel` retornado contém os mesmos valores
   - Usa FluentAssertions para asserções claras: `result.Should().BeEquivalentTo(expected)`
2. Executar `dotnet test` e verificar que teste passa
3. Compilar projeto e verificar que OutputModel e ResponseModel são usados corretamente em CognitoAuthService e (futuramente) no UseCase

## Critérios de Aceite da Subtask
- [ ] Classe `LoginOutput` criada com propriedades AccessToken, IdToken, RefreshToken (nullable), ExpiresIn, TokenType
- [ ] Classe `LoginResponseModel` criada com as mesmas propriedades (record ou class)
- [ ] `LoginPresenter` implementado como classe estática com método `Present` que transforma OutputModel em ResponseModel
- [ ] Testes unitários do Presenter criados; cobertura 100% (método simples de mapeamento)
- [ ] XML comments adicionados em LoginResponseModel para documentação Swagger
- [ ] `dotnet build` e `dotnet test` executam sem erros
