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
    "AppClientId": "xxxxxxxxxxxxxxxxxx",
    "AppClientSecret": "xxxxxxxxxxxxxxxxxx" // Opcional
  }
}
```

## 📖 Mais Informações

Consulte a [documentação completa](./docs/) para mais detalhes sobre:
- Geração de clientes tipados com Kiota
- Configuração para API Gateway
- Arquitetura e decisões técnicas
