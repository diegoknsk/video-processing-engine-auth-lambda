# Subtask 03: Ajustar testes unitários

## Descrição
Atualizar os testes que usam CognitoOptions (AppClientId/AppClientSecret) para usar apenas ClientId e remover testes específicos de SECRET_HASH; remover o projeto de testes do SecretHashCalculator.

## Passos de implementação
1. Em `CognitoAuthServiceTests.cs`: em _options, remover `AppClientSecret` e trocar `AppClientId` por `ClientId`; remover os testes `LoginAsync_WhenAppClientSecretIsPresent_ShouldIncludeSecretHashInRequest` e `LoginAsync_WhenAppClientSecretIsEmpty_ShouldNotIncludeSecretHashInRequest`.
2. Em `CognitoAuthServiceSignUpTests.cs`: em _options (e em qualquer outro CognitoOptions construído no arquivo), remover `AppClientSecret` e usar `ClientId` no lugar de `AppClientId`; remover os testes `SignUpAsync_WhenAppClientSecretIsPresent_ShouldIncludeSecretHashInRequest` e `SignUpAsync_WhenAppClientSecretIsEmpty_ShouldNotIncludeSecretHashInRequest`.
3. Remover o arquivo `tests/VideoProcessing.Auth.Tests.Unit/Services/SecretHashCalculatorTests.cs`.

## Formas de teste
1. Executar `dotnet test` e garantir que todos os testes passam.
2. Verificar que não restam referências a SecretHashCalculator ou AppClientSecret nos testes.

## Critérios de aceite da subtask
- [x] CognitoAuthServiceTests usa CognitoOptions apenas com Region, UserPoolId, ClientId; testes de SecretHash removidos
- [x] CognitoAuthServiceSignUpTests idem; testes de SecretHash removidos
- [x] SecretHashCalculatorTests.cs removido
- [x] dotnet test passa sem erros
