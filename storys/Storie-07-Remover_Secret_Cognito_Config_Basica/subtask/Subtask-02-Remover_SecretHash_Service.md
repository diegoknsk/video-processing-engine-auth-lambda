# Subtask 02: Remover SECRET_HASH do CognitoAuthService e remover SecretHashCalculator

## Descrição
Remover toda a lógica condicional de SECRET_HASH no Login e no SignUp do CognitoAuthService e excluir a classe SecretHashCalculator, pois a autenticação passará a usar apenas ClientId (App Client público).

## Passos de implementação
1. Em `CognitoAuthService.cs`: no método `LoginAsync`, remover o bloco que verifica `AppClientSecret` e adiciona `SECRET_HASH` em `AuthParameters`; usar `_options.ClientId` no `InitiateAuthRequest.ClientId`.
2. Em `CognitoAuthService.cs`: no método `SignUpAsync`, remover o bloco que verifica `AppClientSecret` e define `request.SecretHash`; usar `_options.ClientId` no `SignUpRequest.ClientId`.
3. Remover o arquivo `src/VideoProcessing.Auth.Infra/Models/SecretHashCalculator.cs`.

## Formas de teste
1. Executar `dotnet build` e garantir que não há referências restantes a SecretHashCalculator ou AppClientSecret.
2. Testes unitários do CognitoAuthService (após ajuste na Subtask 03) devem passar.
3. Teste manual de login e criação de usuário contra um User Pool com App Client sem secret (se disponível).

## Critérios de aceite da subtask
- [x] LoginAsync não calcula nem envia SECRET_HASH
- [x] SignUpAsync não calcula nem envia SecretHash
- [x] CognitoAuthService usa _options.ClientId em InitiateAuth e SignUp
- [x] Arquivo SecretHashCalculator.cs removido; nenhum using ou referência restante no projeto
