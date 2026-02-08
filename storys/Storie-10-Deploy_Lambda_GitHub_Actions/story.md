# Storie-10: Deploy Automatizado Lambda via GitHub Actions

## Status
- **Estado:** 📋 Pendente
- **Data de Conclusão:** —

## Descrição
Como desenvolvedor do projeto, quero que a aplicação seja automaticamente deployada no AWS Lambda quando houver merge/PR para a branch main, para que o código em produção esteja sempre atualizado e o deploy seja padronizado e rastreável.

## Objetivo
Criar um workflow do GitHub Actions que realize o build da aplicação .NET 10, gere o pacote ZIP e faça o deploy automático no AWS Lambda já provisionado pela infraestrutura. O workflow deve ser executado automaticamente em PRs para main e permitir execução manual em qualquer branch. Além disso, documentar todas as variáveis de ambiente e secrets necessários para o correto funcionamento da aplicação e do pipeline.

## Abordagem
Implementar um workflow do GitHub Actions (`.github/workflows/deploy-lambda.yml`) que:
1. Execute o build do projeto .NET 10 em modo Release
2. Publique a aplicação para Linux-x64 (runtime compatível com Lambda)
3. Gere o pacote ZIP do deployment
4. Faça upload do ZIP para o Lambda via AWS CLI
5. Utilize secrets e variáveis do GitHub para credenciais AWS e configuração do Lambda
6. Permita trigger automático (push/PR para main) e manual (workflow_dispatch para qualquer branch)

Documentar no README todas as variáveis de ambiente (GitHub Variables), secrets (GitHub Secrets) e variáveis de ambiente do Lambda necessárias.

## Escopo Técnico
- Tecnologias: GitHub Actions, AWS CLI, .NET 10 SDK, AWS Lambda
- Arquivos novos:
  - `.github/workflows/deploy-lambda.yml` (workflow principal de deploy)
  - `docs/deploy-github-actions.md` (documentação detalhada do processo de deploy)
- Arquivos afetados:
  - `README.md` (adicionar seção sobre Deploy e CI/CD, documentar secrets/variáveis)
- Componentes: GitHub Actions, AWS Lambda, AWS CLI
- Dependências: 
  - .NET 10 SDK (já usado no projeto)
  - AWS CLI (disponível em GitHub Actions runners)
- Pré-requisitos AWS:
  - Lambda já provisionado: `video-processing-engine-dev-auth` (configurável)
  - IAM User/Role com permissões: `lambda:UpdateFunctionCode`, `lambda:UpdateFunctionConfiguration`
  - Credenciais AWS (Access Key ID e Secret Access Key)
- Pré-requisitos GitHub:
  - Secrets configurados no repositório
  - Variables configuradas no repositório

## Dependências e Riscos (para estimativa)
- Dependências: 
  - Stories anteriores (Storie-01 a Storie-09) concluídas
  - Lambda provisionado pela infraestrutura com placeholder ZIP
  - API Gateway já configurado apontando para o Lambda
- Riscos/Pré-condições:
  - Credenciais AWS devem ter permissões adequadas de deploy no Lambda
  - Runtime do Lambda deve ser compatível com .NET 10 (dotnet8 ou dotnet6 bootstrap)
  - Variáveis de ambiente do Lambda precisam ser configuradas corretamente após primeiro deploy
  - Tamanho do ZIP não pode exceder limites do Lambda (50MB comprimido, 250MB descomprimido)
- Considerações futuras:
  - Story futura: Action para bloquear PR com cobertura de testes < 70% (não implementada agora, mas workflow pode ser estruturado para facilitar essa adição)

## Subtasks
- [ ] [Subtask 01: Criar workflow GitHub Actions para build e package](./subtask/Subtask-01-Workflow_Build_Package.md)
- [ ] [Subtask 02: Implementar step de deploy no Lambda via AWS CLI](./subtask/Subtask-02-Deploy_Lambda_AWS_CLI.md)
- [ ] [Subtask 03: Configurar triggers automáticos e manuais do workflow](./subtask/Subtask-03-Triggers_Workflow.md)
- [ ] [Subtask 04: Documentar secrets, variáveis e processo de deploy](./subtask/Subtask-04-Documentacao_Secrets_Variables.md)
- [ ] [Subtask 05: Atualizar README com seção de Deploy e CI/CD](./subtask/Subtask-05-Atualizar_README_Deploy.md)

## Critérios de Aceite da História
- [ ] Workflow `.github/workflows/deploy-lambda.yml` criado e funcional
- [ ] Build da aplicação .NET 10 em modo Release executado com sucesso no workflow
- [ ] Geração do ZIP de deployment inclui todos os arquivos necessários (binários, appsettings, etc.)
- [ ] Deploy no Lambda via AWS CLI funciona e atualiza o código do Lambda
- [ ] Workflow é executado automaticamente em push/PR para branch `main`
- [ ] Workflow permite execução manual (`workflow_dispatch`) em qualquer branch
- [ ] Documentação completa de todos os GitHub Secrets necessários (AWS_ACCESS_KEY_ID, AWS_SECRET_ACCESS_KEY, etc.)
- [ ] Documentação completa de todas as GitHub Variables necessárias (AWS_REGION, LAMBDA_FUNCTION_NAME, etc.)
- [ ] Documentação de como configurar as variáveis de ambiente do Lambda (Cognito__Region, Cognito__UserPoolId, Cognito__ClientId)
- [ ] README atualizado com seção de Deploy/CI/CD explicando o processo
- [ ] Estrutura do workflow preparada para futura integração com verificação de cobertura de testes (comentários ou estrutura de jobs)

## Rastreamento (dev tracking)
- **Início:** —
- **Fim:** —
- **Tempo total de desenvolvimento:** —
