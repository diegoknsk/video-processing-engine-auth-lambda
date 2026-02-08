# Storie-07: Remover Secret do Cognito — Autenticação com Config Básica

## Status
- **Estado:** ✅ Concluída
- **Data de Conclusão:** 07/02/2026

## Descrição
Como desenvolvedor do sistema, quero autenticar com o Cognito usando apenas Region, UserPoolId e ClientId (sem App Client Secret), para simplificar a configuração e permitir uso de App Client público (sem secret).

## Objetivo
Remover em todos os lugares o uso de App Client Secret do Cognito: modelo de opções, configuração, serviço de autenticação (Login e SignUp), calculadora de SECRET_HASH e testes. A API passará a autenticar somente com os três parâmetros básicos (Region, UserPoolId, ClientId).

## Escopo Técnico
- Tecnologias: .NET 10, ASP.NET Core, AWS SDK (AWSSDK.CognitoIdentityProvider)
- Arquivos afetados:
  - `src/VideoProcessing.Auth.Infra/Models/CognitoOptions.cs` (remover AppClientSecret; renomear AppClientId → ClientId)
  - `src/VideoProcessing.Auth.Infra/Models/SecretHashCalculator.cs` (remover arquivo)
  - `src/VideoProcessing.Auth.Infra/Services/CognitoAuthService.cs` (remover blocos SECRET_HASH em Login e SignUp)
  - `src/VideoProcessing.Auth.Api/appsettings.json` (remover AppClientSecret; usar ClientId)
  - `README.md` (atualizar exemplo de config)
  - `tests/VideoProcessing.Auth.Tests.Unit/Services/CognitoAuthServiceTests.cs` (remover AppClientSecret e testes de SecretHash)
  - `tests/VideoProcessing.Auth.Tests.Unit/Services/CognitoAuthServiceSignUpTests.cs` (idem)
  - `tests/VideoProcessing.Auth.Tests.Unit/Services/SecretHashCalculatorTests.cs` (remover arquivo)
- Componentes: CognitoOptions, CognitoAuthService (sem SECRET_HASH)

## Dependências e Riscos (para estimativa)
- Dependências: Nenhuma.
- Riscos/Pré-condições: O User Pool do Cognito deve usar um App Client **sem** Client Secret (tipo “public”) para que InitiateAuth e SignUp funcionem sem SECRET_HASH.

## Subtasks
- [x] [Subtask 01: Atualizar CognitoOptions e configuração (appsettings, README)](./subtask/Subtask-01-CognitoOptions_Config.md)
- [x] [Subtask 02: Remover SECRET_HASH do CognitoAuthService e remover SecretHashCalculator](./subtask/Subtask-02-Remover_SecretHash_Service.md)
- [x] [Subtask 03: Ajustar testes unitários](./subtask/Subtask-03-Ajustar_Testes_Unitarios.md)

## Critérios de Aceite da História
- [x] Configuração Cognito aceita apenas: Region, UserPoolId, ClientId (sem AppClientSecret)
- [x] CognitoOptions não possui mais AppClientSecret; propriedade de client id nomeada ClientId
- [x] Login (InitiateAuth) e SignUp não enviam SECRET_HASH; autenticação funciona apenas com os três parâmetros
- [x] Classe SecretHashCalculator e SecretHashCalculatorTests removidas
- [x] appsettings.json e README documentam apenas Region, UserPoolId, ClientId
- [x] Todos os testes unitários passando (dotnet test)

## Rastreamento (dev tracking)
- **Início:** 07/02/2026, às 21:47 (Brasília)
- **Fim:** 07/02/2026, às 21:51 (Brasília)
- **Tempo total de desenvolvimento:** 4min
