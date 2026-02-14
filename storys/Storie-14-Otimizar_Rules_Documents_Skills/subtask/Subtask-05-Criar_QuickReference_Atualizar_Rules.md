# Subtask 05 — Criar Quick-Reference e Atualizar Rules

**Status:** ✅ Concluído  
**Responsável:** Agente AI  
**Tempo estimado:** 20min

---

## Objetivo

Criar `quick-reference.md` (1 página, ~80 linhas) e atualizar rules para referenciar skills diretamente (eliminar skills-index como intermediário).

---

## Passos

1. **Criar `.cursor/documents/quick-reference.md`:**
   - Tabela de decisão: gatilhos → skill (1 linha por skill)
   - Princípios-chave .NET (5-8 itens mais críticos)
   - Princípios-chave Clean Architecture (5-8 itens mais críticos)
   - Quando ler documentação completa vs skill
   - Máximo 80 linhas (cabe em 1 tela/página)

2. **Atualizar `core-dotnet.mdc`:**
   - Remover referência a skills-index.md
   - Adicionar tabela inline com gatilhos → skills (como era antes, mas enxuta)
   - Adicionar referência ao quick-reference.md

3. **Atualizar `core-clean-architecture.mdc`:**
   - Mesma abordagem: tabela inline de gatilhos → skills
   - Referência ao quick-reference.md

4. **Validar:**
   - Rules continuam ~15 linhas (ou até ~25 linhas com tabela inline pequena)
   - Quick-reference escaneável em <1 min

---

## Entregáveis

- [x] `.cursor/documents/quick-reference.md` criado (~80 linhas)
- [x] `core-dotnet.mdc` atualizado (skills referenciadas diretamente)
- [x] `core-clean-architecture.mdc` atualizado (skills referenciadas diretamente)

---

## Critérios de Aceite

- ✅ Quick-reference tem máximo 80 linhas (1 página escaneável)
- ✅ Tabela de decisão clara: gatilhos → skill (9 skills listadas)
- ✅ Princípios-chave das duas rules resumidos no quick-ref
- ✅ Rules atualizado para referenciar skills diretamente (não via index)
- ✅ Rules continuam agnósticas e reutilizáveis
- ✅ Nenhuma skill perdida na refatoração
