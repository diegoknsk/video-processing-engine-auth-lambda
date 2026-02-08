# Subtask 02: Ajustar Port, Service e UseCase de criação de usuário

## Descrição
Alterar a assinatura de `SignUpAsync` na porta `ICognitoAuthService` e na implementação `CognitoAuthService` para receber apenas `username` e `password`. No SignUp do Cognito, enviar `Username = username` e no `UserAttributes` incluir o atributo `email` com o mesmo valor (username). Ajustar o `CreateUserUseCase` para não passar email.

## Passos de implementação
1. Em `ICognitoAuthService.cs`: alterar `SignUpAsync(string username, string password, string email, ...)` para `SignUpAsync(string username, string password, ...)`; atualizar o comentário XML removendo o parâmetro `email`.
2. Em `CognitoAuthService.cs`: alterar a assinatura do método para receber apenas `username` e `password`; no `SignUpRequest`, manter `Username = username`; em `UserAttributes`, usar `new AttributeType { Name = "email", Value = username }` (o username já é o email). Ajustar log para não mencionar "with email".
3. Em `CreateUserUseCase.cs`: chamar `_cognitoAuthService.SignUpAsync(input.Username, input.Password, cancellationToken)`; ajustar log para não incluir `input.Email`.

## Formas de teste
- Executar testes unitários do `CognitoAuthServiceSignUpTests` e `CreateUserUseCaseTests` após ajustes da Subtask 04.
- Build da solução sem erros; nenhuma referência a `email` como parâmetro de SignUp na Infra e Application.

## Critérios de aceite da subtask
- [ ] `ICognitoAuthService.SignUpAsync` tem assinatura com apenas username, password e CancellationToken.
- [ ] `CognitoAuthService.SignUpAsync` envia ao Cognito `Username = username` e atributo `email` = username nos UserAttributes.
- [ ] `CreateUserUseCase` chama SignUpAsync apenas com input.Username e input.Password; logs atualizados.
