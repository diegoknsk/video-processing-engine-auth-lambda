# Subtask 03: Documentar configuração do Handler pela action

## Descrição
Atualizar a documentação do projeto para deixar explícito que o Handler da Lambda é configurado pelo workflow de deploy (valor `VideoProcessing.Auth.Api`) e que não é necessário setar o handler manualmente na AWS quando a Lambda for criada como “casca”.

## Passos de implementação
1. Em `docs/processo-subida-deploy.md`: na seção de checklist ou “o que setar”, mencionar que o **Handler** é definido pela action e qual é o valor (`VideoProcessing.Auth.Api`). Incluir uma linha na tabela de referência rápida ou no resumo do que a action faz.
2. Em `docs/lambda-handler-addawslambdahosting.md`: na seção 6 (Resumo para IaC / Checklist), adicionar uma nota de que o workflow de deploy já configura o Handler, portanto em deploys via GitHub Actions não é necessário configurar o handler manualmente no IaC/Console (a menos que se queira override).
3. (Opcional) Em `docs/deploy-github-actions.md`, se existir seção de steps do workflow, incluir o step “Update Lambda handler” na descrição dos passos executados.

## Formas de teste
- Ler a documentação e confirmar que um novo desenvolvedor entenderia que o handler é setado pela action.
- Verificar que não há contradição entre os docs (processo-subida, lambda-handler, deploy-github-actions).

## Critérios de aceite da subtask
- [ ] `docs/processo-subida-deploy.md` menciona que o Handler é configurado pelo workflow e indica o valor `VideoProcessing.Auth.Api`.
- [ ] `docs/lambda-handler-addawslambdahosting.md` (seção 6 ou equivalente) contém nota sobre o workflow configurar o Handler.
- [ ] A documentação está consistente e suficiente para quem for fazer deploy ou provisionar a Lambda.
