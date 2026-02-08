# Subtask 02: Configurar Scalar UI para Servir Documentação

## Descrição
Configurar Scalar.AspNetCore no `Program.cs` para servir interface de documentação interativa em /docs, mapeando para o documento OpenAPI gerado pelo Swashbuckle, e personalizar tema/título conforme identidade do projeto.

## Passos de Implementação
1. No `Program.cs`, após `app.UseSwagger()`, adicionar:
   ```csharp
   app.MapScalarApiReference(options =>
   {
       options.Title = "Video Processing Auth API";
       options.Theme = ScalarTheme.Default; // ou ScalarTheme.Purple, ScalarTheme.Alternate, etc.
       // Opcional: configurar outras opções (DarkMode, ShowModels, etc.)
   });
   ```
2. Verificar que pacote `Scalar.AspNetCore` está instalado (já feito na Story 01)
3. Adicionar `using Scalar.AspNetCore;` no topo do Program.cs
4. Por padrão, Scalar UI será servido em `/scalar/v1` ou `/docs` (depende da versão do pacote; verificar documentação)
5. Se necessário, customizar endpoint explicitamente:
   ```csharp
   app.MapScalarApiReference(options => { ... }).RequireHost("*"); // ou configurar rota customizada
   ```
6. Executar aplicação e acessar /docs no navegador

## Formas de Teste
1. Executar `dotnet run` e acessar `http://localhost:5000/docs` (ajustar porta conforme configuração) no navegador
2. Verificar que interface Scalar UI carrega corretamente com tema configurado
3. Verificar que todos os endpoints aparecem na UI (POST /auth/login, POST /auth/users/create, GET /health)
4. Testar chamada de exemplo diretamente pela UI (Scalar permite executar requests); verificar que funciona
5. Testar em diferentes navegadores (Chrome, Firefox) para garantir compatibilidade

## Critérios de Aceite da Subtask
- [ ] Scalar UI configurado no `Program.cs` com `MapScalarApiReference` e opções personalizadas (título, tema)
- [ ] Endpoint /docs serve interface Scalar UI funcional e navegável
- [ ] Todos os endpoints da API aparecem na documentação (login, create user, health)
- [ ] Interface é visualmente limpa e profissional (tema configurado)
- [ ] Testes manuais de chamada via UI funcionam (opcional, mas recomendado para validar integração)
- [ ] `dotnet build` e `dotnet run` executam sem erros; /docs acessível via navegador
