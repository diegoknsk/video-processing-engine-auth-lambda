# Subtask 01: Atualizar CognitoOptions e configuração (appsettings, README)

## Descrição
Ajustar o modelo de opções do Cognito para expor apenas Region, UserPoolId e ClientId, e atualizar appsettings e README para refletir a config básica (sem secret).

## Passos de implementação
1. Em `CognitoOptions.cs`: remover a propriedade `AppClientSecret`; renomear `AppClientId` para `ClientId` (para alinhar ao formato de config desejado).
2. Em `appsettings.json`: na seção Cognito, remover `AppClientSecret` e `AppClientId`; adicionar/manter `Region`, `UserPoolId` e `ClientId` (valores vazios ou placeholder).
3. Em `README.md`: na seção de configuração, atualizar o exemplo JSON para mostrar apenas `Cognito.Region`, `Cognito.UserPoolId` e `Cognito.ClientId`, removendo qualquer menção a App Client Secret.

## Formas de teste
1. Executar a API e verificar que a configuração é carregada (sem erros de binding).
2. Conferir que variáveis de ambiente no formato `Cognito__ClientId`, `Cognito__Region`, `Cognito__UserPoolId` continuam funcionando.
3. Revisão manual do README para clareza.

## Critérios de aceite da subtask
- [x] CognitoOptions possui apenas Region, UserPoolId e ClientId (sem AppClientSecret)
- [x] appsettings.json contém apenas Region, UserPoolId, ClientId na seção Cognito
- [x] README documenta apenas os três parâmetros na configuração
