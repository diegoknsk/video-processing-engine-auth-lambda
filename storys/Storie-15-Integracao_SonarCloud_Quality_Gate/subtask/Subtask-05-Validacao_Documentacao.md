# Subtask 05: Validar pipeline end-to-end e documentar secrets/variáveis necessários

## Descrição
Executar um ciclo completo de validação do pipeline (PR → análise Sonar → Quality Gate → merge → deploy) e documentar todos os secrets e variáveis GitHub necessários para que o ambiente funcione corretamente em novos deploys ou outros repositórios do projeto.

## Passos de Implementação
1. Criar uma PR de validação com uma mudança simples (ex.: comentário em código) e acompanhar todo o fluxo: check de build → análise SonarCloud → Quality Gate → merge → deploy para Lambda.
2. Verificar no dashboard do SonarCloud que os dados de cobertura estão corretos e condizem com a saída local do `dotnet test`.
3. Documentar em `docs/sonarcloud.md` (ou atualizar `docs/documentacaoSonar.md` existente) incluindo:
   - Secrets obrigatórios: `SONAR_TOKEN`
   - Variáveis obrigatórias: `SONAR_PROJECT_KEY`, `SONAR_ORGANIZATION`
   - Como gerar o `SONAR_TOKEN` (passo a passo no SonarCloud)
   - Threshold do Quality Gate configurado
   - Como interpretar o relatório de cobertura no SonarCloud
   - Como executar análise local com `dotnet-sonarscanner`
4. Remover (ou descomentarizar e atualizar) o bloco comentado `test-coverage` do workflow `deploy-lambda.yml` para refletir a nova solução com SonarCloud.
5. Revisar o `.gitignore` para garantir que `TestResults/`, `coverage.opencover.xml` e arquivos temporários do scanner (`**/out/.sonar/`) estão ignorados.

## Formas de Teste
1. Executar a PR de validação completa e confirmar que todos os status checks passam e o deploy é executado com sucesso.
2. Verificar que o arquivo de documentação está correto e um desenvolvedor novo consegue seguir os passos para configurar o ambiente.
3. Confirmar que o SonarCloud exibe o badge de Quality Gate como "Passed" no dashboard do projeto.

## Critérios de Aceite
- [ ] Pipeline end-to-end executado com sucesso (PR → Sonar → Quality Gate → merge → deploy)
- [ ] Documentação criada/atualizada com todos os secrets, variáveis e instruções de configuração
- [ ] Badge do Quality Gate visível e "Passed" no SonarCloud para a branch `main`
