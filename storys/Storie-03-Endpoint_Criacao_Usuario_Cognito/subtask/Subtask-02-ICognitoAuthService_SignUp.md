# Subtask 02: Adicionar Método SignUpAsync em ICognitoAuthService

## Descrição
Atualizar a interface `ICognitoAuthService` no projeto Application (Ports) adicionando o método `SignUpAsync` que define o contrato para criação de usuário no Cognito, incluindo parâmetro email.

## Passos de Implementação
1. Abrir arquivo `ICognitoAuthService.cs` em `Application/Ports/`
2. Adicionar novo método à interface:
   ```csharp
   Task<CreateUserOutput> SignUpAsync(string username, string password, string email, CancellationToken cancellationToken = default);
   ```
3. Adicionar XML comments documentando:
   - Propósito do método (criar novo usuário no Cognito User Pool via SignUp)
   - Parâmetros (username, password, email)
   - Retorno (CreateUserOutput com userId/UserSub, userConfirmed, confirmationRequired)
   - Exceções esperadas (ex.: ConflictException para UsernameExistsException, UnprocessableEntityException para InvalidPasswordException)
4. Nota: CreateUserOutput será criado na Subtask 04; por enquanto, usar tipo temporário ou criar stub vazio

## Formas de Teste
1. Compilar projeto Application e verificar que interface compila sem erros
2. Criar mock da interface em teste unitário temporário usando Moq e verificar que novo método pode ser mockado corretamente
3. Inspecionar código e confirmar assinatura do método (async Task, CancellationToken, 3 parâmetros string)
4. Verificar que interface está consistente (LoginAsync e SignUpAsync ambos presentes)

## Critérios de Aceite da Subtask
- [ ] Método `SignUpAsync` adicionado a `ICognitoAuthService` com assinatura correta: `Task<CreateUserOutput> SignUpAsync(string username, string password, string email, CancellationToken cancellationToken = default)`
- [ ] XML comments adicionados documentando propósito, parâmetros, retorno e exceções
- [ ] Interface pode ser mockada em testes; Moq.Mock<ICognitoAuthService> funciona corretamente com ambos métodos
- [ ] `dotnet build` executa sem erros no projeto Application
