# Subtask 01: Criar Workflow GitHub Actions para Build e Package

## Descrição
Criar o arquivo de workflow do GitHub Actions (`.github/workflows/deploy-lambda.yml`) com os steps necessários para fazer checkout do código, configurar o .NET 10 SDK, restaurar dependências, fazer build em modo Release e publicar a aplicação para o runtime Linux-x64 (compatível com AWS Lambda).

## Objetivos
1. Criar estrutura de pastas `.github/workflows/` se não existir
2. Criar arquivo `deploy-lambda.yml` com nome descritivo
3. Configurar job de build com runner ubuntu-latest
4. Implementar steps de checkout, setup .NET 10, restore, build e publish
5. Preparar artefatos para o step de deploy (próxima subtask)

## Detalhes Técnicos

### Estrutura do Workflow (Build)

```yaml
name: Deploy to AWS Lambda

on:
  # Configurado na Subtask 03
  workflow_dispatch:
  push:
    branches:
      - main

env:
  DOTNET_VERSION: '10.x'
  AWS_REGION: ${{ vars.AWS_REGION || 'us-east-1' }}
  LAMBDA_FUNCTION_NAME: ${{ vars.LAMBDA_FUNCTION_NAME || 'video-processing-engine-dev-auth' }}

jobs:
  deploy:
    name: Build and Deploy Lambda
    runs-on: ubuntu-latest
    
    steps:
      - name: Checkout code
        uses: actions/checkout@v4
      
      - name: Setup .NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: ${{ env.DOTNET_VERSION }}
      
      - name: Restore dependencies
        run: dotnet restore
      
      - name: Build application
        run: dotnet build --configuration Release --no-restore
      
      - name: Publish application
        run: |
          dotnet publish src/VideoProcessing.Auth.Api/VideoProcessing.Auth.Api.csproj \
            --configuration Release \
            --runtime linux-x64 \
            --self-contained false \
            --output ./publish
      
      - name: Create deployment package
        run: |
          cd publish
          zip -r ../deployment-package.zip .
          cd ..
      
      # Deploy steps virão na Subtask 02
```

### Explicação dos Steps

1. **Checkout code**: Faz checkout do repositório
2. **Setup .NET**: Configura .NET 10 SDK no runner
3. **Restore dependencies**: Restaura pacotes NuGet
4. **Build application**: Compila em modo Release (otimizado)
5. **Publish application**: Publica para Linux-x64 (runtime do Lambda) na pasta `./publish`
6. **Create deployment package**: Cria ZIP com todos os arquivos necessários

### Variáveis de Ambiente do Workflow

- `DOTNET_VERSION`: Versão do .NET (10.x)
- `AWS_REGION`: Região AWS (padrão: us-east-1, configurável via GitHub Variable)
- `LAMBDA_FUNCTION_NAME`: Nome do Lambda (padrão: video-processing-engine-dev-auth, configurável via GitHub Variable)

## Passos de Implementação

1. Criar pasta `.github/workflows/` na raiz do projeto
2. Criar arquivo `deploy-lambda.yml`
3. Implementar estrutura básica do workflow (name, on, env, jobs)
4. Adicionar steps de checkout e setup .NET
5. Adicionar steps de restore e build
6. Adicionar step de publish com flags corretas para Lambda
7. Adicionar step de criação do ZIP
8. Testar localmente se o comando de publish funciona

## Critérios de Aceite

- [ ] Pasta `.github/workflows/` criada
- [ ] Arquivo `deploy-lambda.yml` criado com nome descritivo
- [ ] Step de checkout implementado (actions/checkout@v4)
- [ ] Step de setup .NET 10 implementado (actions/setup-dotnet@v4)
- [ ] Step de restore dependencies implementado
- [ ] Step de build em Release implementado
- [ ] Step de publish para linux-x64 implementado, output em `./publish`
- [ ] Step de criação do ZIP implementado, gera `deployment-package.zip`
- [ ] Variáveis de ambiente `AWS_REGION` e `LAMBDA_FUNCTION_NAME` configuradas com valores padrão

## Notas

- O runtime `linux-x64` é compatível com AWS Lambda
- O flag `--self-contained false` reduz o tamanho do pacote (usa runtime do Lambda)
- O ZIP deve incluir todos os arquivos da pasta `publish` (binários, appsettings.json, etc.)
- A estrutura do workflow já deve prever a futura adição de step de cobertura de testes (comentário ou estrutura de jobs)
