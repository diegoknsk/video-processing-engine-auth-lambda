# Subtask 01: Criar projeto no SonarCloud e configurar sonar-project.properties

## Descrição
Criar o projeto no SonarCloud vinculado ao repositório GitHub e gerar o arquivo `sonar-project.properties` na raiz do repositório com as configurações corretas de `projectKey`, `organization`, exclusões de paths de teste e paths de cobertura.

## Passos de Implementação
1. Acessar [sonarcloud.io](https://sonarcloud.io), criar organização vinculada ao GitHub (caso não exista) e criar o projeto para o repositório `video-processing-engine-auth-lambda`.
2. Copiar o `projectKey` e o `organization slug` gerados pelo SonarCloud.
3. Criar o arquivo `sonar-project.properties` na raiz do repositório com:
   - `sonar.projectKey=<projectKey>`
   - `sonar.organization=<organization>`
   - `sonar.sources=src/`
   - `sonar.tests=tests/`
   - `sonar.cs.opencover.reportsPaths=tests/**/TestResults/**/coverage.opencover.xml`
   - `sonar.exclusions=**/bin/**,**/obj/**,**/*.Designer.cs`
   - `sonar.test.exclusions=tests/**`
   - `sonar.coverage.exclusions=**/Program.cs,**/*Extensions.cs`
4. Gerar o `SONAR_TOKEN` nas configurações de segurança do projeto no SonarCloud (My Account > Security > Generate Tokens).
5. Adicionar o token como secret `SONAR_TOKEN` no repositório GitHub (Settings > Secrets > Actions).

## Formas de Teste
1. Executar `dotnet sonarscanner begin` localmente com o token para validar que o `sonar-project.properties` é reconhecido sem erros.
2. Abrir uma PR de teste e verificar no SonarCloud que o projeto aparece com o nome e as configurações corretas.
3. Verificar na aba "Project Settings > Analysis Method" do SonarCloud que o projeto está configurado para CI-based analysis.

## Critérios de Aceite
- [ ] Arquivo `sonar-project.properties` presente na raiz do repositório com todas as chaves obrigatórias preenchidas
- [ ] Projeto visível no SonarCloud com o nome correto e vinculado ao repositório GitHub
- [ ] Secret `SONAR_TOKEN` configurado no repositório GitHub sem erros de autenticação
