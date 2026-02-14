# Subtask 04: OpenAPI (Scalar ou Swagger) com Gateway (filter server, API_PUBLIC_BASE_URL)

## Descrição
Documentar na skill o tratamento de OpenAPI quando a API está atrás do API Gateway: preenchimento do server do OpenAPI a partir do request (PathBase + stage) para que o "Try it" na Scalar ou no Swagger UI use URLs corretas; uso opcional de API_PUBLIC_BASE_URL como fallback.

## Passos de implementação
1. Se o usuário indicar que usará documentação OpenAPI (Scalar ou Swagger UI) e Gateway: instruir a configurar um DocumentFilter (ou equivalente) que define o Server do OpenAPI com base no request: Scheme + Host + segmento de stage (GATEWAY_STAGE) + PathBase, para que a UI (Scalar ou Swagger) gere URLs como `https://.../dev/auth/login`.
2. Documentar que o PathBase no request já reflete o prefixo (ex.: /auth); o stage foi removido pelo middleware, então o filter deve ler GATEWAY_STAGE e prepender ao path do server.
3. Opcional: se API_PUBLIC_BASE_URL estiver definida, usar como server adicional ou fallback quando o documento for gerado sem contexto de request (casos edge).
4. Referenciar AddServer em SwaggerGen com API_PUBLIC_BASE_URL quando preenchida e registrar o filter que sobrescreve Servers a partir do request.

## Formas de teste
- Em um projeto com gateway + OpenAPI (Scalar ou Swagger), acessar a doc pelo gateway e usar "Try it" em um endpoint; a URL deve incluir stage e prefixo.
- Verificar consistência com `OpenApiServerFromRequestFilter` e `docs/gateway-path-prefix.md`.

## Critérios de aceite da subtask
- [ ] Skill descreve o uso de um DocumentFilter (ou equivalente) para definir OpenAPI Server a partir do request quando OpenAPI (Scalar ou Swagger) + Gateway.
- [ ] Inclusão do segmento de stage (GATEWAY_STAGE) no server URL está explicada (PathBase não contém stage após middleware).
- [ ] API_PUBLIC_BASE_URL documentada como opcional/fallback.
- [ ] Integração com Swashbuckle (AddServer, DocumentFilter) referenciada; aplicável tanto a Swagger UI quanto a Scalar (ambos consomem o mesmo JSON OpenAPI).
