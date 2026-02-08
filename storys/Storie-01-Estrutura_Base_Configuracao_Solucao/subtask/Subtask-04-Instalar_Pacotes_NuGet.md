# Subtask 04: Instalar Pacotes NuGet

## Descrição
Instalar todos os pacotes NuGet necessários nos projetos correspondentes (Api, Application, Infra, Tests.Unit) conforme lista definida no plano arquitetural, garantindo compatibilidade com .NET 10.

## Passos de Implementação
1. No projeto **Api**, instalar:
   - `Amazon.Lambda.AspNetCoreServer.Hosting` (2.0.0)
   - `AWSSDK.CognitoIdentityProvider` (3.7.400.16)
   - `Microsoft.AspNetCore.OpenApi` (10.0.0)
   - `Swashbuckle.AspNetCore` (7.2.0)
   - `Scalar.AspNetCore` (1.3.0)
   - `FluentValidation.AspNetCore` (11.3.0)
2. No projeto **Application**, instalar:
   - `FluentValidation` (11.11.0)
3. No projeto **Infra**, instalar:
   - `AWSSDK.CognitoIdentityProvider` (3.7.400.16)
   - `Microsoft.Extensions.Options` (10.0.0)
4. No projeto **Tests.Unit**, instalar:
   - `xUnit` (2.9.2)
   - `xUnit.runner.visualstudio` (2.8.2)
   - `Moq` (4.20.72)
   - `FluentAssertions` (7.0.0)
   - `coverlet.collector` (6.0.2)
5. Executar `dotnet restore` na raiz para garantir que todos os pacotes sejam baixados
6. Verificar arquivos `.csproj` de cada projeto para confirmar que PackageReferences foram adicionadas

## Formas de Teste
1. Executar `dotnet restore` na raiz e verificar que não há erros de resolução de pacotes
2. Executar `dotnet build` na raiz e confirmar compilação sem erros
3. Inspecionar cada arquivo `.csproj` e verificar presença de todas as PackageReferences
4. Executar `dotnet list package` em cada projeto para listar pacotes instalados e versões

## Critérios de Aceite da Subtask
- [ ] Todos os pacotes NuGet listados instalados nos projetos corretos
- [ ] Versões dos pacotes conforme especificado (ou versões compatíveis se as exatas não estiverem disponíveis)
- [ ] `dotnet restore` executa sem erros ou warnings de dependências
- [ ] `dotnet build` na raiz compila todos os projetos sem erros
- [ ] Arquivos `.csproj` contêm todas as PackageReferences esperadas
