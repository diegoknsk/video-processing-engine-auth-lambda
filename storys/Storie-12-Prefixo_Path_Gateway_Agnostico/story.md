# Storie-12: Prefixo de Path do Gateway Agnóstico (PathBase configurável)

## Status
- **Estado:** 🔄 Em desenvolvimento
- **Data de Conclusão:** —

## Descrição
Como desenvolvedor da API Auth, quero que a aplicação funcione atrás de qualquer prefixo de path no API Gateway (ex.: /auth, /Auth, /autenticacao), para que a mesma Lambda sirva em ambientes locais (sem prefixo) e atrás do gateway sem hardcodar o prefixo no código.

## Objetivo
Implementar um middleware que leia a variável de ambiente `GATEWAY_PATH_PREFIX`: quando vazia ou ausente, manter o comportamento atual (path inalterado); quando preenchida, remover esse prefixo do path de forma case-insensitive e definir PathBase/Path, permitindo que as rotas da aplicação sejam agnósticas ao gateway.

## Escopo Técnico
- Tecnologias: ASP.NET Core, .NET 10
- Arquivos afetados:
  - `src/VideoProcessing.Auth.Api/Middleware/` (novo middleware)
  - `src/VideoProcessing.Auth.Api/Program.cs`
  - `src/VideoProcessing.Auth.Api/Controllers/Auth/AuthController.cs`
  - `src/VideoProcessing.Auth.Api/Controllers/Auth/UserController.cs`
  - Documentação (docs ou README) com a variável e comportamento
- Componentes: GatewayPathBaseMiddleware (ou nome equivalente), ajuste de rotas para /health, /login, /users/create
- Pacotes/Dependências: nenhum novo; uso de IConfiguration ou Environment.GetEnvironmentVariable

## Dependências e Riscos (para estimativa)
- Dependências: Nenhuma outra story.
- Riscos/Pré-condições: Rotas atuais /auth/login e /auth/users/create passam a ser /login e /users/create; clientes e documentação (OpenAPI) devem refletir a URL final (gateway + path).

## Subtasks
- [ ] [Subtask 01: Criar middleware PathBase com env e comparação case-insensitive](./subtask/Subtask-01-Middleware_PathBase_Env_CaseInsensitive.md)
- [ ] [Subtask 02: Ajustar rotas dos controllers para path agnóstico](./subtask/Subtask-02-Rotas_Controllers_Agnosticas.md)
- [ ] [Subtask 03: Registrar middleware e documentar variável GATEWAY_PATH_PREFIX](./subtask/Subtask-03-Registrar_Middleware_Documentar.md)
- [ ] [Subtask 04: Testes unitários do middleware](./subtask/Subtask-04-Testes_Unitarios_Middleware.md)

## Critérios de Aceite da História
- [ ] Variável `GATEWAY_PATH_PREFIX` não definida ou vazia: request path não é alterado (comportamento igual ao atual; ex.: GET /health continua /health).
- [ ] Variável `GATEWAY_PATH_PREFIX` definida (ex.: `/auth`): path que começa com o prefixo (maiúsculo ou minúsculo) tem o prefixo removido e PathBase definido; ex.: /auth/health ou /Auth/health → PathBase=/auth (ou valor do request), Path=/health.
- [ ] Rotas da aplicação expostas como /health, /login, /users/create; funcionam localmente e atrás do gateway com prefixo.
- [ ] Documentação descreve o nome da variável, o comportamento (vazio vs preenchido) e o uso case-insensitive.
- [ ] Testes unitários cobrindo o middleware (path sem prefixo, com prefixo, case-insensitive, env vazia).

## Rastreamento (dev tracking)
- **Início:** 13/02/2026, às 22:33 (Brasília)
- **Fim:** —
- **Tempo total de desenvolvimento:** —
