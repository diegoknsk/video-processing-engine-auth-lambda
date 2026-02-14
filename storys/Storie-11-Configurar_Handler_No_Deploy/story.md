# Storie-11: Configurar Handler da Lambda no Workflow de Deploy

## Status
- **Estado:** 📋 Pendente
- **Data de Conclusão:** —

## Descrição
Como desenvolvedor do projeto, quero que o workflow de deploy configure o Handler da função Lambda, para que uma Lambda criada apenas como “casca” tenha o handler correto (`VideoProcessing.Auth.Api`) sem configuração manual na AWS.

## Objetivo
Incluir no workflow do GitHub Actions um step que execute `aws lambda update-function-configuration --handler VideoProcessing.Auth.Api` após o deploy do código, garantindo que o Handler fique versionado no repositório e aplicado a cada deploy.

## Escopo Técnico
- Tecnologias: GitHub Actions, AWS CLI
- Arquivos afetados:
  - `.github/workflows/deploy-lambda.yml` (novo step)
  - `docs/processo-subida-deploy.md` e/ou `docs/deploy-github-actions.md` (documentar que o handler é setado pela action)
- Componentes: step "Update Lambda handler" no workflow
- Dependências: AWS CLI (já usado no workflow); permissão `lambda:UpdateFunctionConfiguration` (já coberta por UpdateFunctionConfiguration no escopo do deploy)

## Dependências e Riscos (para estimativa)
- Dependências: Storie-10 (Deploy Lambda GitHub Actions) concluída; Lambda já existe.
- Riscos/Pré-condições: Nenhum crítico; o step só altera o Handler, mantendo demais configurações.

## Subtasks
- [ ] [Subtask 01: Adicionar step de configuração do Handler no workflow](./subtask/Subtask-01-Step_Update_Handler_Workflow.md)
- [ ] [Subtask 02: Incluir Handler na verificação do deploy](./subtask/Subtask-02-Verify_Handler_Deploy.md)
- [ ] [Subtask 03: Documentar configuração do Handler pela action](./subtask/Subtask-03-Documentar_Handler_Action.md)

## Critérios de Aceite da História
- [ ] O workflow executa um step que chama `update-function-configuration --handler "VideoProcessing.Auth.Api"` após o deploy do código (e após o wait).
- [ ] O step só roda em contexto de deploy (não em PR), assim como os demais steps de deploy.
- [ ] A verificação do deploy (Verify deployment) inclui o Handler no output, permitindo conferir que está correto.
- [ ] A documentação do processo de deploy menciona que o Handler é configurado pela action e qual é o valor (`VideoProcessing.Auth.Api`).
- [ ] Lambda criada como “casca” passa a ter o handler correto após um deploy sem configuração manual.

## Rastreamento (dev tracking)
- **Início:** 13/02/2026, às 22:32 (Brasília)
- **Fim:** —
- **Tempo total de desenvolvimento:** —
