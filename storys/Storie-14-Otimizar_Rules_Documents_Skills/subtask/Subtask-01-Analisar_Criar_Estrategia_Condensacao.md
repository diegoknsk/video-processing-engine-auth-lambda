# Subtask 01 — Analisar e Criar Estratégia de Condensação

**Status:** ✅ Concluído  
**Responsável:** Agente AI  
**Tempo estimado:** 15min

---

## Objetivo

Analisar todas as 9 skills atuais e criar estratégia de condensação documentada, definindo:
- Template padrão para skills condensadas (~150 linhas)
- Quais seções manter/remover de cada skill
- Como mesclar documents (dotnet-conventions, clean-architecture-spec) nas skills
- Estrutura do quick-reference.md

---

## Passos

1. Ler todas as 9 skills atuais e mapear seções por tamanho
2. Identificar padrões comuns e redundâncias
3. Criar template padrão de skill condensada:
   - Frontmatter (name, description, gatilhos)
   - Quando usar (3-5 linhas)
   - Princípios (❌/✅) — máx 10 itens
   - Checklist rápido — 5-8 passos
   - 1 exemplo minimal — apenas código essencial
   - Referências (opcional)
4. Definir como mesclar documents:
   - dotnet-conventions → eliminar (já está nas rules)
   - clean-architecture-spec → eliminar (já está nas rules)
   - skills-index → eliminar (tabela vai para rules)
5. Criar estrutura do quick-reference.md (1 página, máx 80 linhas)

---

## Entregáveis

- [x] Documento `estrategia-condensacao.md` em `.cursor/documents/` com template e decisões
- [x] Rascunho de `quick-reference.md` (estrutura, não conteúdo final)
- [x] Lista de seções a remover/manter por skill

---

## Critérios de Aceite

- ✅ Template de skill condensada definido (máx ~150 linhas)
- ✅ Decisão clara sobre documents (manter/eliminar/mesclar)
- ✅ Estrutura do quick-reference definida (tabela + princípios-chave)
- ✅ Nenhuma informação crítica será perdida (validado na análise)
