# Subtask 01: Implementar AdminCreateUser + AdminSetUserPassword no CognitoAuthService

## Descrição
Substituir a chamada `SignUp` no `CognitoAuthService.SignUpAsync` por fluxo **AdminCreateUser** seguido de **AdminSetUserPassword**, utilizando `UserPoolId` das opções, sem enviar email (MessageAction = SUPPRESS) e com usuário já confirmado (email_verified = true, senha permanente).

## Passos de implementação
1. No método `SignUpAsync` de `CognitoAuthService`, remover o uso de `SignUpRequest`/`SignUpAsync` do cliente Cognito.
2. Montar `AdminCreateUserRequest` com: `UserPoolId = _options.UserPoolId`, `Username = email`, `TemporaryPassword = password`, `MessageAction = MessageActionType.SUPPRESS`, `UserAttributes` contendo email, name e `email_verified` com valor `"true"` (string).
3. Chamar `_cognitoClient.AdminCreateUserAsync(adminCreateUserRequest, cancellationToken)`.
4. Imediatamente após sucesso, montar `AdminSetUserPasswordRequest` com: mesmo `UserPoolId`, `Username = email`, `Password = password`, `Permanent = true`. Chamar `_cognitoClient.AdminSetUserPasswordAsync(adminSetUserPasswordRequest, cancellationToken)`.
5. Montar `CreateUserOutput` com: `UserId` do resultado do AdminCreateUser (ex.: `response.User.Username` ou Sub do User), `Username = email`, `UserConfirmed = true`, `ConfirmationRequired = false`. Retornar.

## Formas de teste
- Teste unitário: mock de `IAmazonCognitoIdentityProvider` para `AdminCreateUserAsync` e `AdminSetUserPasswordAsync`; verificar que são chamados com os parâmetros corretos (MessageAction SUPPRESS, email_verified, Permanent true).
- Teste manual: criar usuário via POST /auth/users/create e em seguida fazer POST /auth/login com o mesmo email/senha; deve retornar tokens.
- Integração: com User Pool real e IAM configurado, executar create + login e validar que não há passo de confirmação de email.

## Critérios de aceite da subtask
- [x] `CognitoAuthService.SignUpAsync` utiliza apenas `AdminCreateUserAsync` e `AdminSetUserPasswordAsync` (não chama mais `SignUpAsync`).
- [x] AdminCreateUser é chamado com `MessageAction = SUPPRESS`, `UserAttributes` com email, name e email_verified = "true", e `TemporaryPassword` com a senha informada.
- [x] AdminSetUserPassword é chamado com a mesma senha e `Permanent = true`.
- [x] Retorno mantém contrato `CreateUserOutput` com `UserConfirmed = true` e `ConfirmationRequired = false`.
