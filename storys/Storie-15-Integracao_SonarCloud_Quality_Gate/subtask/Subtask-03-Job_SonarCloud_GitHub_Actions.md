# Subtask 03: Adicionar job de análise SonarCloud no workflow GitHub Actions

## Descrição
Criar um job `sonar-analysis` separado no workflow `deploy-lambda.yml` (ou criar um novo workflow `sonar.yml`) que execute o SonarScanner for .NET, colete cobertura e publique os resultados no SonarCloud. O job de deploy deve depender do Quality Gate aprovado.

## Passos de Implementação
1. Adicionar novo job `sonar-analysis` no workflow, com `runs-on: ubuntu-latest`, disparado em `pull_request` para `main` e em `push` para `main`:
   ```yaml
   sonar-analysis:
     name: SonarCloud Analysis
     runs-on: ubuntu-latest
     steps:
       - uses: actions/checkout@v4
         with:
           fetch-depth: 0  # necessário para blame info do Sonar
       - uses: actions/setup-dotnet@v4
         with:
           dotnet-version: '10.0.x'
       - name: Install SonarScanner
         run: dotnet tool install --global dotnet-sonarscanner
       - name: Restore dependencies
         run: dotnet restore
       - name: Begin SonarCloud analysis
         run: |
           dotnet sonarscanner begin \
             /k:"${{ vars.SONAR_PROJECT_KEY }}" \
             /o:"${{ vars.SONAR_ORGANIZATION }}" \
             /d:sonar.token="${{ secrets.SONAR_TOKEN }}" \
             /d:sonar.host.url="https://sonarcloud.io" \
             /d:sonar.cs.opencover.reportsPaths="tests/**/TestResults/**/coverage.opencover.xml"
       - name: Build solution
         run: dotnet build --configuration Release --no-restore
       - name: Run tests with coverage
         run: |
           dotnet test \
             --configuration Release \
             --no-build \
             /p:CollectCoverage=true \
             /p:CoverageReporter=opencover \
             /p:CoverletOutputFormat=opencover \
             /p:CoverletOutput=./TestResults/coverage.opencover.xml
       - name: End SonarCloud analysis
         run: |
           dotnet sonarscanner end \
             /d:sonar.token="${{ secrets.SONAR_TOKEN }}"
   ```
2. Adicionar `needs: sonar-analysis` no job `build-and-deploy` para que o deploy só execute se o Sonar passar (apenas em push para `main`).
3. Configurar as variáveis `SONAR_PROJECT_KEY` e `SONAR_ORGANIZATION` como GitHub Variables (Settings > Variables > Actions).
4. Garantir `fetch-depth: 0` no checkout para que o SonarCloud tenha acesso ao histórico de blame (necessário para análise correta de new code).

## Formas de Teste
1. Abrir uma PR de teste para `main` e verificar que o check `SonarCloud Analysis` aparece na PR com status passando ou falhando.
2. Verificar no dashboard do SonarCloud que a análise foi registrada com métricas de cobertura, bugs e vulnerabilidades.
3. Introduzir propositalmente um code smell ou reduzir cobertura abaixo do threshold e verificar que o Quality Gate falha e bloqueia o merge.

## Critérios de Aceite
- [ ] O job `sonar-analysis` executa com sucesso em PRs para `main` e em pushes para `main`
- [ ] O resultado da análise aparece no SonarCloud com cobertura calculada corretamente
- [ ] O job de deploy (`build-and-deploy`) aguarda a conclusão do `sonar-analysis` antes de executar em pushes para `main`
