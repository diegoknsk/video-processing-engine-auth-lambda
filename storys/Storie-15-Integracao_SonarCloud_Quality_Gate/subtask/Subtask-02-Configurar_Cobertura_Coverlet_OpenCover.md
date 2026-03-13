# Subtask 02: Configurar coleta de cobertura de testes com coverlet no formato OpenCover

## Descrição
Garantir que o projeto de testes unitários gera o relatório de cobertura no formato OpenCover (exigido pelo SonarCloud para .NET), ajustando o comando `dotnet test` no workflow e validando que o XML é gerado no path esperado pelo `sonar-project.properties`.

## Passos de Implementação
1. Verificar que `coverlet.msbuild` e `coverlet.collector` já estão referenciados no `.csproj` de testes (já presentes na versão 6.0.2).
2. Ajustar o comando `dotnet test` no workflow para coletar cobertura no formato OpenCover:
   ```
   dotnet test \
     --configuration Release \
     --no-build \
     --verbosity normal \
     /p:CollectCoverage=true \
     /p:CoverageReporter=opencover \
     /p:CoverletOutputFormat=opencover \
     /p:CoverletOutput=./TestResults/coverage.opencover.xml
   ```
3. Validar que o path de saída do XML está alinhado com `sonar.cs.opencover.reportsPaths` definido no `sonar-project.properties` (ex.: `tests/**/TestResults/**/coverage.opencover.xml`).
4. Adicionar o passo de teste com cobertura **antes** do `sonarscanner end` no workflow, para que o relatório já esteja disponível na análise.
5. Confirmar que arquivos gerados em `TestResults/` estão no `.gitignore` (não devem ser comitados).

## Formas de Teste
1. Rodar `dotnet test /p:CollectCoverage=true /p:CoverageReporter=opencover` localmente e verificar que o arquivo `coverage.opencover.xml` é gerado no path correto.
2. Abrir o XML gerado e confirmar que contém os módulos do projeto (`VideoProcessing.Auth.*`) com dados de cobertura de linhas e branches.
3. Fazer upload manual do XML para o SonarCloud via scanner local e verificar que a cobertura aparece no dashboard do projeto.

## Critérios de Aceite
- [ ] O arquivo `coverage.opencover.xml` é gerado com sucesso ao rodar `dotnet test` com os parâmetros de cobertura
- [ ] O path do XML gerado corresponde ao pattern configurado em `sonar.cs.opencover.reportsPaths`
- [ ] O relatório contém cobertura dos projetos `src/` (Api, Application, Infra) e exclui o próprio projeto de testes
