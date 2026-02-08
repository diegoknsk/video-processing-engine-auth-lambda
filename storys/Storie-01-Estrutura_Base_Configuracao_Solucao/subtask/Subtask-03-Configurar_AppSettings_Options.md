# Subtask 03: Configurar appsettings e Options Pattern

## Descrição
Criar configuração no `appsettings.json` para seção Cognito (com placeholders vazios), implementar classe `CognitoOptions` no projeto Infra usando Options pattern, e registrar no DI do `Program.cs`.

## Passos de Implementação
1. Criar/editar `appsettings.json` no projeto Api com seção Cognito:
   ```json
   {
     "Cognito": {
       "Region": "",
       "AppClientId": "",
       "AppClientSecret": "",
       "UserPoolId": ""
     },
     "Logging": { ... },
     "AllowedHosts": "*"
   }
   ```
2. Criar classe `CognitoOptions` no projeto Infra (`Models/CognitoOptions.cs`) com propriedades: Region, AppClientId, AppClientSecret, UserPoolId
3. No `Program.cs`, registrar options usando `builder.Services.Configure<CognitoOptions>(builder.Configuration.GetSection("Cognito"))`
4. Adicionar validação de options (opcional, mas recomendado): usar `ValidateDataAnnotations()` ou FluentValidation para options
5. Criar arquivo `appsettings.Development.json` (se necessário) para sobrescrever valores em dev

## Formas de Teste
1. Executar `dotnet build` e verificar compilação sem erros
2. Adicionar teste unitário que injeta `IOptions<CognitoOptions>` e verifica que a configuração é lida corretamente (mock de IConfiguration)
3. No `Program.cs`, adicionar log temporário para confirmar que options foram carregadas: `var opts = app.Services.GetRequiredService<IOptions<CognitoOptions>>(); logger.LogInformation("Cognito Region: {Region}", opts.Value.Region);`
4. Executar aplicação localmente e verificar log (mesmo com valores vazios, deve aparecer no log)

## Critérios de Aceite da Subtask
- [ ] `appsettings.json` criado com seção Cognito contendo placeholders vazios para Region, AppClientId, AppClientSecret, UserPoolId
- [ ] Classe `CognitoOptions` criada no projeto Infra com propriedades correspondentes ao JSON
- [ ] `IOptions<CognitoOptions>` registrado no DI via `Configure<CognitoOptions>()` no `Program.cs`
- [ ] Documentação inline (comentário) indicando que valores reais vêm de variáveis de ambiente
- [ ] Build sem erros; aplicação consegue resolver `IOptions<CognitoOptions>` via DI
