# Subtask 01: Configurar Swashbuckle para Gerar OpenAPI JSON

## Descrição
Configurar Swashbuckle.AspNetCore no `Program.cs` para gerar especificação OpenAPI 3.0, habilitar geração de arquivo XML de documentação no projeto Api, e incluir XML comments na configuração do Swagger.

## Passos de Implementação
1. No arquivo `.csproj` do projeto Api, adicionar dentro de `<PropertyGroup>`:
   ```xml
   <GenerateDocumentationFile>true</GenerateDocumentationFile>
   <NoWarn>$(NoWarn);1591</NoWarn>
   ```
   (1591 = warning de XML comments faltando; suprimimos inicialmente, mas ideal é documentar tudo)
2. No `Program.cs`, adicionar após `builder.Services.AddControllers()`:
   ```csharp
   builder.Services.AddEndpointsApiExplorer();
   builder.Services.AddSwaggerGen(options =>
   {
       options.SwaggerDoc("v1", new OpenApiInfo
       {
           Title = "Video Processing Auth API",
           Version = "v1",
           Description = "API de autenticação para Video Processing Engine usando Amazon Cognito",
           Contact = new OpenApiContact
           {
               Name = "Equipe Video Processing",
               Email = "team@videoprocessing.example.com"
           }
       });
       
       // Incluir XML comments
       var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
       var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
       if (File.Exists(xmlPath))
       {
           options.IncludeXmlComments(xmlPath);
       }
   });
   ```
3. Adicionar `using System.Reflection;` e `using Microsoft.OpenApi.Models;` no topo do Program.cs
4. No pipeline de middleware (após `app.Build()`), adicionar:
   ```csharp
   app.UseSwagger(); // Serve JSON em /swagger/v1/swagger.json
   ```
5. Executar `dotnet build` e verificar que arquivo XML é gerado em `bin/`
6. Executar aplicação e acessar `/swagger/v1/swagger.json` (ou `/openapi/v1.json` se configurado) e verificar JSON OpenAPI válido

## Formas de Teste
1. Executar `dotnet build` e verificar que `VideoProcessing.Auth.Api.xml` é gerado em `bin/Debug/net10.0/` (ou equivalente)
2. Executar `dotnet run` e acessar `http://localhost:5000/swagger/v1/swagger.json` (ajustar porta conforme configuração)
3. Copiar JSON e validar em validador online (ex.: https://editor.swagger.io) ou usar Swagger CLI
4. Verificar que JSON contém informações de título, versão, descrição e endpoints (mesmo sem XML comments ainda, deve listar rotas)

## Critérios de Aceite da Subtask
- [ ] Propriedades `GenerateDocumentationFile` e `NoWarn` adicionadas ao `.csproj` do projeto Api
- [ ] Swashbuckle configurado no `Program.cs` com `AddSwaggerGen` incluindo `OpenApiInfo` (título, versão, descrição, contato)
- [ ] XML comments incluídos na configuração via `IncludeXmlComments` (com verificação se arquivo existe)
- [ ] `UseSwagger()` adicionado ao pipeline de middleware
- [ ] Arquivo XML de documentação gerado no build (verificado em `bin/`)
- [ ] Endpoint `/swagger/v1/swagger.json` retorna JSON OpenAPI válido
- [ ] `dotnet build` e `dotnet run` executam sem erros
