# Subtask 04: Ajustar testes unitários

## Descrição
Ajustar todos os testes unitários que dependem de `CreateUserInput.Email` ou da assinatura `SignUpAsync(username, password, email)`: validadores, use case e serviço de SignUp. Garantir que os testes usem username no formato de email onde aplicável e que os mocks e asserts estejam alinhados à nova assinatura e comportamento.

## Passos de implementação
1. **CreateUserInputValidatorTests**: remover ou reescrever testes que validam o campo Email (Validate_WhenEmailIsEmpty_ShouldHaveValidationError, Validate_WhenEmailIsInvalid_ShouldHaveValidationError, Validate_WhenEmailExceedsMaxLength_ShouldHaveValidationError, Validate_WhenEmailIsValid_ShouldNotHaveValidationError). Adicionar/ajustar testes para validar que Username deve ser email: exemplo com username vazio, username inválido (não-email), username email válido, username excedendo tamanho máximo. Ajustar testes existentes de Username que usavam formato alfanumérico para usar email (ex.: "user@example.com") onde o caso espera sucesso.
2. **CognitoAuthServiceSignUpTests**: em todas as chamadas a `SignUpAsync`, remover o terceiro argumento (email); usar o mesmo valor de email como username onde hoje há username e email separados (ex.: username = "test@example.com", sem variável email separada). No teste que verifica UserAttributes (SignUpAsync_ShouldIncludeEmailInUserAttributes), manter o assert de que o atributo "email" está presente com valor igual ao username passado.
3. **CreateUserUseCaseTests**: remover a propriedade `Email` de todos os `CreateUserInput` dos testes; usar `Username = "test@example.com"` (ou similar) onde for necessário. Ajustar mocks de `SignUpAsync` para receber apenas (username, password, It.IsAny<CancellationToken>()) e, nos setups, usar a assinatura de dois parâmetros + CancellationToken.

## Formas de teste
- Executar `dotnet test` na raiz da solução; todos os testes devem passar.
- Nenhum teste deve referenciar `CreateUserInput.Email` ou `SignUpAsync(..., email)`.

## Critérios de aceite da subtask
- [ ] CreateUserInputValidatorTests atualizados: sem testes do campo Email; testes de Username como email (válido/inválido) passando.
- [ ] CognitoAuthServiceSignUpTests passando com SignUpAsync(username, password) e username no formato email.
- [ ] CreateUserUseCaseTests passando com input sem Email e mocks ajustados.
- [ ] `dotnet test` conclui sem falhas.
