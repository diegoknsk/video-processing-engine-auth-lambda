# Storie-01: Estrutura Base e Configuração da Solução

## Status
- **Estado:** 🔄 Em desenvolvimento
- **Data de Conclusão:** —

## Descrição
Como desenvolvedor do sistema, quero criar a estrutura base de projetos e configuração da solução do Auth Lambda, para ter uma fundação sólida que suporte o desenvolvimento dos endpoints de autenticação.

## Objetivo
Criar a estrutura completa de projetos (.Api, .Application, .Infra, .Tests.Unit) com configuração de DI, appsettings, pacotes NuGet base, bootstrap do Lambda com Amazon.Lambda.AspNetCoreServer.Hosting, e estrutura de pastas alinhada à Clean Architecture.

## Escopo Técnico
- Tecnologias: .NET 10, ASP.NET Core, AWS Lambda, Amazon Cognito
- Arquivos afetados:
  - `VideoProcessing.Auth.sln` (novo)
  - `src/VideoProcessing.Auth.Api/Program.cs` (novo)
  - `src/VideoProcessing.Auth.Api/appsettings.json` (novo)
  - `src/VideoProcessing.Auth.Api/*.csproj` (novo)
  - `src/VideoProcessing.Auth.Application/*.csproj` (novo)
  - `src/VideoProcessing.Auth.Infra/*.csproj` (novo)
  - `tests/VideoProcessing.Auth.Tests.Unit/*.csproj` (novo)
- Componentes/Recursos: Solução .NET, projetos, DI container, Options pattern, FluentValidation
- Pacotes/Dependências:
  - Amazon.Lambda.AspNetCoreServer.Hosting (2.0.0)
  - AWSSDK.CognitoIdentityProvider (3.7.400.16)
  - Microsoft.AspNetCore.OpenApi (10.0.0)
  - Swashbuckle.AspNetCore (7.2.0)
  - Scalar.AspNetCore (1.3.0)
  - FluentValidation.AspNetCore (11.3.0)
  - FluentValidation (11.11.0)
  - Microsoft.Extensions.Options (10.0.0)
  - xUnit (2.9.2)
  - xUnit.runner.visualstudio (2.8.2)
  - Moq (4.20.72)
  - FluentAssertions (7.0.0)
  - coverlet.collector (6.0.2)

## Dependências e Riscos (para estimativa)
- Dependências: Nenhuma; esta é a primeira story do projeto.
- Riscos/Pré-condições: 
  - .NET 10 SDK instalado
  - Pacotes NuGet devem estar disponíveis nas versões especificadas (ajustar se necessário)
  - Familiaridade com Amazon.Lambda.AspNetCoreServer.Hosting

## Subtasks
- [Subtask 01: Criar solução e projetos](./subtask/Subtask-01-Criar_Solucao_Projetos.md)
- [Subtask 02: Configurar Program.cs com Lambda Hosting](./subtask/Subtask-02-Configurar_Program_Lambda.md)
- [Subtask 03: Configurar appsettings e Options pattern](./subtask/Subtask-03-Configurar_AppSettings_Options.md)
- [Subtask 04: Instalar pacotes NuGet](./subtask/Subtask-04-Instalar_Pacotes_NuGet.md)
- [Subtask 05: Criar estrutura de pastas](./subtask/Subtask-05-Criar_Estrutura_Pastas.md)
- [Subtask 06: Configurar FluentValidation](./subtask/Subtask-06-Configurar_FluentValidation.md)

## Critérios de Aceite da História
- [ ] Solução com 4 projetos criados (Api, Application, Infra, Tests.Unit) e adicionados ao .sln; `dotnet build` executa sem erros
- [ ] Program.cs configurado com `AddAWSLambdaHosting()` e middleware básico (CORS, logging estruturado)
- [ ] appsettings.json com seção Cognito (placeholders vazios); CognitoOptions com IOptions<T> registrado no DI
- [ ] Todos os pacotes NuGet instalados nas versões recomendadas; `dotnet restore` funciona sem erros
- [ ] Estrutura de pastas criada conforme plano arquitetural (Controllers/Auth, UseCases/Auth, Validators/Auth, Presenters/Auth, Ports, Services)
- [ ] FluentValidation registrado com `AddFluentValidationAutoValidation()` e assembly scanning; filtro global de validação aplicado
- [ ] Testes unitários iniciais (smoke test) passando; `dotnet test` executa com sucesso; cobertura mínima configurada

## Rastreamento (dev tracking)
- **Início:** —
- **Fim:** —
- **Tempo total de desenvolvimento:** —
