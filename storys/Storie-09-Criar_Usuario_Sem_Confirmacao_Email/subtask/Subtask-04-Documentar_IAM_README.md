# Subtask 04: Documentar permissões IAM no README

## Descrição
Incluir no README do projeto (ou em documentação referenciada) as permissões IAM necessárias para o fluxo de criação de usuário via AdminCreateUser e AdminSetUserPassword, para que quem configure a infra (Lambda, EC2, etc.) saiba quais políticas anexar.

## Passos de implementação
1. Abrir o README.md do repositório e localizar a seção de configuração do Cognito / variáveis de ambiente (ou criar uma seção "Requisitos IAM" / "Permissões AWS").
2. Documentar que a aplicação precisa de credenciais IAM com permissão para:
   - `cognito-idp:AdminCreateUser` no recurso do User Pool (ARN do user pool).
   - `cognito-idp:AdminSetUserPassword` no recurso do User Pool.
3. Opcional: incluir exemplo de política IAM (JSON) mínima para essas duas ações no User Pool utilizado.
4. Mencionar que essas permissões são necessárias para o endpoint de criação de usuário (POST /auth/users/create) que cria usuários já confirmados sem envio de email.

## Formas de teste
- Revisão: ler o README e confirmar que um desenvolvedor conseguiria configurar a política IAM com base na documentação.
- Verificar que a seção está coerente com o uso de UserPoolId em CognitoOptions.

## Critérios de aceite da subtask
- [ ] README (ou doc referenciada) descreve que são necessárias as permissões `cognito-idp:AdminCreateUser` e `cognito-idp:AdminSetUserPassword` no User Pool.
- [ ] Fica claro que isso se aplica ao fluxo de criação de usuário sem confirmação de email.
- [ ] Opcional: exemplo de política IAM incluído ou link para documentação AWS.
