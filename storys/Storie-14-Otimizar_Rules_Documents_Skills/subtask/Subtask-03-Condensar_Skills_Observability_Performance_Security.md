# Subtask 03 — Condensar Skills de Observabilidade, Performance e Segurança

**Status:** ✅ Concluído  
**Responsável:** Agente AI  
**Tempo estimado:** 25min

---

## Objetivo

Condensar 3 skills de infraestrutura aplicando o template:
- `observability/SKILL.md` (~400 linhas → ~150 linhas)
- `performance-optimization/SKILL.md` (~450 linhas → ~150 linhas)
- `security/SKILL.md` (~380 linhas → ~150 linhas)

---

## Passos

1. Para cada skill:
   - Aplicar template da Subtask 01
   - Observability: focar em logging estruturado + OpenTelemetry básico + health checks
   - Performance: focar em Span<T>, Memory<T>, ArrayPool, ValueTask (técnicas mais comuns)
   - Security: focar em JWT, secrets management, rate limiting, CORS
   - Remover: exemplos exaustivos, técnicas avançadas menos usadas
2. Adicionar links para docs oficiais (OpenTelemetry, Microsoft Security)
3. Validar que decisões críticas permanecem

---

## Entregáveis

- [x] `observability/SKILL.md` condensado (~150 linhas)
- [x] `performance-optimization/SKILL.md` condensado (~150 linhas)
- [x] `security/SKILL.md` condensado (~150 linhas)

---

## Critérios de Aceite

- ✅ Cada skill tem máximo ~150 linhas (tolerância ±20 linhas)
- ✅ Foca nas técnicas/padrões mais comuns (80/20)
- ✅ Mantém decisões críticas e anti-patterns importantes
- ✅ Links para docs oficiais adicionados
- ✅ Exemplos mínimos mas suficientes para entendimento
