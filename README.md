# Video Processing Auth API

API de autenticação para Video Processing Engine usando Amazon Cognito.

[![Deploy to AWS Lambda](https://github.com/diegoknsk/video-processing-engine-auth-lambda/actions/workflows/deploy-lambda.yml/badge.svg)](https://github.com/diegoknsk/video-processing-engine-auth-lambda/actions/workflows/deploy-lambda.yml)

## 📚 Documentação

- **Documentação Interativa (Scalar UI)**: Acesse `/docs` quando a aplicação estiver em execução
- **Especificação OpenAPI**: Disponível em `/swagger/v1/swagger.json`
- **Geração de Client Kiota**: Veja [docs/kiota-client-generation.md](./docs/kiota-client-generation.md)
- **Configuração API Gateway**: Veja [docs/api-gateway-configuration.md](./docs/api-gateway-configuration.md)
- **Contexto Arquitetural**: Veja [docs/contexto-arquitetural.md](./docs/contexto-arquitetural.md)
- **Deploy e CI/CD**: Veja [docs/deploy-github-actions.md](./docs/deploy-github-actions.md)

## 🚀 Endpoints

### Autenticação

- **POST** `/auth/login` - Autentica usuário e retorna tokens JWT
- **POST** `/auth/users/create` - Cria novo usuário no sistema

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

- `cognito-idp:AdminCreateUser` - Necessária para criar usuários via endpoint `POST /auth/users/create`
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
- Geração de clientes tipados com Kiota
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

#### GitHub Secrets (obrigatórios)

Configure em `Settings > Secrets and variables > Actions > Secrets`:

| Secret | Descrição |
|--------|-----------|
| `AWS_ACCESS_KEY_ID` | Access Key ID do IAM User para deploy |
| `AWS_SECRET_ACCESS_KEY` | Secret Access Key do IAM User para deploy |

#### GitHub Variables (opcionais)

Configure em `Settings > Secrets and variables > Actions > Variables`:

| Variable | Descrição | Valor Padrão |
|----------|-----------|--------------|
| `AWS_REGION` | Região AWS do Lambda | `us-east-1` |
| `LAMBDA_FUNCTION_NAME` | Nome da função Lambda | `video-processing-engine-dev-auth` |

#### Variáveis de Ambiente do Lambda

Configure as seguintes variáveis de ambiente na função Lambda (via AWS Console ou IaC):

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
