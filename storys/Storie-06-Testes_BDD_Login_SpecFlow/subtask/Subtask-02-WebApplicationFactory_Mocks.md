# Subtask-02: Implementar WebApplicationFactory customizado com mocks

## Descrição
Criar uma classe customizada que herda de `WebApplicationFactory<Program>` para configurar a aplicação de testes, substituindo as dependências reais do AWS Cognito por mocks do NSubstitute.

## Passos de Implementação
1. Criar classe `CustomWebApplicationFactory` em `Support/CustomWebApplicationFactory.cs` que herda de `WebApplicationFactory<Program>`
2. Sobrescrever método `ConfigureWebHost` para substituir serviços reais por mocks
3. Criar propriedades públicas para expor os mocks (ex.: `MockCognitoService`, `MockAuthService`, etc.) para configuração nos steps
4. Registrar mocks de interfaces/serviços relacionados ao Cognito no container de DI usando `services.AddScoped` ou `services.AddSingleton`
5. Garantir que a aplicação de teste use configurações apropriadas (appsettings.test.json ou in-memory)
6. Implementar `Dispose` se necessário para limpeza de recursos

## Formas de Teste
1. **Instanciação:** criar instância de `CustomWebApplicationFactory` e verificar que não lança exceções
2. **Client HTTP:** obter `HttpClient` via `CreateClient()` e fazer requisição simples ao health endpoint para validar que a app está rodando
3. **Mocks registrados:** verificar via reflexão ou teste que os mocks foram registrados corretamente no DI container

## Critérios de Aceite
- [ ] Classe `CustomWebApplicationFactory` criada e herda de `WebApplicationFactory<Program>`
- [ ] Método `ConfigureWebHost` sobrescrito e substitui dependências do Cognito por mocks
- [ ] Propriedades públicas expõem mocks para configuração nos steps
- [ ] Factory consegue criar `HttpClient` sem erros
- [ ] Mocks estão registrados corretamente no container de DI
- [ ] Aplicação de teste inicia e responde a requisições HTTP
