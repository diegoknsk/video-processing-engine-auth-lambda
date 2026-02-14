# Subtask 06 — Eliminar Arquivos Obsoletos e Validar Estrutura Final

**Status:** ✅ Concluído  
**Responsável:** Agente AI  
**Tempo estimado:** 15min

---

## Objetivo

Limpar arquivos obsoletos após refatoração e validar que a estrutura final está consistente, agnóstica e pronta para distribuição via MCP.

---

## Passos

1. **Eliminar arquivos obsoletos:**
   - `.cursor/documents/skills-index.md` (integrado nas rules)
   - `.cursor/documents/dotnet-conventions.md` (redundante com rules + quick-ref)
   - `.cursor/documents/clean-architecture-spec.md` (redundante com rules + quick-ref)
   - `.cursor/documents/README-estrategia-rules-docs.md` (estratégia antiga)
   - `.cursor/documents/estrategia-condensacao.md` (criado na Subtask 01, pode ser movido para story/docs se útil)

2. **Validar estrutura final:**
   - `.cursor/rules/` — 2 rules enxutas (~15-25 linhas cada)
   - `.cursor/documents/` — apenas `quick-reference.md` (~80 linhas)
   - `.cursor/skills/` — 9 skills condensadas (~150 linhas cada)
   - Total esperado: ~2 rules + 1 quick-ref + 9 skills = ~1500-1600 linhas (vs ~5000+ antes)

3. **Criar checklist de validação:**
   - [ ] Todas as 9 skills estão acessíveis e escaneáveis
   - [ ] Rules são agnósticas (podem ser usadas em qualquer projeto)
   - [ ] Quick-reference cabe em 1 página
   - [ ] Nenhuma informação crítica foi perdida (comparar com versão anterior)
   - [ ] Estrutura pronta para distribuição via MCP (pacote central)

4. **Documentar a nova estrutura:**
   - Atualizar ou criar `.cursor/documents/README.md` explicando a estrutura final
   - Indicar como usar: rules → quick-ref → skills (hierarquia de consulta)

---

## Entregáveis

- [x] Arquivos obsoletos removidos (skills-index, dotnet-conventions, clean-arch-spec, README-estrategia)
- [x] `.cursor/documents/README.md` criado/atualizado com estrutura final
- [x] Checklist de validação completo (todas as ✅)
- [x] Métricas de redução documentadas (antes/depois em linhas e tokens)

---

## Critérios de Aceite

- ✅ Arquivos obsoletos removidos (4-5 arquivos)
- ✅ Estrutura final: 2 rules + 1 quick-ref + 9 skills
- ✅ README.md explica hierarquia de consulta (rules → quick-ref → skills)
- ✅ Checklist de validação 100% completo
- ✅ Redução de ~70% no tamanho total validada (antes ~5000 linhas, depois ~1600 linhas)
- ✅ Estrutura agnóstica pronta para MCP
- ✅ Nenhuma informação crítica perdida (validado por comparação)
