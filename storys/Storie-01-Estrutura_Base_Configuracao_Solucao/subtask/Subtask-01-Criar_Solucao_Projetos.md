# Subtask 01: Criar Solução e Projetos

## Descrição
Criar a solução .NET (`VideoProcessing.Auth.sln`) e os 4 projetos base (Api, Application, Infra, Tests.Unit) com as referências corretas entre eles, adicionando todos à solução.

## Passos de Implementação
1. Criar arquivo de solução `VideoProcessing.Auth.sln` na raiz do repositório usando `dotnet new sln`
2. Criar projetos:
   - `src/VideoProcessing.Auth.Api` (web/API com `dotnet new web`)
   - `src/VideoProcessing.Auth.Application` (classlib com `dotnet new classlib`)
   - `src/VideoProcessing.Auth.Infra` (classlib com `dotnet new classlib`)
   - `tests/VideoProcessing.Auth.Tests.Unit` (xUnit com `dotnet new xunit`)
3. Adicionar todos os projetos à solução usando `dotnet sln add`
4. Configurar referências entre projetos:
   - Api referencia Application
   - Application não referencia nada externo (exceto FluentValidation)
   - Infra referencia Application
   - Tests.Unit referencia todos (Api, Application, Infra)
5. Verificar build com `dotnet build` na raiz
6. Remover arquivos de exemplo gerados automaticamente (Class1.cs, WeatherForecast.cs, etc.)

## Formas de Teste
1. Executar `dotnet build` na raiz e verificar que compila sem erros
2. Executar `dotnet sln list` e confirmar que todos os 4 projetos estão listados
3. Abrir a solução no Visual Studio/Rider e verificar que a estrutura aparece corretamente no Solution Explorer
4. Executar `dotnet test` e verificar que o projeto de testes é descoberto (mesmo sem testes ainda)

## Critérios de Aceite da Subtask
- [ ] Arquivo `VideoProcessing.Auth.sln` criado na raiz do repositório
- [ ] 4 projetos criados na estrutura de pastas correta (`src/` e `tests/`)
- [ ] Todos os projetos adicionados à solução; `dotnet sln list` mostra os 4 projetos
- [ ] Referências entre projetos configuradas corretamente (Api → Application, Infra → Application, Tests.Unit → todos)
- [ ] `dotnet build` executa sem erros ou warnings
- [ ] Arquivos de exemplo removidos; projetos contêm apenas estrutura mínima
