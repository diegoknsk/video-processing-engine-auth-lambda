# SonarCloud — Integração e execução local

Este documento descreve a integração do SonarCloud com o pipeline GitHub Actions (Storie-15), os secrets/variáveis necessários e a execução local da análise.

---

## 1. CI/CD — Pipeline GitHub Actions

O workflow `.github/workflows/deploy-lambda.yml` inclui o job **SonarCloud Analysis**, que roda em todo `push` e `pull_request` para `main` e em `workflow_dispatch`. O job de deploy (`build-and-deploy`) só executa após o Quality Gate do SonarCloud ser aprovado.

### Secrets obrigatórios (Settings > Secrets and variables > Actions)

| Secret        | Descrição |
|---------------|-----------|
| `SONAR_TOKEN` | Token de análise do SonarCloud. Gerar em: [sonarcloud.io/account/security](https://sonarcloud.io/account/security) (Generate Tokens). |

### Variáveis obrigatórias (Settings > Variables > Actions)

| Variável              | Descrição |
|-----------------------|-----------|
| `SONAR_PROJECT_KEY`   | Chave do projeto no SonarCloud (ex.: `video-processing-engine-auth-lambda`). |
| `SONAR_ORGANIZATION`   | Slug da organização no SonarCloud (ex.: `diegoknsk`). |

O arquivo `sonar-project.properties` na raiz do repositório deve usar os mesmos valores (ou as variáveis do GitHub sobrescrevem no CI).

### Quality Gate

- Cobertura em novo código: **≥ 70%**.
- Sem bugs ou vulnerabilidades **CRITICAL** ou **BLOCKER** em novo código.
- Rating de manutenção em novo código: **A**.

Configuração no SonarCloud: Project Settings > Quality Gate (usar o padrão ou criar regra customizada).

### Branch Protection (configuração manual no GitHub)

Para bloquear o merge sem Quality Gate aprovado:

1. **Settings > Branches > Branch protection rules** — editar/criar regra para `main`.
2. Habilitar **Require a pull request before merging** e **Require status checks to pass before merging**.
3. Adicionar o status check **SonarCloud Analysis** (nome do job no workflow) como obrigatório.
4. Habilitar **Do not allow bypassing the above settings** (opcional, para incluir admins).
5. No SonarCloud: **Project Settings > GitHub** — ativar o webhook para o repositório para o status do Quality Gate aparecer na PR.

### Relatório de cobertura no SonarCloud

- Dashboard do projeto → abas **Coverage** ou **Measures** → métrica **Coverage**.
- O relatório OpenCover é gerado pelo job **Run tests with coverage** e enviado ao SonarCloud via `sonar.cs.opencover.reportsPaths=tests/**/TestResults/**/coverage.opencover.xml`.

---

## 2. Execução local (Windows, PowerShell)

Execute **todos os comandos na raiz do repositório**, **na mesma sessão do PowerShell** (do passo 1 ao 6, sem fechar o terminal). O passo 2 (build) precisa rodar logo após o begin para o Sonar descobrir os projetos. A cobertura só aparece no SonarCloud se o relatório tiver caminhos relativos; o passo 5 faz essa conversão.

### 0. Definir o token (uma vez por sessão)

Gere o token em: https://sonarcloud.io/account/security

```powershell
$env:SONAR_TOKEN = "seu_token_aqui"
```

---

### 1. Iniciar análise

Exclusões: o Sonar ignora `TestResults`, `bin`, `obj` e a pasta `coverage` (evita avisos de arquivos “fora do base dir”). O relatório de cobertura usado é só `coverage/coverage.opencover.xml` (gerado nos passos 3 e 4).

```powershell
dotnet sonarscanner begin /k:"video-processing-engine-auth-lambda" /o:"diegoknsk" /d:sonar.token=$env:SONAR_TOKEN /d:sonar.host.url="https://sonarcloud.io" /d:sonar.projectBaseDir="." /d:sonar.cs.opencover.reportsPaths="coverage/coverage.opencover.xml" /d:sonar.exclusions="**/TestResults/**,**/bin/**,**/obj/**,**/coverage/**,**/.sonarqube/**"
```

---

### 2. Build (obrigatório: é assim que o Sonar descobre os projetos)

Use a solution para o scanner enxergar todos os projetos. Se der "No analyzable projects were found", rode de novo o passo 1 e em seguida este.

```powershell
dotnet build VideoProcessing.Auth.sln --no-incremental
```

---

### 3. Rodar testes com cobertura

```powershell
dotnet test --collect:"XPlat Code Coverage" --results-directory TestResults -- DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.Format=opencover
```

---

### 4. Copiar relatório para pasta fixa (coverage\)

O relatório fica em `TestResults\<guid>\` ou em `tests\VideoProcessing.Auth.Tests.Unit\TestResults\<guid>\`. Este bloco copia o mais recente para `coverage\coverage.opencover.xml`.

```powershell
$src = Get-ChildItem -Path "TestResults","tests\VideoProcessing.Auth.Tests.Unit\TestResults" -Filter "coverage.opencover.xml" -Recurse -ErrorAction SilentlyContinue | Sort-Object LastWriteTime -Descending | Select-Object -First 1
if (-not $src) {
  Write-Error "Arquivo de cobertura não encontrado. Procure em TestResults ou em tests\VideoProcessing.Auth.Tests.Unit\TestResults"
} else {
  New-Item -ItemType Directory -Force -Path coverage | Out-Null
  Copy-Item $src.FullName -Destination "coverage\coverage.opencover.xml" -Force
  Write-Host "Copiado: $($src.FullName) -> coverage\coverage.opencover.xml"
}
```

---

### 5. Converter caminhos absolutos → relativos no XML

Necessário para o SonarCloud conseguir exibir a cobertura (ele não faz match com caminhos como `C:\...`). O script procura `coverage\coverage.opencover.xml` na pasta atual ou sobe até achar a raiz do repo (pasta com `.sln` ou `.git`).

```powershell
$xmlPath = $null
$cur = Get-Location
while ($cur) {
  $p = Join-Path $cur "coverage\coverage.opencover.xml"
  if (Test-Path $p) { $xmlPath = $p; break }
  $parent = Split-Path $cur -Parent
  if ($parent -eq $cur) { break }
  $cur = $parent
}
if (-not $xmlPath) {
  Write-Warning "Arquivo coverage\coverage.opencover.xml não encontrado. Execute os passos 3 e 4 na raiz do repositório antes."
} else {
  $base = (Split-Path $xmlPath -Parent | Split-Path -Parent).TrimEnd('\', '/') + [System.IO.Path]::DirectorySeparatorChar
  $content = [System.IO.File]::ReadAllText($xmlPath)
  $content = $content.Replace($base, '').Replace('\', '/')
  [System.IO.File]::WriteAllText($xmlPath, $content)
  Write-Host "Caminhos convertidos em $xmlPath"
}
```

---

### 6. Finalizar e enviar para o SonarCloud

```powershell
dotnet sonarscanner end /d:sonar.token=$env:SONAR_TOKEN
```

---

### Depois de enviar

- Aguarde 1–2 minutos (o relatório é processado em background).
- No SonarCloud: projeto → aba **Coverage** ou **Measures** → métrica **Coverage**.

**Segurança:** não versionar o token. Use sempre a variável `$env:SONAR_TOKEN` no terminal.
