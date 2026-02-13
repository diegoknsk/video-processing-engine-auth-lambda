# Subtask 03: Configurar Triggers Automáticos e Manuais do Workflow

## Descrição
Configurar os triggers do workflow do GitHub Actions para que ele seja executado automaticamente em eventos de push/PR para a branch `main` e também permita execução manual (`workflow_dispatch`) em qualquer branch, permitindo deploys sob demanda para testes e validações.

## Objetivos
1. Configurar trigger automático para push/PR na branch `main`
2. Configurar trigger manual (`workflow_dispatch`) para qualquer branch
3. Adicionar inputs opcionais no workflow_dispatch para customização
4. Garantir que apenas um deploy execute por vez (evitar conflitos)
5. Adicionar badges/status no README para visualizar status do workflow

## Detalhes Técnicos

### Configuração de Triggers no Workflow

```yaml
name: Deploy to AWS Lambda

on:
  # Trigger automático: push ou PR para main
  push:
    branches:
      - main
  pull_request:
    branches:
      - main
  
  # Trigger manual: permite executar em qualquer branch
  workflow_dispatch:
    inputs:
      lambda_function_name:
        description: 'Nome do Lambda (opcional, usa variável se vazio)'
        required: false
        type: string
      aws_region:
        description: 'Região AWS (opcional, usa variável se vazio)'
        required: false
        type: string

env:
  DOTNET_VERSION: '10.x'
  # Usa input manual se fornecido, senão usa variável do GitHub, senão usa valor padrão
  AWS_REGION: ${{ inputs.aws_region || vars.AWS_REGION || 'us-east-1' }}
  LAMBDA_FUNCTION_NAME: ${{ inputs.lambda_function_name || vars.LAMBDA_FUNCTION_NAME || 'video-processing-engine-dev-auth' }}

jobs:
  deploy:
    name: Build and Deploy Lambda
    runs-on: ubuntu-latest
    
    # Previne execuções concorrentes do mesmo workflow
    concurrency:
      group: deploy-lambda-${{ github.ref }}
      cancel-in-progress: false
    
    steps:
      # ... steps das subtasks anteriores ...
```

### Explicação das Configurações

#### Triggers (`on`)

1. **push: branches: [main]**: Executa automaticamente quando há push direto na main
2. **pull_request: branches: [main]**: Executa quando um PR é aberto/atualizado para main (útil para validar build antes de merge)
3. **workflow_dispatch**: Permite execução manual via interface do GitHub
   - `inputs.lambda_function_name`: Permite sobrescrever o nome do Lambda manualmente
   - `inputs.aws_region`: Permite sobrescrever a região AWS manualmente

#### Variáveis de Ambiente (env)

Usa ordem de precedência (fallback chain):
1. Input manual (`inputs.aws_region`) - maior prioridade
2. GitHub Variable (`vars.AWS_REGION`) - prioridade média
3. Valor padrão hardcoded (`'us-east-1'`) - fallback final

Isso permite:
- Execução automática com valores padrão
- Customização via GitHub Variables para ambientes
- Override manual quando necessário

#### Concurrency

```yaml
concurrency:
  group: deploy-lambda-${{ github.ref }}
  cancel-in-progress: false
```

- Previne múltiplos deploys simultâneos para o mesmo Lambda
- Usa `github.ref` (branch) como grupo de concorrência
- `cancel-in-progress: false` faz deploys aguardarem na fila (não cancela em progresso)

### Badge para README

Adicionar badge no README para mostrar status do workflow:

```markdown
[![Deploy to AWS Lambda](https://github.com/OWNER/REPO/actions/workflows/deploy-lambda.yml/badge.svg)](https://github.com/OWNER/REPO/actions/workflows/deploy-lambda.yml)
```

## Passos de Implementação

1. Configurar seção `on` no workflow com push, pull_request e workflow_dispatch
2. Adicionar inputs opcionais no workflow_dispatch (lambda_function_name, aws_region)
3. Ajustar variáveis de ambiente (env) para usar fallback chain (inputs > vars > padrão)
4. Adicionar seção `concurrency` no job para prevenir deploys simultâneos
5. Testar trigger automático (fazer commit na main ou abrir PR)
6. Testar trigger manual (executar via interface do GitHub Actions)
7. Adicionar badge no README (será feito na Subtask 05)

## Critérios de Aceite

- [ ] Trigger `push` para branch `main` configurado
- [ ] Trigger `pull_request` para branch `main` configurado
- [ ] Trigger `workflow_dispatch` configurado com inputs opcionais
- [ ] Input `lambda_function_name` permite customizar nome do Lambda manualmente
- [ ] Input `aws_region` permite customizar região AWS manualmente
- [ ] Variáveis de ambiente usam fallback chain (inputs > vars > padrão)
- [ ] Concurrency configurada para prevenir deploys simultâneos
- [ ] Workflow executa automaticamente em push/PR para main
- [ ] Workflow pode ser executado manualmente em qualquer branch
- [ ] Execução manual permite override de nome do Lambda e região via inputs

## Notas

- O trigger `pull_request` permite validar o build antes de mergear, mas não faz deploy de fato (pode adicionar condição `if: github.event_name != 'pull_request'` no step de deploy se quiser apenas validar build em PRs)
- A estrutura de `workflow_dispatch` com inputs é útil para testes e deploys em ambientes diferentes
- O `concurrency` é importante para evitar race conditions no Lambda
- O badge no README será adicionado na Subtask 05 (atualização do README)
- Considerar adicionar environment protection rules no GitHub para deploys em produção (futuro)
