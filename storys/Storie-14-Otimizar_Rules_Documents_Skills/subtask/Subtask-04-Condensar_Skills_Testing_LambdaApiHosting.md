# Subtask 04 — Condensar Skills de Testes e Lambda API Hosting

**Status:** ✅ Concluído  
**Responsável:** Agente AI  
**Tempo estimado:** 20min

---

## Objetivo

Condensar 2 skills finais (exceto technical-stories que não será alterada):
- `testing/SKILL.md` (~500 linhas → ~150 linhas)
- `lambda-api-hosting/SKILL.md` (~200 linhas → pode ficar ~150 linhas por ser mais específica)

---

## Passos

1. **Testing:**
   - Focar em: estrutura de testes unitários (xUnit), BDD (SpecFlow), cobertura mínima
   - Remover: exemplos exaustivos, técnicas avançadas de mock
   - Manter: padrões AAA, nomenclatura, estrutura de pastas, comando de build/teste
   - Links para xUnit e SpecFlow docs

2. **Lambda API Hosting:**
   - Focar em: AddAWSLambdaHosting, GATEWAY_PATH_PREFIX, GATEWAY_STAGE, configuração OpenAPI
   - Manter: exemplo de Program.cs, configuração de ambiente, troubleshooting crítico
   - Já é relativamente enxuta; apenas remover redundâncias

3. **Não alterar:** `technical-stories/SKILL.md` (já otimizada e específica)

---

## Entregáveis

- [x] `testing/SKILL.md` condensado (~150 linhas)
- [x] `lambda-api-hosting/SKILL.md` revisado (~150 linhas ou menos)

---

## Critérios de Aceite

- ✅ Testing skill condensado mantendo essenciais: estrutura, BDD, cobertura, build
- ✅ Lambda API Hosting skill mantém configurações críticas (GATEWAY_*, OpenAPI)
- ✅ Technical-stories skill não foi modificada
- ✅ Ambas skills escaneáveis em <2 min
- ✅ Links para docs oficiais adicionados
