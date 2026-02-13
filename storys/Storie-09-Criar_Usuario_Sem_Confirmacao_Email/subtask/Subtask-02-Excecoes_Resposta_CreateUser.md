# Subtask 02: Tratamento de exceções e compatibilidade de resposta

## Descrição
Garantir que as exceções lançadas pelas APIs Admin (AdminCreateUser, AdminSetUserPassword) sejam mapeadas de forma que o controller e o middleware continuem retornando os mesmos códigos HTTP (409, 422, 400) e que a resposta de sucesso permaneça compatível com o contrato atual.

## Passos de implementação
1. Verificar na documentação AWS SDK .NET quais exceções são lançadas por `AdminCreateUser` (ex.: `UsernameExistsException`, `InvalidPasswordException`, `InvalidParameterException`) e por `AdminSetUserPassword` (ex.: `InvalidPasswordException`, `InvalidParameterException`).
2. Manter os blocos catch existentes no `SignUpAsync` que traduzem para `UsernameExistsException`, `InvalidPasswordException`, `InvalidParameterException` e relançam ou envolvem em `ArgumentException` quando fizer sentido; ajustar para capturar as exceções vindas de AdminCreateUser/AdminSetUserPassword.
3. Garantir que em caso de sucesso o `CreateUserOutput` preenche `UserId` (Sub do usuário criado, obtido do resultado de AdminCreateUser), `Username`, `UserConfirmed = true`, `ConfirmationRequired = false`.
4. Se AdminSetUserPassword falhar após AdminCreateUser ter sucesso, decidir se faz retry, rollback (ex.: AdminDeleteUser) ou apenas log e relançar; para esta story, relançar a exceção é aceitável (o usuário ficaria em estado FORCE_CHANGE_PASSWORD até correção manual ou nova story).

## Formas de teste
- Teste unitário: simular `UsernameExistsException` no AdminCreateUser e verificar que a exceção é propagada (controller retorna 409).
- Teste unitário: simular `InvalidPasswordException` e verificar propagação (422).
- Teste manual: enviar email duplicado no create e verificar resposta 409.

## Critérios de aceite da subtask
- [x] Exceções de AdminCreateUser/AdminSetUserPassword tratadas de forma que 409 (usuário já existe), 422 (senha inválida) e 400 (parâmetro inválido) continuem sendo retornados pela API.
- [x] Resposta 201 de create retorna `userConfirmed: true` e `confirmationRequired: false`.
- [x] Nenhuma alteração desnecessária em contratos da API (request/response) além do que for exigido pela mudança de fluxo.
