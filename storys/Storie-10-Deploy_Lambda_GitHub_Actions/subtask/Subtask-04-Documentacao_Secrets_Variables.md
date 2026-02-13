# Subtask 04: Documentar Secrets, Variáveis e Processo de Deploy

## Descrição
Criar documentação completa e detalhada sobre todos os secrets do GitHub, variáveis do GitHub, variáveis de ambiente do Lambda e processo de configuração necessários para o funcionamento correto do pipeline de deploy e da aplicação no AWS Lambda.

## Objetivos
1. Criar documento detalhado `docs/deploy-github-actions.md`
2. Documentar todos os GitHub Secrets necessários e como configurá-los
3. Documentar todas as GitHub Variables necessárias e como configurá-las
4. Documentar variáveis de ambiente do Lambda (Cognito configuration)
5. Documentar permissões IAM necessárias para deploy
6. Documentar processo completo de configuração inicial
7. Adicionar troubleshooting e dicas

## Detalhes Técnicos

### Estrutura do Documento `docs/deploy-github-actions.md`

```markdown
# Deploy Automatizado com GitHub Actions

Este documento descreve o processo de deploy automatizado da aplicação Video Processing Auth API no AWS Lambda usando GitHub Actions.

## 📋 Pré-requisitos

### 1. AWS Lambda
- Lambda function já provisionado pela infraestrutura
- Runtime: .NET 8 ou .NET 6 (compatível com .NET 10 compiled)
- Nome padrão: `video-processing-engine-dev-auth`
- API Gateway já configurado

### 2. IAM User/Role
- IAM User ou Role com permissões de deploy
- Access Key ID e Secret Access Key gerados

### 3. GitHub Repository
- Acesso de administrador para configurar Secrets e Variables

---

## 🔐 GitHub Secrets (Configuração Obrigatória)

Configurar os seguintes secrets no GitHub:
**Settings** → **Secrets and variables** → **Actions** → **New repository secret**

### AWS_ACCESS_KEY_ID
- **Descrição**: Access Key ID do IAM User/Role para deploy
- **Tipo**: Secret
- **Obrigatório**: Sim
- **Exemplo**: `AKIAIOSFODNN7EXAMPLE`

### AWS_SECRET_ACCESS_KEY
- **Descrição**: Secret Access Key correspondente ao Access Key ID
- **Tipo**: Secret
- **Obrigatório**: Sim
- **Exemplo**: `wJalrXUtnFEMI/K7MDENG/bPxRfiCYEXAMPLEKEY`

---

## 📊 GitHub Variables (Configuração Opcional)

Configurar as seguintes variáveis no GitHub:
**Settings** → **Secrets and variables** → **Actions** → **Variables** → **New repository variable**

### AWS_REGION
- **Descrição**: Região AWS onde o Lambda está hospedado
- **Tipo**: Variable
- **Obrigatório**: Não (padrão: `us-east-1`)
- **Valor sugerido**: `us-east-1`

### LAMBDA_FUNCTION_NAME
- **Descrição**: Nome da função Lambda
- **Tipo**: Variable
- **Obrigatório**: Não (padrão: `video-processing-engine-dev-auth`)
- **Valor sugerido**: `video-processing-engine-dev-auth`

**Nota**: Se não configuradas, os valores padrão hardcoded no workflow serão usados.

---

## ⚙️ Variáveis de Ambiente do Lambda

As seguintes variáveis de ambiente devem ser configuradas **diretamente no AWS Lambda**:

### Via AWS Console
**Lambda** → **Sua função** → **Configuration** → **Environment variables** → **Edit**

### Via AWS CLI
```bash
aws lambda update-function-configuration \
  --function-name video-processing-engine-dev-auth \
  --environment "Variables={
    Cognito__Region=us-east-1,
    Cognito__UserPoolId=us-east-1_XXXXXXXXX,
    Cognito__ClientId=xxxxxxxxxxxxxxxxxx
  }" \
  --region us-east-1
```

### Variáveis Obrigatórias

#### Cognito__Region
- **Descrição**: Região AWS do Cognito User Pool
- **Tipo**: Variable (GitHub Variable recomendada para reutilização)
- **Exemplo**: `us-east-1`
- **Obrigatório**: Sim

#### Cognito__UserPoolId
- **Descrição**: ID do Cognito User Pool
- **Tipo**: Variable
- **Exemplo**: `us-east-1_AbCdEfGhI`
- **Obrigatório**: Sim

#### Cognito__ClientId
- **Descrição**: App Client ID do Cognito
- **Tipo**: Variable
- **Exemplo**: `1a2b3c4d5e6f7g8h9i0j1k2l3m`
- **Obrigatório**: Sim

**Nota**: As variáveis do Lambda usam a notação `__` (double underscore) para representar hierarquia JSON do `appsettings.json`.

---

## 🔑 Permissões IAM Necessárias

O IAM User/Role usado no GitHub Actions precisa das seguintes permissões:

### Política Mínima para Deploy

```json
{
  "Version": "2012-10-17",
  "Statement": [
    {
      "Effect": "Allow",
      "Action": [
        "lambda:UpdateFunctionCode",
        "lambda:GetFunction",
        "lambda:GetFunctionConfiguration"
      ],
      "Resource": "arn:aws:lambda:us-east-1:ACCOUNT_ID:function/video-processing-engine-dev-auth"
    }
  ]
}
```

### Permissões da Aplicação no Lambda

O **Lambda Execution Role** precisa de permissões para acessar o Cognito:

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
      "Resource": "arn:aws:cognito-idp:us-east-1:ACCOUNT_ID:userpool/us-east-1_XXXXXXXXX"
    }
  ]
}
```

**Nota**: Essas são permissões separadas:
- **IAM User do GitHub Actions**: Deploy do código
- **Lambda Execution Role**: Execução da aplicação

---

## 🚀 Processo de Deploy

### Deploy Automático
O workflow é executado automaticamente nos seguintes eventos:

1. **Push para branch `main`**: Deploy direto em produção
2. **Pull Request para `main`**: Valida build (não faz deploy)

### Deploy Manual
Permite deploy em qualquer branch:

1. Acessar: **Actions** → **Deploy to AWS Lambda** → **Run workflow**
2. Selecionar branch desejada
3. (Opcional) Customizar inputs:
   - **lambda_function_name**: Nome do Lambda (override)
   - **aws_region**: Região AWS (override)
4. Clicar em **Run workflow**

---

## 🔍 Verificação Pós-Deploy

### 1. Verificar no GitHub Actions
- Acessar **Actions** → workflow recém-executado
- Verificar logs de cada step
- Confirmar step "Verify deployment" mostra informações corretas

### 2. Verificar no AWS Lambda
```bash
aws lambda get-function \
  --function-name video-processing-engine-dev-auth \
  --region us-east-1
```

### 3. Testar API
```bash
# Health check
curl https://seu-api-gateway.execute-api.us-east-1.amazonaws.com/health

# Login (exemplo)
curl -X POST https://seu-api-gateway.execute-api.us-east-1.amazonaws.com/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"user@example.com","password":"Password123!"}'
```

---

## 🐛 Troubleshooting

### Erro: "AccessDenied" no Deploy
**Causa**: IAM User não tem permissões de `lambda:UpdateFunctionCode`
**Solução**: Adicionar permissões IAM necessárias (ver seção Permissões IAM)

### Erro: "ResourceNotFoundException: Function not found"
**Causa**: Nome do Lambda incorreto ou região AWS incorreta
**Solução**: Verificar `LAMBDA_FUNCTION_NAME` e `AWS_REGION` nas GitHub Variables

### Erro: Build falhou no workflow
**Causa**: Problemas no código, dependências ou testes
**Solução**: Executar build localmente para reproduzir erro: `dotnet build --configuration Release`

### Lambda retorna erro 500 após deploy
**Causa**: Variáveis de ambiente do Lambda não configuradas corretamente
**Solução**: Verificar variáveis `Cognito__*` no Lambda Configuration

### Deploy demora muito tempo
**Causa**: Pacote ZIP muito grande
**Solução**: Verificar tamanho do ZIP (limite: 50MB comprimido). Considerar usar Layers para dependências grandes.

---

## 📦 Estrutura do Pacote de Deploy

O workflow gera um ZIP com a seguinte estrutura:

```
deployment-package.zip
├── VideoProcessing.Auth.Api.dll
├── VideoProcessing.Auth.Api.deps.json
├── VideoProcessing.Auth.Api.runtimeconfig.json
├── VideoProcessing.Auth.Application.dll
├── VideoProcessing.Auth.Infra.dll
├── appsettings.json
├── appsettings.Development.json (se existir)
├── AWSSDK.*.dll (dependências AWS)
├── FluentValidation.dll
├── ... (outras dependências)
└── ... (demais binários)
```

---

## 🔮 Próximos Passos / Melhorias Futuras

- [ ] Adicionar step de testes com cobertura mínima de 70% antes do deploy
- [ ] Adicionar notificações (Slack, email) sobre status do deploy
- [ ] Implementar estratégia de rollback automático em caso de falha
- [ ] Adicionar ambientes (staging, production) com GitHub Environments
- [ ] Implementar versionamento de Lambda (aliases e versions)

---

## 📚 Referências

- [GitHub Actions Documentation](https://docs.github.com/en/actions)
- [AWS Lambda Documentation](https://docs.aws.amazon.com/lambda/)
- [AWS CLI Lambda Commands](https://awscli.amazonaws.com/v2/documentation/api/latest/reference/lambda/index.html)
- [.NET on AWS Lambda](https://docs.aws.amazon.com/lambda/latest/dg/csharp-handler.html)
```

## Passos de Implementação

1. Criar pasta `docs/` se não existir
2. Criar arquivo `docs/deploy-github-actions.md` com estrutura completa
3. Documentar todos os GitHub Secrets com exemplos
4. Documentar todas as GitHub Variables com valores padrão
5. Documentar variáveis de ambiente do Lambda (Cognito configuration)
6. Documentar permissões IAM necessárias (deploy e runtime)
7. Documentar processo de deploy automático e manual
8. Adicionar seção de troubleshooting com problemas comuns
9. Adicionar verificação pós-deploy
10. Adicionar referências úteis

## Critérios de Aceite

- [ ] Arquivo `docs/deploy-github-actions.md` criado
- [ ] Todos os GitHub Secrets documentados (AWS_ACCESS_KEY_ID, AWS_SECRET_ACCESS_KEY)
- [ ] Todas as GitHub Variables documentadas (AWS_REGION, LAMBDA_FUNCTION_NAME)
- [ ] Variáveis de ambiente do Lambda documentadas (Cognito__Region, Cognito__UserPoolId, Cognito__ClientId)
- [ ] Instruções de como configurar secrets e variáveis no GitHub incluídas
- [ ] Permissões IAM para deploy documentadas (com exemplo de política JSON)
- [ ] Permissões IAM para runtime da aplicação documentadas
- [ ] Processo de deploy automático explicado
- [ ] Processo de deploy manual explicado
- [ ] Seção de verificação pós-deploy incluída
- [ ] Seção de troubleshooting com problemas comuns incluída
- [ ] Comandos AWS CLI de exemplo incluídos
- [ ] Estrutura do pacote ZIP documentada

## Notas

- Documentação deve ser clara e acessível para desenvolvedores com diferentes níveis de experiência
- Exemplos práticos e comandos prontos para copiar/colar facilitam a configuração
- Separar claramente permissões IAM de deploy vs. runtime da aplicação
- Incluir valores de exemplo realistas (mas não credenciais reais)
- Considerar adicionar diagramas de fluxo do processo de deploy (futuro)
- A notação `__` (double underscore) nas variáveis do Lambda representa hierarquia JSON (ex: `Cognito__Region` → `Cognito.Region`)
