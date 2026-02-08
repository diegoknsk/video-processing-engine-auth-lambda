# Storie-06: Testes BDD de Login com SpecFlow

## Status
- **Estado:** 🔄 Em desenvolvimento
- **Data de Conclusão:** [DD/MM/AAAA]

## Descrição
Como desenvolvedor do sistema, quero implementar testes BDD para o endpoint de login utilizando SpecFlow, para garantir que os cenários de sucesso e falha funcionem corretamente através de testes de integração mockados.

## Objetivo
Criar projeto de testes BDD com SpecFlow que valide os cenários principais do endpoint de login: autenticação bem-sucedida e falha na autenticação. Os testes devem mockar as dependências externas (AWS Cognito) e validar o comportamento da API de forma legível e baseada em especificações.

## Escopo Técnico
- Tecnologias: .NET 8, SpecFlow, xUnit, NSubstitute
- Arquivos afetados: Novo projeto `tests/VideoProcessing.Auth.Tests.Bdd/`
- Componentes: 
  - Projeto de testes BDD
  - Features do SpecFlow (login.feature)
  - Steps definitions
  - Configuração de teste com WebApplicationFactory
  - Mocks de serviços do Cognito
- Pacotes/Dependências:
  - SpecFlow (3.9.74)
  - SpecFlow.xUnit (3.9.74)
  - SpecFlow.Tools.MsBuild.Generation (3.9.74)
  - Microsoft.AspNetCore.Mvc.Testing (8.0.0)
  - NSubstitute (5.1.0)
  - FluentAssertions (6.12.0)
  - xunit (2.6.6)
  - xunit.runner.visualstudio (2.5.6)

## Dependências e Riscos (para estimativa)
- Dependências: Storie-02 (Endpoint de Login) deve estar implementada
- Riscos: Necessidade de configurar corretamente o WebApplicationFactory para substituir dependências reais por mocks
- Pré-condições: Endpoint de login funcional; compreensão da estrutura de DI da API

## Subtasks
- [Subtask 01: Criar projeto de testes BDD e configurar SpecFlow](./subtask/Subtask-01-Criar_Projeto_Configurar_SpecFlow.md)
- [Subtask 02: Implementar WebApplicationFactory customizado com mocks](./subtask/Subtask-02-WebApplicationFactory_Mocks.md)
- [Subtask 03: Criar feature e steps para cenário de login bem-sucedido](./subtask/Subtask-03-Feature_Steps_Login_Sucesso.md)
- [Subtask 04: Criar feature e steps para cenário de login com falha](./subtask/Subtask-04-Feature_Steps_Login_Falha.md)
- [Subtask 05: Executar e validar testes BDD](./subtask/Subtask-05-Executar_Validar_Testes.md)

## Critérios de Aceite da História
- [ ] Projeto `VideoProcessing.Auth.Tests.Bdd` criado em `tests/` com SpecFlow configurado
- [ ] Feature file `Login.feature` contém cenários de sucesso e falha escritos em Gherkin
- [ ] Steps definitions implementados e associados aos cenários da feature
- [ ] WebApplicationFactory customizado substitui dependências do Cognito por mocks (NSubstitute)
- [ ] Teste de login bem-sucedido valida status 200 e presença de token JWT no response
- [ ] Teste de login com falha valida status 401 e mensagem de erro apropriada
- [ ] Todos os testes BDD executam com sucesso via `dotnet test`
- [ ] Mocks configurados corretamente retornam respostas esperadas para cada cenário
- [ ] Relatório de testes SpecFlow gerado e legível

## Rastreamento (dev tracking)
- **Início:** —
- **Fim:** —
- **Tempo total de desenvolvimento:** —
