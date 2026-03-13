---
name: sonarcloud-dotnet
description: Integra projeto .NET ao SonarCloud via GitHub Actions — configura job de análise, coleta de cobertura com coverlet (OpenCover), Quality Gate e .gitignore. Use quando a tarefa envolver SonarCloud, SonarQube, sonarscanner, análise estática, cobertura de código, Quality Gate, coverlet OpenCover ou integração Sonar com pipeline CI/CD .NET.
---

# SonarCloud — Integração com .NET e GitHub Actions

## Armadilhas críticas (leia primeiro)

| # | Problema | Causa | Solução |
|---|----------|-------|---------|
| 1 | `sonar-project.properties files are not understood` | O SonarScanner for .NET ignora esse arquivo | Nunca criar `sonar-project.properties`; passar tudo via `/d:` |
| 2 | `No analyzable projects were found` — todos os arquivos "not located under the base directory"  | `sonar.projectBaseDir="."` resolve para `.sonarqube/` no runner | Usar `sonar.projectBaseDir="${{ github.workspace }}"` (caminho absoluto) |
| 3 | Cobertura não aparece no SonarCloud | Path relativo no `CoverletOutput` não bate com o glob do Sonar | Manter `CoverletOutput=./TestResults/coverage.opencover.xml` e `sonar.cs.opencover.reportsPaths="tests/**/TestResults/**/coverage.opencover.xml"` |

---

## 1. Pacotes NuGet no projeto de testes

```xml
<PackageReference Include="coverlet.collector" Version="6.0.2">
  <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
  <PrivateAssets>all</PrivateAssets>
</PackageReference>
<PackageReference Include="coverlet.msbuild" Version="6.0.2">
  <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
  <PrivateAssets>all</PrivateAssets>
</PackageReference>
```

---

## 2. Job `sonar-analysis` no workflow

Adicionar **antes** do job de deploy. O job de deploy deve declarar `needs: [sonar-analysis]`.

```yaml
sonar-analysis:
  name: SonarCloud Analysis
  runs-on: ubuntu-latest
  steps:
    - name: Checkout code
      uses: actions/checkout@v4
      with:
        fetch-depth: 0          # obrigatório para blame info do Sonar

    - name: Setup .NET
      uses: actions/setup-dotnet@v4
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
          /d:sonar.projectBaseDir="${{ github.workspace }}" \
          /d:sonar.sources="src/" \
          /d:sonar.tests="tests/" \
          /d:sonar.exclusions="**/bin/**,**/obj/**,**/*.Designer.cs" \
          /d:sonar.test.exclusions="tests/**/" \
          /d:sonar.coverage.exclusions="**/Program.cs,**/*Extensions.cs" \
          /d:sonar.cs.opencover.reportsPaths="tests/**/TestResults/**/coverage.opencover.xml"

    - name: Build solution
      run: dotnet build --configuration Release --no-restore

    - name: Run tests with coverage
      run: |
        dotnet test \
          --configuration Release \
          --no-build \
          --verbosity normal \
          /p:CollectCoverage=true \
          /p:CoverageReporter=opencover \
          /p:CoverletOutputFormat=opencover \
          /p:CoverletOutput=./TestResults/coverage.opencover.xml

    - name: End SonarCloud analysis
      run: |
        dotnet sonarscanner end \
          /d:sonar.token="${{ secrets.SONAR_TOKEN }}"
```

---

## 3. Dependência do job de deploy

```yaml
build-and-deploy:
  name: Build and Deploy
  needs: [sonar-analysis]
  ...
```

---

## 4. GitHub — Secrets e Variables

| Tipo | Nome | Onde criar | Valor |
|------|------|-----------|-------|
| Secret | `SONAR_TOKEN` | Settings > Secrets and variables > Actions | Token gerado em [sonarcloud.io/account/security](https://sonarcloud.io/account/security) |
| Variable | `SONAR_PROJECT_KEY` | Settings > Variables > Actions | Chave do projeto no SonarCloud (ex.: `meu-projeto`) |
| Variable | `SONAR_ORGANIZATION` | Settings > Variables > Actions | Slug da organização no SonarCloud (ex.: `minha-org`) |

---

## 5. .gitignore — entradas obrigatórias

```gitignore
# SonarCloud / SonarQube
.sonarqube/
**/.sonarqube/
**/out/.sonar/
.scannerwork/
**/.scannerwork/
coverage.opencover.xml
```

---

## 6. Quality Gate recomendado

Configurar no SonarCloud em **Project Settings > Quality Gate**:

- Cobertura em novo código: **≥ 70%**
- Sem bugs ou vulnerabilidades `CRITICAL` / `BLOCKER` em novo código
- Maintainability Rating em novo código: **A**

---

## 7. Branch Protection (manual no GitHub)

1. **Settings > Branches > Branch protection rules** → regra para `main`
2. Habilitar **Require status checks to pass before merging**
3. Adicionar `SonarCloud Analysis` (nome exato do `name:` do job) como check obrigatório
4. No SonarCloud: **Project Settings > GitHub** → ativar webhook para reportar Quality Gate na PR

---

## Checklist de implementação

- [ ] `coverlet.collector` e `coverlet.msbuild` no `.csproj` de testes
- [ ] Job `sonar-analysis` adicionado ao workflow com `fetch-depth: 0`
- [ ] `sonar.projectBaseDir="${{ github.workspace }}"` (nunca `"."`)
- [ ] Job de deploy com `needs: [sonar-analysis]`
- [ ] Secret `SONAR_TOKEN` e variables `SONAR_PROJECT_KEY` / `SONAR_ORGANIZATION` configurados no GitHub
- [ ] Entradas do Sonar no `.gitignore`
- [ ] Branch Protection Rule configurada com check obrigatório

---

## Referência detalhada

Consultar [reference.md](reference.md) para execução local do scanner (Windows/PowerShell).
