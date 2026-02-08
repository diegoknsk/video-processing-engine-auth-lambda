# Subtask 02: Configurar Program.cs com Lambda Hosting

## Descrição
Configurar o arquivo `Program.cs` do projeto Api para usar `Amazon.Lambda.AspNetCoreServer.Hosting`, registrar serviços base (Controllers, JSON options, CORS, logging estruturado) e configurar middleware.

## Passos de Implementação
1. Adicionar pacote `Amazon.Lambda.AspNetCoreServer.Hosting` ao projeto Api
2. No `Program.cs`, configurar `builder.Services.AddAWSLambdaHosting(LambdaEventSource.HttpApi)`
3. Registrar Controllers com `AddControllers()` e configurar JSON options:
   - `PropertyNamingPolicy = JsonNamingPolicy.CamelCase`
   - `PropertyNameCaseInsensitive = true`
   - `DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull`
4. Configurar CORS com política permissiva (dev) usando `AddCors()`
5. Configurar logging estruturado no `appsettings.json` (seção Logging com níveis Information/Warning)
6. Adicionar middleware: `UseHttpsRedirection()`, `UseCors()`, `UseAuthorization()`, `MapControllers()`
7. Testar que o Program.cs compila sem erros

## Formas de Teste
1. Executar `dotnet build` no projeto Api e verificar compilação sem erros
2. Executar `dotnet run` localmente (mesmo sem endpoints ainda) e verificar que a aplicação inicia sem exceções
3. Verificar logs de inicialização no console (Application started, Hosting started, etc.)
4. Inspecionar código e confirmar que `AddAWSLambdaHosting()` está presente antes de `builder.Build()`

## Critérios de Aceite da Subtask
- [ ] `Program.cs` configurado com `AddAWSLambdaHosting(LambdaEventSource.HttpApi)`
- [ ] Controllers registrados com `AddControllers()` e JSON options configuradas (camelCase, ignore null)
- [ ] CORS configurado com política adequada para desenvolvimento
- [ ] Logging estruturado configurado em `appsettings.json` (níveis Default: Information, Microsoft.AspNetCore: Warning)
- [ ] Middleware pipeline configurado (HTTPS redirect, CORS, Authorization, MapControllers)
- [ ] `dotnet build` e `dotnet run` executam sem erros; aplicação inicia com sucesso
