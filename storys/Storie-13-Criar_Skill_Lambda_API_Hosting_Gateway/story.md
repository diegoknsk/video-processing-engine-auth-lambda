# Storie-13: Criar Skill Lambda API Hosting e Gateway

## Status
- **Estado:** 🔄 Em desenvolvimento
- **Data de Conclusão:** [DD/MM/AAAA]

## Descrição
Como desenvolvedor que cria novas Lambdas com API .NET, quero uma skill Cursor que oriente a configuração de API .NET 10 com AddAWSLambdaHosting e os tratamentos de API Gateway (GATEWAY_STAGE, GATEWAY_PATH_PREFIX) e documentação OpenAPI (Scalar ou Swagger), para economizar tempo e evitar erros repetidos de path e documentação (Scalar ou Swagger) ao montar novos lambdas.

## Objetivo
Entregar uma **skill** (arquivo SKILL.md em `.cursor/skills/`) que, em combinação com as rules e skills existentes (core-dotnet, core-clean-architecture, etc.), guie a criação e configuração de novas APIs .NET 10 hospedadas em AWS Lambda com AddAWSLambdaHosting. A skill deve cobrir: (1) bootstrap da API com AddAWSLambdaHosting; (2) pergunta ao usuário se usará documentação OpenAPI (Scalar ou Swagger UI) e se usará API Gateway; (3) quando Gateway for usado, aplicação dos tratamentos de GATEWAY_STAGE e GATEWAY_PATH_PREFIX (middleware, variáveis de ambiente, OpenAPI quando houver doc); (4) outros cenários relevantes (Handler, timeout, evento HTTP API v2). Questões de arquitetura (Clean Architecture, camadas) ficam a cargo de outras rules/skills.

## Escopo Técnico
- Tecnologias: .NET 10, ASP.NET Core, Amazon.Lambda.AspNetCoreServer.Hosting, API Gateway HTTP API (v2)
- Arquivos afetados: `.cursor/skills/<nome-skill>/SKILL.md` (novo); opcionalmente referência em `.cursor/rules/` ou documentação em `docs/`
- Componentes: Conteúdo da skill (gatilhos, fluxo de perguntas, trechos de código/instruções para middleware, OpenAPI filter, Program.cs, variáveis de ambiente, checklist IaC)
- Pacotes/Dependências: Nenhum novo pacote no repositório atual; a skill documenta uso de `Amazon.Lambda.AspNetCoreServer.Hosting` e, quando documentação OpenAPI: Swashbuckle (Swagger UI) e/ou Scalar e filter de server OpenAPI

## Dependências e Riscos (para estimativa)
- Dependências: Documentação existente (`docs/gateway-path-prefix.md`, `docs/lambda-handler-addawslambdahosting.md`); código de referência (GatewayPathBaseMiddleware, OpenApiServerFromRequestFilter, Program.cs).
- Riscos: Nenhum crítico; a skill é apenas guia para o agente/humano, não altera o comportamento do projeto atual.

## Subtasks
- [Subtask 01: Definir escopo, nome e gatilhos da skill](./subtask/Subtask-01-Definir_Escopo_Nome_Gatilhos_Skill.md)
- [Subtask 02: Fluxo de perguntas (OpenAPI Scalar/Swagger, Gateway) e variáveis](./subtask/Subtask-02-Fluxo_Perguntas_Swagger_Gateway_Variaveis.md)
- [Subtask 03: Instruções AddAWSLambdaHosting pipeline e middleware Gateway](./subtask/Subtask-03-AddAWSLambdaHosting_Pipeline_Middleware.md)
- [Subtask 04: OpenAPI (Scalar ou Swagger) com Gateway (filter server, API_PUBLIC_BASE_URL)](./subtask/Subtask-04-OpenAPI_Swagger_Com_Gateway.md)
- [Subtask 05: Outros cenários e checklist IaC (Handler, timeout, evento v2)](./subtask/Subtask-05-Outros_Cenarios_Checklist_IaC.md)
- [Subtask 06: Redação final SKILL.md e referência em rules se aplicável](./subtask/Subtask-06-Redacao_Final_Skill_Referencia.md)

## Critérios de Aceite da História
- [ ] Skill criada em `.cursor/skills/<nome-skill>/SKILL.md` com frontmatter (name, description) e gatilhos claros (ex.: "novo lambda api", "AddAWSLambdaHosting", "API Gateway")
- [ ] Skill orienta a perguntar ao usuário se usará documentação OpenAPI (Scalar ou Swagger UI) e se usará API Gateway antes de aplicar tratamentos
- [ ] Quando Gateway for usado, skill descreve ou referencia GATEWAY_STAGE e GATEWAY_PATH_PREFIX com instruções para middleware de path (ordem: stage depois prefix), variáveis de ambiente e posição no pipeline (antes de UseRouting)
- [ ] Quando OpenAPI (Scalar ou Swagger) + Gateway: skill cobre OpenAPI server (filter a partir do request e/ou API_PUBLIC_BASE_URL) para "Try it" correto em qualquer uma das UIs
- [ ] Outros cenários documentados: Handler (nome do assembly), timeout mínimo recomendado (ex.: 30s), formato de evento HTTP API v2 para testes; checklist breve para IaC
- [ ] Skill deixa explícito que arquitetura (Clean Architecture, camadas) é responsabilidade de outras rules/skills; foco em bootstrap e gateway para economizar tempo
- [ ] Documentação interna (docs) ou referência em rules atualizada para apontar à nova skill quando aplicável

## Rastreamento (dev tracking)
- **Início:** —
- **Fim:** —
- **Tempo total de desenvolvimento:** —
