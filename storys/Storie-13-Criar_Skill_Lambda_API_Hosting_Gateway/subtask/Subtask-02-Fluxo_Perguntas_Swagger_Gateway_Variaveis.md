# Subtask 02: Fluxo de perguntas (OpenAPI Scalar/Swagger, Gateway) e variáveis de ambiente

## Descrição
Documentar na skill o fluxo de perguntas ao usuário (usará documentação OpenAPI com Scalar ou Swagger UI? usará API Gateway?) e a tabela de variáveis de ambiente GATEWAY_STAGE e GATEWAY_PATH_PREFIX, com uso e exemplos.

## Passos de implementação
1. Incluir na skill um fluxo tipo "Antes de configurar, perguntar ao usuário: (1) Usará documentação OpenAPI (Scalar ou Swagger UI)? (2) A API será exposta atrás de API Gateway (HTTP API) com prefixo de path e/ou stage nomeado?"
2. Tabela de variáveis de ambiente com: Nome (GATEWAY_STAGE, GATEWAY_PATH_PREFIX, opcionalmente API_PUBLIC_BASE_URL), Uso, Obrigatoriedade, Exemplo.
3. Explicar: GATEWAY_STAGE = nome do stage (ex.: dev, default) para o middleware remover do path quando o stage não é $default; GATEWAY_PATH_PREFIX = prefixo (ex.: /auth) para o middleware remover do path.
4. Referenciar ou resumir exemplos de path (ex.: request `/default/auth/health` com GATEWAY_STAGE=default e GATEWAY_PATH_PREFIX=/auth → Path = `/health`).

## Formas de teste
- Seguir o fluxo da skill e confirmar que as perguntas estão na ordem certa e que as variáveis ficam claras.
- Comparar com `docs/gateway-path-prefix.md` e garantir consistência.

## Critérios de aceite da subtask
- [ ] Skill contém fluxo explícito: perguntar ao usuário se usará documentação OpenAPI (Scalar ou Swagger UI) e se usará Gateway antes de aplicar tratamentos.
- [ ] Tabela de variáveis GATEWAY_STAGE e GATEWAY_PATH_PREFIX com uso e exemplos.
- [ ] Menção a API_PUBLIC_BASE_URL (opcional) quando OpenAPI (Scalar ou Swagger) for usado atrás do gateway.
- [ ] Pelo menos um exemplo de path antes/depois do middleware (stage + prefix).
