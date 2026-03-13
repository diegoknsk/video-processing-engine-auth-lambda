# Subtask 04: Configurar Branch Protection Rule para bloquear merge sem Quality Gate aprovado

## Descrição
Configurar a Branch Protection Rule da branch `main` no GitHub para exigir que o status check do SonarCloud (`SonarCloud Code Analysis` ou o nome do check configurado) passe antes de permitir o merge de qualquer PR.

## Passos de Implementação
1. Acessar Settings > Branches > Branch protection rules no repositório GitHub.
2. Editar (ou criar) a regra para a branch `main` com as seguintes configurações:
   - **Require a pull request before merging:** habilitado
   - **Require status checks to pass before merging:** habilitado
   - Adicionar o status check `SonarCloud Analysis` (nome exato do job definido no workflow) como obrigatório
   - Adicionar também o status check do build existente como obrigatório
   - **Do not allow bypassing the above settings:** habilitado para bloquear inclusive admins
3. Configurar no SonarCloud o Quality Gate padrão com threshold mínimo de:
   - Cobertura em novo código: ≥ 70%
   - Sem bugs ou vulnerabilidades de severidade `CRITICAL` ou `BLOCKER` introduzidos
   - Rating de manutenção (Maintainability Rating) em novo código: A
4. Ativar o webhook do SonarCloud para o repositório GitHub (SonarCloud > Project Settings > GitHub > Enable) para que o Quality Gate reporte o status diretamente na PR.

## Formas de Teste
1. Abrir uma PR com código sem problemas e cobertura acima de 70%: verificar que o merge é liberado após todos os checks passarem.
2. Abrir uma PR com cobertura abaixo do threshold configurado: verificar que o merge é bloqueado pelo status check do SonarCloud.
3. Tentar fazer merge diretamente na `main` sem PR: verificar que a Branch Protection Rule bloqueia o push direto.

## Critérios de Aceite
- [ ] A Branch Protection Rule da `main` exige o status check `SonarCloud Analysis` como obrigatório
- [ ] PRs com Quality Gate reprovado têm o merge bloqueado pelo GitHub
- [ ] O Quality Gate do SonarCloud está configurado com threshold de cobertura ≥ 70% e sem issues críticas em novo código
