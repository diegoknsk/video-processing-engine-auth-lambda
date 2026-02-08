# Subtask 03: Ajustar testes unitários do serviço de criação de usuário

## Descrição
Atualizar os testes unitários do `CognitoAuthService` que cobrem a criação de usuário (ex.: `CognitoAuthServiceSignUpTests`) para mockar `AdminCreateUserAsync` e `AdminSetUserPasswordAsync` em vez de `SignUpAsync`, e validar parâmetros e retorno.

## Passos de implementação
1. Localizar os testes que chamam `SignUpAsync` do serviço e que mockam `IAmazonCognitoIdentityProvider.Setup(x => x.SignUpAsync(...))`.
2. Substituir os setups por: `Setup(x => x.AdminCreateUserAsync(It.IsAny<AdminCreateUserRequest>(), It.IsAny<CancellationToken>()))` retornando um `AdminCreateUserResponse` com `User` preenchido (Sub, Username, etc.); e `Setup(x => x.AdminSetUserPasswordAsync(It.IsAny<AdminSetUserPasswordRequest>(), It.IsAny<CancellationToken>()))` retornando `Task.CompletedTask`.
3. Adicionar testes que verificam os parâmetros passados: MessageAction = SUPPRESS, presença de email_verified = "true" em UserAttributes, e Permanent = true no AdminSetUserPasswordRequest.
4. Ajustar testes de exceção (UsernameExistsException, InvalidPasswordException, etc.) para serem disparados a partir de AdminCreateUserAsync ou AdminSetUserPasswordAsync conforme o caso.
5. Executar `dotnet test` e garantir que todos os testes passam.

## Formas de teste
- Executar `dotnet test` no projeto de testes unitários.
- Verificar cobertura do método SignUpAsync (ou equivalente) após a mudança.
- Revisar que nenhum teste quebrado depende de SignUp.

## Critérios de aceite da subtask
- [ ] Testes de criação de usuário passam com mocks de AdminCreateUserAsync e AdminSetUserPasswordAsync.
- [ ] Pelo menos um teste verifica que AdminCreateUser é chamado com MessageAction SUPPRESS e email_verified nos atributos.
- [ ] Pelo menos um teste verifica que AdminSetUserPassword é chamado com Permanent = true.
- [ ] Testes de exceção (usuário já existe, senha inválida) atualizados e passando.
- [ ] `dotnet test` executa sem falhas no projeto VideoProcessing.Auth.Tests.Unit.
