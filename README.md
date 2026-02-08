# Video Processing Auth API

API de autenticação para Video Processing Engine usando Amazon Cognito.

## 📚 Documentação

- **Documentação Interativa (Scalar UI)**: Acesse `/docs` quando a aplicação estiver em execução
- **Especificação OpenAPI**: Disponível em `/swagger/v1/swagger.json`
- **Geração de Client Kiota**: Veja [docs/kiota-client-generation.md](./docs/kiota-client-generation.md)
- **Configuração API Gateway**: Veja [docs/api-gateway-configuration.md](./docs/api-gateway-configuration.md)
- **Contexto Arquitetural**: Veja [docs/contexto-arquitetural.md](./docs/contexto-arquitetural.md)

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
