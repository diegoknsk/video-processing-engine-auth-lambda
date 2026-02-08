# Subtask-04: Criar feature e steps para cenário de login com falha

## Descrição
Adicionar cenário de falha ao arquivo `.feature` e implementar steps definitions correspondentes que validam o comportamento esperado quando as credenciais são inválidas (senha incorreta, usuário não existe, etc.).

## Passos de Implementação
1. Adicionar cenário "Login com credenciais inválidas" ao arquivo `Features/Login.feature`
2. Escrever cenário em Gherkin:
   - **Given** um usuário com email válido mas senha incorreta
   - **When** o usuário faz requisição POST para /auth/login
   - **Then** a resposta deve ter status 401
   - **And** a resposta deve conter mensagem de erro apropriada
3. Adicionar steps à classe `LoginSteps.cs` (ou criar nova classe se preferir separar):
   - `[Given]`: configurar mock do serviço Cognito para retornar erro de autenticação (NotAuthorizedException, etc.)
   - `[When]`: fazer requisição HTTP POST com credenciais inválidas
   - `[Then]`: validar status code 401 usando FluentAssertions
   - `[And]`: validar presença de mensagem de erro no response body
4. Garantir que os mocks estão configurados corretamente para lançar/retornar erro esperado

## Formas de Teste
1. **Execução do teste BDD:** executar `dotnet test` e verificar que ambos os cenários (sucesso e falha) passam
2. **Relatório SpecFlow:** verificar no relatório que ambos os cenários estão documentados e passaram
3. **Validação de erro:** revisar os steps e confirmar que validam corretamente status 401 e mensagem de erro

## Critérios de Aceite
- [ ] Cenário de login com falha adicionado ao arquivo `Login.feature`
- [ ] Cenário de falha escrito em Gherkin com Given/When/Then/And
- [ ] Steps de falha implementados na classe `LoginSteps.cs`
- [ ] Mock do Cognito configurado para retornar erro de autenticação
- [ ] Validação de status 401 implementada com FluentAssertions
- [ ] Validação de mensagem de erro no response implementada
- [ ] Teste de falha executa com sucesso via `dotnet test`
- [ ] Ambos os cenários (sucesso e falha) passam juntos
