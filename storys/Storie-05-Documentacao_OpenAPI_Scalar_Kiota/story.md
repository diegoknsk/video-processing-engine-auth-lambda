# Storie-05: Documentação OpenAPI com Scalar UI e Geração de Client (Kiota)

## Status
- **Estado:** 🔄 Em desenvolvimento
- **Data de Conclusão:** —

## Descrição
Como desenvolvedor que consome a API de autenticação, quero ter acesso a documentação interativa completa via Scalar UI e especificação OpenAPI JSON, para entender contratos dos endpoints, testar chamadas manualmente, e gerar clients tipados automaticamente com Kiota.

## Objetivo
Configurar geração de especificação OpenAPI JSON, servir documentação interativa com Scalar UI nos endpoints /docs e /openapi/v1.json, adicionar XML comments completos aos controllers para enriquecer a documentação, ajustar base path se necessário para Lambda + API Gateway, e documentar comando Kiota para geração de client C# a partir do OpenAPI.

## Escopo Técnico
- Tecnologias: .NET 10, Swashbuckle.AspNetCore, Scalar.AspNetCore, OpenAPI 3.0
- Arquivos afetados:
  - `src/VideoProcessing.Auth.Api/Program.cs` (atualizar para configurar Swagger e Scalar)
  - `src/VideoProcessing.Auth.Api/*.csproj` (habilitar geração de XML documentation)
  - `src/VideoProcessing.Auth.Api/Controllers/Auth/AuthController.cs` (adicionar XML comments completos)
  - `src/VideoProcessing.Auth.Api/Controllers/Auth/UserController.cs` (adicionar XML comments completos)
  - `src/VideoProcessing.Auth.Api/Controllers/HealthController.cs` (adicionar XML comments completos)
  - `docs/kiota-client-generation.md` (novo - documentação de geração de client)
  - `README.md` (atualizar com links para /docs e instruções de uso)
- Componentes/Recursos: Swashbuckle (OpenAPI generation), Scalar UI, XML documentation, Kiota (tooling externo)
- Pacotes/Dependências (já instalados na Story 01):
  - Swashbuckle.AspNetCore (7.2.0)
  - Scalar.AspNetCore (1.3.0)
  - Microsoft.AspNetCore.OpenApi (10.0.0)

## Dependências e Riscos (para estimativa)
- Dependências: Story 01 concluída; Stories 02, 03, 04 desejáveis (endpoints já implementados para documentar)
- Riscos/Pré-condições:
  - Kiota CLI instalado globalmente (ou documentar instalação via `dotnet tool install`)
  - Conhecimento de OpenAPI 3.0 e configuração de Swagger
  - Lambda + API Gateway pode exigir ajuste de base path (ex.: /prod, /dev)

## Subtasks
- [Subtask 01: Configurar Swashbuckle para gerar OpenAPI JSON](./subtask/Subtask-01-Configurar_Swashbuckle.md)
- [Subtask 02: Configurar Scalar UI para servir docs](./subtask/Subtask-02-Configurar_Scalar_UI.md)
- [Subtask 03: Adicionar XML comments aos controllers](./subtask/Subtask-03-Adicionar_XML_Comments.md)
- [Subtask 04: Documentar ajustes de base path para API Gateway](./subtask/Subtask-04-Ajustes_Base_Path_API_Gateway.md)
- [Subtask 05: Criar documentação de geração de client Kiota](./subtask/Subtask-05-Documentacao_Kiota.md)

## Critérios de Aceite da História
- [ ] GET /openapi/v1.json retorna especificação OpenAPI 3.0 completa e válida (testar com validador OpenAPI online ou CLI)
- [ ] GET /docs exibe interface Scalar UI funcional e navegável com todos os endpoints documentados (login, create user, health)
- [ ] XML comments presentes em todos os endpoints públicos (AuthController.Login, UserController.Create, HealthController.Health) com descrições claras, parâmetros documentados, exemplos de request/response e códigos HTTP (200, 201, 400, 401, 409, 422, 429)
- [ ] Documentação incluída sobre ajuste de base path para API Gateway (configuração de `servers` no OpenAPI se necessário)
- [ ] Documento `docs/kiota-client-generation.md` criado com comando completo de geração de client Kiota, instruções de instalação do Kiota CLI, e sugestão de onde salvar client (ex.: `clients/VideoProcessing.Clients.Auth/`)
- [ ] README.md atualizado com link para /docs e breve descrição da API
- [ ] `dotnet build` executa sem erros; aplicação roda localmente e /docs é acessível via navegador

## Rastreamento (dev tracking)
- **Início:** —
- **Fim:** —
- **Tempo total de desenvolvimento:** —
