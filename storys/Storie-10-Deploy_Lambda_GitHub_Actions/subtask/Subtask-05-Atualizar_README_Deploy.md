# Subtask 05: Atualizar README com Seção de Deploy e CI/CD

## Descrição
Atualizar o arquivo `README.md` do projeto para incluir seção dedicada sobre Deploy e CI/CD, explicando o processo de deploy automatizado via GitHub Actions, status do workflow, links para documentação detalhada e informações sobre configuração de secrets e variáveis.

## Objetivos
1. Adicionar badge do workflow no topo do README (status do deploy)
2. Criar seção "🚀 Deploy e CI/CD" no README
3. Explicar resumidamente o processo de deploy automático
4. Listar secrets e variáveis necessários (com link para doc detalhada)
5. Adicionar instruções para deploy manual
6. Manter README conciso, com link para documentação completa

## Detalhes Técnicos

### Localização das Alterações no README

#### 1. Badge no Topo (após título principal)

Adicionar logo após o subtítulo "API de autenticação para Video Processing Engine...":

```markdown
# Video Processing Auth API

API de autenticação para Video Processing Engine usando Amazon Cognito.

[![Deploy to AWS Lambda](https://github.com/OWNER/REPO/actions/workflows/deploy-lambda.yml/badge.svg)](https://github.com/OWNER/REPO/actions/workflows/deploy-lambda.yml)
```

**Nota**: Substituir `OWNER` e `REPO` pelos valores corretos do repositório GitHub.

#### 2. Nova Seção de Deploy (após seção "📝 Estrutura do Projeto")

```markdown
## 🚀 Deploy e CI/CD

Este projeto utiliza **GitHub Actions** para deploy automatizado no AWS Lambda.

### Deploy Automático

O deploy é executado automaticamente nos seguintes eventos:

- **Push para `main`**: Deploy direto no Lambda `video-processing-engine-dev-auth`
- **Pull Request para `main`**: Valida build (não faz deploy)

### Deploy Manual

Para fazer deploy manual em qualquer branch:

1. Acessar **Actions** → **Deploy to AWS Lambda** → **Run workflow**
2. Selecionar branch desejada
3. (Opcional) Customizar nome do Lambda ou região AWS
4. Clicar em **Run workflow**

### Configuração Necessária

#### GitHub Secrets (Obrigatórios)

Configure em **Settings** → **Secrets and variables** → **Actions**:

- `AWS_ACCESS_KEY_ID` - Access Key ID do IAM User com permissões de deploy
- `AWS_SECRET_ACCESS_KEY` - Secret Access Key correspondente

#### GitHub Variables (Opcionais)

- `AWS_REGION` - Região AWS (padrão: `us-east-1`)
- `LAMBDA_FUNCTION_NAME` - Nome do Lambda (padrão: `video-processing-engine-dev-auth`)

#### Variáveis de Ambiente do Lambda

Configure diretamente no **AWS Lambda Console** → **Configuration** → **Environment variables**:

- `Cognito__Region` - Região do Cognito User Pool (ex: `us-east-1`)
- `Cognito__UserPoolId` - ID do User Pool (ex: `us-east-1_AbCdEfGhI`)
- `Cognito__ClientId` - App Client ID do Cognito

### Documentação Completa

Para informações detalhadas sobre:
- Configuração de secrets e variáveis
- Permissões IAM necessárias
- Troubleshooting
- Verificação pós-deploy

Consulte: **[Deploy com GitHub Actions](./docs/deploy-github-actions.md)**
```

#### 3. Atualizar Seção "📚 Documentação" (adicionar link para deploy)

Adicionar linha na lista de documentações:

```markdown
## 📚 Documentação

- **Documentação Interativa (Scalar UI)**: Acesse `/docs` quando a aplicação estiver em execução
- **Especificação OpenAPI**: Disponível em `/swagger/v1/swagger.json`
- **Geração de Client Kiota**: Veja [docs/kiota-client-generation.md](./docs/kiota-client-generation.md)
- **Configuração API Gateway**: Veja [docs/api-gateway-configuration.md](./docs/api-gateway-configuration.md)
- **Contexto Arquitetural**: Veja [docs/contexto-arquitetural.md](./docs/contexto-arquitetural.md)
- **Deploy e CI/CD**: Veja [docs/deploy-github-actions.md](./docs/deploy-github-actions.md) ← ADICIONAR ESTA LINHA
```

### Estrutura Resumida da Alteração

```
README.md
├── Título + Badge do workflow
├── ...
├── 📚 Documentação (adicionar link deploy)
├── ...
├── 📝 Estrutura do Projeto
├── 🚀 Deploy e CI/CD (NOVA SEÇÃO)
│   ├── Deploy Automático
│   ├── Deploy Manual
│   ├── Configuração Necessária
│   │   ├── GitHub Secrets
│   │   ├── GitHub Variables
│   │   └── Variáveis de Ambiente do Lambda
│   └── Documentação Completa (link)
└── ...
```

## Passos de Implementação

1. Ler conteúdo atual do `README.md`
2. Adicionar badge do workflow após o título principal
3. Localizar seção "📝 Estrutura do Projeto"
4. Inserir nova seção "🚀 Deploy e CI/CD" após ela
5. Adicionar informações sobre deploy automático e manual
6. Listar secrets e variáveis necessários (resumido)
7. Adicionar link para documentação completa (`docs/deploy-github-actions.md`)
8. Atualizar seção "📚 Documentação" com link para deploy
9. Verificar formatação Markdown

## Critérios de Aceite

- [ ] Badge do workflow adicionado no topo do README (após título)
- [ ] Nova seção "🚀 Deploy e CI/CD" criada após "📝 Estrutura do Projeto"
- [ ] Explicação sobre deploy automático (push/PR para main) incluída
- [ ] Instruções para deploy manual incluídas
- [ ] GitHub Secrets necessários listados (AWS_ACCESS_KEY_ID, AWS_SECRET_ACCESS_KEY)
- [ ] GitHub Variables opcionais listados (AWS_REGION, LAMBDA_FUNCTION_NAME) com valores padrão
- [ ] Variáveis de ambiente do Lambda listadas (Cognito__*)
- [ ] Link para documentação completa (`docs/deploy-github-actions.md`) incluído
- [ ] Seção "📚 Documentação" atualizada com link para deploy
- [ ] Formatação Markdown correta e consistente com resto do README
- [ ] README permanece conciso, detalhes técnicos ficam na doc separada

## Notas

- O README deve permanecer conciso e de alto nível
- Detalhes técnicos, troubleshooting e exemplos de código ficam em `docs/deploy-github-actions.md`
- O badge mostra status do último workflow executado (✅ success, ❌ failed, 🟡 in progress)
- Link para documentação completa é essencial para não poluir o README
- Badge deve usar URL correta do repositório (substituir OWNER/REPO)
- Seção de deploy deve ser visualmente destacada com emoji 🚀
- Manter consistência de estilo e emojis com o resto do README
