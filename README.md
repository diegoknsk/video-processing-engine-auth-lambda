# Video Processing Auth API

API de autenticação para Video Processing Engine usando Amazon Cognito.

[![Deploy to AWS Lambda](https://github.com/diegoknsk/video-processing-engine-auth-lambda/actions/workflows/deploy-lambda.yml/badge.svg)](https://github.com/diegoknsk/video-processing-engine-auth-lambda/actions/workflows/deploy-lambda.yml)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=diegoknsk_video-processing-engine-auth-lambda&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=diegoknsk_video-processing-engine-auth-lambda)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=diegoknsk_video-processing-engine-auth-lambda&metric=coverage)](https://sonarcloud.io/summary/new_code?id=diegoknsk_video-processing-engine-auth-lambda)

## 📚 Documentação

- **Documentação Interativa (Scalar UI)**: Acesse `/docs` quando a aplicação estiver em execução
- **Especificação OpenAPI**: Disponível em `/swagger/v1/swagger.json`
- **Configuração API Gateway**: Veja [docs/api-gateway-configuration.md](./docs/api-gateway-configuration.md)
- **Contexto Arquitetural**: Veja [docs/contexto-arquitetural.md](./docs/contexto-arquitetural.md)
- **Deploy e CI/CD**: Veja [docs/deploy-github-actions.md](./docs/deploy-github-actions.md)
- **SonarCloud (Quality Gate, cobertura)**: Veja [docs/documentacaoSonar.md](./docs/documentacaoSonar.md)
- **Prefixo de path no API Gateway**: Veja [docs/gateway-path-prefix.md](./docs/gateway-path-prefix.md) (variável `GATEWAY_PATH_PREFIX` para uso atrás de gateway com prefixo, ex.: `/auth`)

## 🚀 Endpoints

### Autenticação

- **POST** `/login` - Autentica usuário e retorna tokens JWT
- **POST** `/users/create` - Cria novo usuário no sistema

### Health Check

- **GET** `/health` - Verifica saúde da aplicação

## 🛠️ Tecnologias

- .NET 10
- ASP.NET Core
- Amazon Cognito (AWS SDK)
- FluentValidation
- Swashbuckle (OpenAPI)
- Scalar UI (Documentação)

## 📦 Executando Localmente

```bash
# Restaurar dependências
dotnet restore

# Build
dotnet build

# Executar
dotnet run --project src/VideoProcessing.Auth.Api
```

A aplicação estará disponível em `http://localhost:5000` (ou porta configurada).

Acesse a documentação interativa em: `http://localhost:5000/docs`

## 🧪 Testes

```bash
# Executar todos os testes
dotnet test

# Executar testes com cobertura
dotnet test /p:CollectCoverage=true
```

## 📝 Estrutura do Projeto

```
src/
  ├── VideoProcessing.Auth.Api/          # API (Controllers, Filters, Middleware)
  ├── VideoProcessing.Auth.Application/   # Casos de uso, Input/Output models, Validators
  └── VideoProcessing.Auth.Infra/        # Implementações (Cognito Service)

tests/
  └── VideoProcessing.Auth.Tests.Unit/   # Testes unitários

docs/                                     # Documentação
storys/                                   # Stories técnicas
.github/workflows/                        # GitHub Actions (CI/CD)
```

## 🔐 Configuração

Configure as variáveis de ambiente ou `appsettings.json`:

```json
{
  "Cognito": {
    "Region": "us-east-1",
    "UserPoolId": "us-east-1_XXXXXXXXX",
    "ClientId": "xxxxxxxxxxxxxxxxxx"
  }
}
```

### Permissões IAM Necessárias

A aplicação requer credenciais IAM com as seguintes permissões no Amazon Cognito User Pool:

- `cognito-idp:AdminCreateUser` - Necessária para criar usuários via endpoint `POST /users/create`
- `cognito-idp:AdminSetUserPassword` - Necessária para definir senha permanente após criação do usuário

**Exemplo de política IAM mínima:**

```json
{
  "Version": "2012-10-17",
  "Statement": [
    {
      "Effect": "Allow",
      "Action": [
        "cognito-idp:AdminCreateUser",
        "cognito-idp:AdminSetUserPassword"
      ],
      "Resource": "arn:aws:cognito-idp:REGION:ACCOUNT_ID:userpool/USER_POOL_ID"
    }
  ]
}
```

**Nota:** Essas permissões são necessárias para o fluxo de criação de usuário sem confirmação de email (AdminCreateUser + AdminSetUserPassword), que permite login imediato após o cadastro.

## 📖 Mais Informações

Consulte a [documentação completa](./docs/) para mais detalhes sobre:
- Configuração para API Gateway
- Arquitetura e decisões técnicas
- Deploy automatizado via GitHub Actions

## 🚀 Deploy e CI/CD

A aplicação possui deploy automatizado via GitHub Actions para AWS Lambda.

### Deploy Automático

O workflow é executado automaticamente em:
- **Push para `main`**: Deploy direto em produção
- **Pull Request para `main`**: Build e testes de validação

### Deploy Manual

Você pode executar o deploy manualmente em qualquer branch via GitHub Actions:

1. Acesse: `Actions > Deploy Lambda Auth API > Run workflow`
2. Selecione a branch desejada
3. Clique em `Run workflow`

### Configuração Necessária

Há três tipos de configuração envolvidos no deploy e na execução da API:

| Onde | O que é | Uso |
|------|---------|-----|
| **GitHub Secrets** | Dados sensíveis (credenciais) — **nunca** aparecem em logs | Autenticação AWS no pipeline (deploy) |
| **GitHub Variables** | Parâmetros do pipeline (não sensíveis) — podem ter valor padrão no workflow | Nome do Lambda, região, opcionalmente Cognito e prefixo de path |
| **Variáveis de ambiente do Lambda** | Configuração da aplicação em runtime na AWS | Cognito, ambiente ASP.NET; a aplicação lê essas variáveis ao rodar no Lambda |

- **Secrets** são obrigatórios para o deploy: o workflow usa eles para autenticar na AWS. Variáveis são opcionais quando o workflow já define valor padrão (ex.: região `us-east-1`).
- **Variáveis de ambiente do Lambda** são configuradas na função Lambda (Console ou IaC) e **não** são as mesmas que GitHub Variables: as do Lambda são lidas pela aplicação .NET em execução; as do GitHub são usadas pelo workflow durante o deploy. Opcionalmente, o workflow pode atualizar as variáveis do Lambda a partir de GitHub Variables (ex.: `COGNITO_USER_POOL_ID`, `GATEWAY_PATH_PREFIX`). Ver [docs/deploy-github-actions.md](./docs/deploy-github-actions.md).

#### GitHub Secrets (obrigatórios)

Configure em **Settings → Secrets and variables → Actions → Secrets**:

| Secret | Descrição |
|--------|-----------|
| `AWS_ACCESS_KEY_ID` | Access Key ID do IAM User para deploy |
| `AWS_SECRET_ACCESS_KEY` | Secret Access Key do IAM User para deploy |

#### GitHub Variables (opcionais)

Configure em **Settings → Secrets and variables → Actions → Variables**:

| Variable | Descrição | Valor Padrão |
|----------|-----------|--------------|
| `AWS_REGION` | Região AWS do Lambda | `us-east-1` |
| `LAMBDA_FUNCTION_NAME` | Nome da função Lambda | `video-processing-engine-dev-auth` |
| `COGNITO_USER_POOL_ID` | (Opcional) Se definida, o workflow atualiza a env var `Cognito__UserPoolId` do Lambda em todo deploy | — |
| `COGNITO_CLIENT_ID` | (Opcional) Idem para `Cognito__ClientId` | — |
| `GATEWAY_PATH_PREFIX` | (Opcional) Prefixo de path no API Gateway (ex.: `/auth`); ver [gateway-path-prefix.md](./docs/gateway-path-prefix.md) | — |

#### Variáveis de Ambiente do Lambda

Configuradas na **função Lambda** (AWS Console ou IaC). A aplicação .NET lê essas variáveis em runtime. Notação `__` (dois underscores) equivale à hierarquia no `appsettings.json` (ex.: `Cognito__Region` → `Cognito:Region`).

| Variável | Descrição | Exemplo |
|----------|-----------|---------|
| `Cognito__Region` | Região do Cognito User Pool | `us-east-1` |
| `Cognito__UserPoolId` | ID do User Pool | `us-east-1_XXXXXXXXX` |
| `Cognito__ClientId` | Client ID da aplicação | `xxxxxxxxxxxxxxxxxx` |
| `ASPNETCORE_ENVIRONMENT` | Ambiente de execução | `Production` |

**📚 Documentação completa:** Veja [docs/deploy-github-actions.md](./docs/deploy-github-actions.md) para:
- Configuração detalhada de IAM permissions
- Troubleshooting de problemas comuns
- Estrutura do workflow
- Processo de deploy passo a passo
