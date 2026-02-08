# Storie-08: Fix Criação de Usuário — Username como Email (Cognito)

## Status
- **Estado:** 🔄 Em desenvolvimento
- **Data de Conclusão:** [DD/MM/AAAA]

## Descrição
Como desenvolvedor do sistema, quero que o endpoint de criação de usuário use apenas username (no formato de email) e senha, para que funcione corretamente com User Pools do Cognito configurados com sign-in por email (UsernameAttributes = email), evitando o erro "Username should be an email".

## Objetivo
Remover o campo `email` do fluxo de criação de usuário e tratar o **username** como o identificador de login (email). A validação passará a exigir que o username seja um email válido; o SignUp enviará esse valor como `Username` no Cognito e, quando aplicável, como atributo `email` nos UserAttributes, compatível com User Pools que usam email como username.

## Contexto do problema
- No Cognito, quando o User Pool é criado com **sign-in por email** (UsernameAttributes = email), o parâmetro `Username` na API SignUp **deve ser o endereço de email**.
- O sistema atual enviava username (alfanumérico) e email separados; o Cognito rejeitava com `InvalidParameterException: Username should be an email`.
- Solução: um único identificador (username) que deve ser um email válido; remover o campo `Email` do contrato e da validação em todos os pontos.

## Escopo Técnico
- Tecnologias: .NET 10, ASP.NET Core, AWS SDK (AWSSDK.CognitoIdentityProvider), FluentValidation
- Arquivos afetados:
  - `src/VideoProcessing.Auth.Application/InputModels/Auth/CreateUserInput.cs` (remover propriedade Email)
  - `src/VideoProcessing.Auth.Application/Validators/Auth/CreateUserInputValidator.cs` (remover regras de Email; validar Username como email)
  - `src/VideoProcessing.Auth.Application/Ports/ICognitoAuthService.cs` (SignUpAsync sem parâmetro email)
  - `src/VideoProcessing.Auth.Infra/Services/CognitoAuthService.cs` (SignUpAsync apenas username e password; Username e atributo email = username)
  - `src/VideoProcessing.Auth.Application/UseCases/Auth/CreateUserUseCase.cs` (não passar email; log ajustado)
  - `src/VideoProcessing.Auth.Api/Controllers/Auth/UserController.cs` (documentação XML: username = email)
  - `tests/VideoProcessing.Auth.Tests.Unit/Services/CognitoAuthServiceSignUpTests.cs` (ajustar chamadas e asserts)
  - `tests/VideoProcessing.Auth.Tests.Unit/UseCases/Auth/CreateUserUseCaseTests.cs` (input sem Email; mocks)
  - `tests/VideoProcessing.Auth.Tests.Unit/Validators/Auth/CreateUserInputValidatorTests.cs` (remover testes de Email; validar Username como email)
- Componentes: CreateUserInput, CreateUserInputValidator, ICognitoAuthService, CognitoAuthService, CreateUserUseCase, UserController

## Dependências e Riscos (para estimativa)
- Dependências: Nenhuma.
- Riscos/Pré-condições: O User Pool do Cognito deve estar configurado com sign-in por email (UsernameAttributes = email). Após a alteração, o cliente da API deve enviar o email no campo `username`.

## Subtasks
- [ ] [Subtask 01: Remover Email do Input e validar Username como email](./subtask/Subtask-01-Input_Validator_Username_Email.md)
- [ ] [Subtask 02: Ajustar Port, Service e UseCase de criação de usuário](./subtask/Subtask-02-Port_Service_UseCase.md)
- [ ] [Subtask 03: Atualizar documentação do Controller e contratos](./subtask/Subtask-03-Controller_Documentacao.md)
- [ ] [Subtask 04: Ajustar testes unitários](./subtask/Subtask-04-Testes_Unitarios.md)

## Critérios de Aceite da História
- [ ] CreateUserInput não possui mais propriedade Email; apenas Username e Password
- [ ] Validação exige que Username seja um email válido (formato e tamanho); regras de Email removidas
- [ ] SignUpAsync na porta e no serviço recebe apenas username e password; no Cognito, Username = valor recebido e atributo email = mesmo valor
- [ ] Documentação da API (XML do UserController) descreve que username deve ser o email
- [ ] Todos os testes unitários ajustados e passando (dotnet test)

## Rastreamento (dev tracking)
- **Início:** 07/02/2026, às 22:08 (Brasília)
- **Fim:** —
- **Tempo total de desenvolvimento:** —
