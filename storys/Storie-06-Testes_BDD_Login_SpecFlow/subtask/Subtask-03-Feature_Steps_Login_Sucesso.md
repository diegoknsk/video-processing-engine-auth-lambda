# Subtask-03: Criar feature e steps para cenário de login bem-sucedido

## Descrição
Implementar o arquivo `.feature` com cenário de login bem-sucedido em Gherkin e criar a classe de steps definitions correspondente que valida o comportamento esperado quando as credenciais são válidas.

## Passos de Implementação
1. Criar arquivo `Features/Login.feature` com cenário "Login com credenciais válidas"
2. Escrever cenário em Gherkin:
   - **Given** um usuário com email e senha válidos
   - **When** o usuário faz requisição POST para /auth/login
   - **Then** a resposta deve ter status 200
   - **And** a resposta deve conter um token JWT válido
3. Criar classe `LoginSteps.cs` em `Steps/` com atributo `[Binding]`
4. Implementar steps:
   - `[Given]`: configurar mock do serviço Cognito para retornar sucesso na autenticação
   - `[When]`: fazer requisição HTTP POST usando `HttpClient` da factory
   - `[Then]`: validar status code 200 usando FluentAssertions
   - `[And]`: validar presença e formato do token JWT no response body
5. Usar `CustomWebApplicationFactory` para obter `HttpClient` e configurar mocks no construtor ou método de setup

## Formas de Teste
1. **Execução do teste BDD:** executar `dotnet test` e verificar que o cenário de sucesso passa
2. **Relatório SpecFlow:** verificar no relatório de testes que o cenário está documentado e passou
3. **Validação manual:** revisar os steps e confirmar que validam corretamente status, token e estrutura da resposta

## Critérios de Aceite
- [ ] Arquivo `Login.feature` criado em `Features/` com cenário de login bem-sucedido
- [ ] Cenário escrito em Gherkin com Given/When/Then/And
- [ ] Classe `LoginSteps.cs` criada em `Steps/` com atributo `[Binding]`
- [ ] Steps implementados e fazem requisição HTTP usando WebApplicationFactory
- [ ] Mock do Cognito configurado para retornar sucesso na autenticação
- [ ] Validação de status 200 implementada com FluentAssertions
- [ ] Validação de presença de token JWT no response implementada
- [ ] Teste executa com sucesso via `dotnet test`
