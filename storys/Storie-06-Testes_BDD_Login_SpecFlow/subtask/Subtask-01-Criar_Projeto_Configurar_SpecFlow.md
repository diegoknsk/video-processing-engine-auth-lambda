# Subtask-01: Criar projeto de testes BDD e configurar SpecFlow

## Descrição
Criar o projeto de testes BDD em `tests/VideoProcessing.Auth.Tests.Bdd/` e instalar/configurar todas as dependências necessárias do SpecFlow, xUnit e ferramentas de mock.

## Passos de Implementação
1. Criar projeto xUnit em `tests/VideoProcessing.Auth.Tests.Bdd/` usando `dotnet new xunit`
2. Adicionar referência ao projeto `VideoProcessing.Auth.Api`
3. Instalar pacotes NuGet:
   - SpecFlow (3.9.74)
   - SpecFlow.xUnit (3.9.74)
   - SpecFlow.Tools.MsBuild.Generation (3.9.74)
   - Microsoft.AspNetCore.Mvc.Testing (8.0.0)
   - NSubstitute (5.1.0)
   - FluentAssertions (6.12.0)
4. Criar arquivo `specflow.json` na raiz do projeto de testes com configurações apropriadas (framework xUnit, binding culture pt-BR ou en-US)
5. Adicionar o projeto à solution principal se necessário
6. Criar estrutura de pastas: `Features/`, `Steps/`, `Support/`

## Formas de Teste
1. **Build do projeto:** executar `dotnet build` no projeto de testes e verificar que compila sem erros
2. **Verificação de pacotes:** confirmar que todos os pacotes foram restaurados corretamente via `dotnet list package`
3. **Teste inicial:** criar feature file simples e executar `dotnet test` para validar que SpecFlow está configurado

## Critérios de Aceite
- [ ] Projeto `VideoProcessing.Auth.Tests.Bdd` criado em `tests/` e presente na solution
- [ ] Todos os pacotes NuGet instalados com versões corretas
- [ ] Arquivo `specflow.json` configurado corretamente
- [ ] Estrutura de pastas criada: `Features/`, `Steps/`, `Support/`
- [ ] `dotnet build` executa sem erros no projeto de testes
- [ ] Referência ao projeto `VideoProcessing.Auth.Api` adicionada e funcionando
