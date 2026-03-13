# Storie-15: Integração SonarCloud com Quality Gate no Pipeline CI/CD

## Status
- **Estado:** 🔄 Em desenvolvimento
- **Data de Conclusão:** [DD/MM/AAAA]

## Descrição
Como desenvolvedor do projeto, quero que toda Pull Request para a branch `main` passe pela análise do SonarCloud com Quality Gate configurado, para garantir qualidade de código e cobertura de testes mínima antes do merge.

## Objetivo
Integrar o SonarCloud ao pipeline GitHub Actions existente, configurando análise estática, coleta de cobertura de testes (via coverlet) e Quality Gate que bloqueia o merge caso os critérios mínimos de qualidade não sejam atendidos.

## Escopo Técnico
- Tecnologias: GitHub Actions, SonarCloud, coverlet, .NET 10, C# 13
- Arquivos afetados:
  - `.github/workflows/deploy-lambda.yml` (adicionar job de análise Sonar)
  - `tests/VideoProcessing.Auth.Tests.Unit/VideoProcessing.Auth.Tests.Unit.csproj` (validar config coverlet)
  - `sonar-project.properties` (novo — configuração do projeto Sonar)
- Componentes/Recursos: SonarCloud project, GitHub Branch Protection Rule, GitHub Secrets/Variables
- Pacotes/Dependências:
  - `coverlet.collector` 6.0.2 (já presente)
  - `coverlet.msbuild` 6.0.2 (já presente)
  - `SonarScanner for .NET` (action `sonarsource/sonarqube-scan-action` ou via dotnet-sonarscanner CLI)

## Dependências e Riscos (para estimativa)
- Dependências: Storie-10 (pipeline GitHub Actions já configurado); projeto no SonarCloud precisa ser criado manualmente
- Riscos/Pré-condições:
  - Conta no SonarCloud (sonarcloud.io) e organização vinculada ao repositório GitHub precisam existir
  - Token `SONAR_TOKEN` deve ser configurado como GitHub Secret antes da execução
  - Quality Gate padrão do SonarCloud pode exigir ajuste de threshold de cobertura (meta: ≥ 70%)

## Subtasks
- [Subtask 01: Criar projeto no SonarCloud e configurar sonar-project.properties](./subtask/Subtask-01-Criar_Projeto_SonarCloud_Properties.md)
- [Subtask 02: Configurar coleta de cobertura de testes com coverlet no formato OpenCover](./subtask/Subtask-02-Configurar_Cobertura_Coverlet_OpenCover.md)
- [Subtask 03: Adicionar job de análise SonarCloud no workflow GitHub Actions](./subtask/Subtask-03-Job_SonarCloud_GitHub_Actions.md)
- [Subtask 04: Configurar Branch Protection Rule para bloquear merge sem Quality Gate aprovado](./subtask/Subtask-04-Branch_Protection_Quality_Gate.md)
- [Subtask 05: Validar pipeline end-to-end e documentar secrets/variáveis necessários](./subtask/Subtask-05-Validacao_Documentacao.md)

## Critérios de Aceite da História
- [ ] Toda PR aberta para `main` dispara automaticamente a análise do SonarCloud
- [ ] O Quality Gate bloqueia o merge quando cobertura de testes for inferior a 70%
- [ ] O Quality Gate bloqueia o merge quando houver bugs, vulnerabilidades ou code smells críticos introduzidos
- [ ] O relatório de cobertura gerado pelo coverlet (formato OpenCover) é corretamente enviado ao SonarCloud
- [ ] A Branch Protection Rule da `main` exige o status check `sonarcloud` como obrigatório antes do merge
- [ ] O arquivo `sonar-project.properties` está configurado com chaves corretas (projectKey, organization, exclusions de cobertura)
- [ ] O secret `SONAR_TOKEN` está documentado e configurado no repositório GitHub
- [ ] O pipeline continua funcionando para o job de deploy após a aprovação do Quality Gate

## Documentação relacionada

- **SonarCloud (setup, secrets, Quality Gate):** [docs/documentacaoSonar.md](../../docs/documentacaoSonar.md)
- **Deploy e secrets/variáveis do pipeline:** [docs/deploy-github-actions.md](../../docs/deploy-github-actions.md) e [README.md](../../README.md) (seção Configuração Necessária)
- **Skill SonarCloud .NET:** `.cursor/skills/sonarcloud-dotnet/SKILL.md`

## Rastreamento (dev tracking)
- **Início:** 13/03/2026, às 01:32 (Brasília)
- **Fim:** —
- **Tempo total de desenvolvimento:** —
