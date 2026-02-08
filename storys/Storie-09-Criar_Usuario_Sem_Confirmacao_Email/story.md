# Storie-09: Criar Usuário sem Confirmação de Email (AdminCreateUser)

## Status
- **Estado:** ✅ Concluída
- **Data de Conclusão:** 08/02/2026

## Descrição
Como desenvolvedor do backend, quero que a criação de usuário via API deixe o usuário já disponível para login (CONFIRMED), para que em ambiente acadêmico não seja necessário confirmar email antes de obter o token.

## Objetivo
Alterar o fluxo de criação de usuário no Cognito de **SignUp** para **AdminCreateUser + AdminSetUserPassword**, de modo que o usuário seja criado já confirmado (email_verified = true, senha permanente), sem envio de email de convite/verificação, permitindo login imediato após o cadastro.

## Abordagem
Usar **AdminCreateUser** com `MessageAction = SUPPRESS`, `UserAttributes` (email, name, **email_verified = "true"**), `TemporaryPassword`; em seguida **AdminSetUserPassword** com a mesma senha e `Permanent = true`. O usuário fica CONFIRMED e pode logar na hora, sem envio de email. Exige credenciais IAM com `cognito-idp:AdminCreateUser` e `cognito-idp:AdminSetUserPassword` no User Pool.

## Escopo Técnico
- Tecnologias: .NET 10, ASP.NET Core, AWSSDK.CognitoIdentityProvider (já instalado)
- Arquivos afetados:
  - `src/VideoProcessing.Auth.Infra/Services/CognitoAuthService.cs` (trocar SignUp por AdminCreateUser + AdminSetUserPassword)
  - `src/VideoProcessing.Auth.Application/Ports/ICognitoAuthService.cs` (atualizar XML/docs se necessário; assinatura do método pode permanecer SignUpAsync)
  - `tests/VideoProcessing.Auth.Tests.Unit/Services/CognitoAuthServiceSignUpTests.cs` (ajustar mocks para AdminCreateUser e AdminSetUserPassword)
- Componentes: CognitoAuthService, ICognitoAuthService
- Pacotes/Dependências: AWSSDK.CognitoIdentityProvider (já existente); nenhum pacote novo.
- Pré-requisito infra: **CognitoOptions** já possui `UserPoolId`; a aplicação deve rodar com credenciais IAM que tenham `cognito-idp:AdminCreateUser` e `cognito-idp:AdminSetUserPassword` no User Pool utilizado.

## Dependências e Riscos (para estimativa)
- Dependências: Storie-03 (criação de usuário) e Storie-07 (CognitoOptions com UserPoolId) concluídas.
- Riscos/Pré-condições:
  - User Pool configurado com `username_attributes = ["email"]` (ou equivalente) para login com email.
  - IAM: política com `AdminCreateUser` e `AdminSetUserPassword` no recurso do User Pool (documentar no README ou na story).

## Subtasks
- [x] [Subtask 01: Implementar AdminCreateUser + AdminSetUserPassword no CognitoAuthService](./subtask/Subtask-01-AdminCreateUser_AdminSetUserPassword_Service.md)
- [x] [Subtask 02: Tratamento de exceções e compatibilidade de resposta](./subtask/Subtask-02-Excecoes_Resposta_CreateUser.md)
- [x] [Subtask 03: Ajustar testes unitários do serviço de criação de usuário](./subtask/Subtask-03-Testes_Unitarios_AdminCreateUser.md)
- [x] [Subtask 04: Documentar permissões IAM no README](./subtask/Subtask-04-Documentar_IAM_README.md)

## Critérios de Aceite da História
- [x] Criação de usuário usa **AdminCreateUser** com `UserPoolId`, `Username` (email), `UserAttributes` (email, name, email_verified = "true"), `MessageAction = SUPPRESS` e `TemporaryPassword`.
- [x] Imediatamente após AdminCreateUser é chamado **AdminSetUserPassword** com mesma senha e `Permanent = true`.
- [x] Usuário criado fica CONFIRMED e é possível fazer **login** (InitiateAuth) logo após o create, sem confirmação de email.
- [x] Comportamento da API (201 Created, corpo com userId, username, userConfirmed, confirmationRequired) mantido; `userConfirmed` deve ser true e `confirmationRequired` false.
- [x] Tratamento de erros existente preservado (409 para usuário já existe, 422 para senha inválida, 400 para parâmetro inválido).
- [x] Testes unitários atualizados e passando; mocks cobrem AdminCreateUser e AdminSetUserPassword.
- [x] README (ou documentação da story) descreve as permissões IAM necessárias: `cognito-idp:AdminCreateUser` e `cognito-idp:AdminSetUserPassword`.

## Rastreamento (dev tracking)
- **Início:** 08/02/2026, às 17:59 (Brasília)
- **Fim:** 08/02/2026, às 18:09 (Brasília)
- **Tempo total de desenvolvimento:** 10min
