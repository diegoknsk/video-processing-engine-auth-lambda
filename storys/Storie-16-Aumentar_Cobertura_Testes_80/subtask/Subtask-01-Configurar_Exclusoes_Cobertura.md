# Subtask-01: Configurar Exclusões de Cobertura

## Descrição
Configurar o coverlet para excluir do cálculo de cobertura os arquivos `Program.cs` e `AssemblyReference.cs`, que somam 93 linhas sem cobertura e não são testáveis de forma unitária. Isso elimina o ruído no relatório e aproxima imediatamente a cobertura da meta de 80%.

## Passos de Implementação
1. Adicionar o atributo `[ExcludeFromCodeCoverage]` na classe/método de `Program.cs` (ou no top-level program), usando `System.Diagnostics.CodeAnalysis`.
2. Adicionar o atributo `[ExcludeFromCodeCoverage]` na classe `AssemblyReference` em `AssemblyReference.cs`.
3. Verificar se o `.csproj` do projeto de testes (`VideoProcessing.Auth.Tests.Unit.csproj`) já possui o nó `<CoverletOutputFormat>` e, se necessário, adicionar `<ExcludeByFile>` para padrões como `**/Program.cs;**/AssemblyReference.cs` como alternativa ou reforço.
4. Executar `dotnet test --collect:"XPlat Code Coverage"` e confirmar que os arquivos excluídos não aparecem mais no relatório de cobertura.

## Formas de Teste
- Executar `dotnet test` e verificar que nenhum teste quebra após a adição dos atributos.
- Verificar no relatório `coverage.opencover.xml` que `Program.cs` e `AssemblyReference.cs` não figuram mais com linhas não cobertas.
- Confirmar via SonarCloud ou `reportgenerator` que a cobertura total sobe após a exclusão.

## Critérios de Aceite
- `[ExcludeFromCodeCoverage]` adicionado em `Program.cs` e `AssemblyReference.cs` (ou exclusão equivalente via `.csproj`).
- `dotnet test` executa com sucesso (0 falhas).
- Arquivos excluídos não constam mais no relatório de cobertura como linhas descobertas.
