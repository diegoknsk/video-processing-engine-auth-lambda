# Subtask 04: Documentar Ajustes de Base Path para API Gateway

## Descrição
Documentar (em comentários no código ou arquivo de doc separado) como ajustar o base path da especificação OpenAPI quando a API for deployada via Lambda + API Gateway, incluindo configuração de `servers` no SwaggerGen para refletir stages (ex.: /prod, /dev).

## Passos de Implementação
1. Criar arquivo `docs/api-gateway-configuration.md` (ou seção no README.md) explicando:
   - Quando Lambda é exposto via API Gateway, o base path pode incluir stage (ex.: `https://api.example.com/prod` ou `https://api.example.com/dev`)
   - Impacto: especificação OpenAPI precisa refletir o base path correto para que clientes gerados (ex.: Kiota) usem a URL correta
2. No `Program.cs`, adicionar comentário e código de exemplo (comentado ou condicional) para configurar `servers` no SwaggerGen:
   ```csharp
   options.AddServer(new OpenApiServer 
   { 
       Url = "https://api.example.com/prod",
       Description = "Production (API Gateway stage: prod)"
   });
   options.AddServer(new OpenApiServer 
   { 
       Url = "https://api.example.com/dev",
       Description = "Development (API Gateway stage: dev)"
   });
   ```
3. Explicar que isso pode ser configurado via variável de ambiente (ex.: `API_BASE_URL`) para evitar hard-coding:
   ```csharp
   var baseUrl = builder.Configuration["API_BASE_URL"] ?? "http://localhost:5000";
   options.AddServer(new OpenApiServer { Url = baseUrl });
   ```
4. Documentar que, se não configurado, clientes podem precisar ajustar base URL manualmente
5. Testar localmente (sem API Gateway) que OpenAPI JSON contém `servers` correto (ou omitir se rodar localmente)

## Formas de Teste
1. Revisar documentação criada e verificar clareza e completude
2. Adicionar configuração de exemplo no `Program.cs` (comentada) e verificar que compila
3. Executar aplicação localmente e verificar que, se `servers` estiver configurado, aparece no JSON OpenAPI
4. Exportar JSON OpenAPI e verificar campo `servers` (se configurado):
   ```json
   "servers": [
     { "url": "https://api.example.com/prod", "description": "Production" }
   ]
   ```
5. (Opcional) Simular deploy em API Gateway (ou usar ferramenta de teste local como LocalStack) e verificar comportamento

## Critérios de Aceite da Subtask
- [ ] Arquivo `docs/api-gateway-configuration.md` criado (ou seção no README.md) documentando ajustes de base path
- [ ] Explicação clara sobre impacto de API Gateway stages no base path
- [ ] Código de exemplo adicionado no `Program.cs` (comentado ou condicional) para configurar `servers` via variável de ambiente
- [ ] Documentação menciona alternativa de ajuste manual em clientes se não configurado
- [ ] `dotnet build` executa sem erros
- [ ] JSON OpenAPI reflete configuração de `servers` se habilitada (testado localmente)
