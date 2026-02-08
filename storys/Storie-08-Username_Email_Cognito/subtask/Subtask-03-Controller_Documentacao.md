# Subtask 03: Atualizar documentação do Controller e contratos

## Descrição
Atualizar a documentação XML do endpoint de criação de usuário no `UserController` para refletir que o campo `username` deve ser o email do usuário (formato válido de email), e que não existe mais o campo `email` no body da requisição.

## Passos de implementação
1. Em `UserController.cs` (método Create): no `<param name="input">`, descrever que os dados contêm **username** (deve ser um email válido), **password** (mínimo 8 caracteres, política Cognito); remover menção a "email" como campo separado.
2. Ajustar `<returns>`, `<response code="400">` e `<response code="409">` para não citar "email" como campo de entrada; em 409, manter "Username ou email já cadastrado" apenas se fizer sentido (username é o email, então "email já cadastrado" permanece coerente).
3. Revisar a descrição adicional (ex.: "O email será registrado...") para indicar que o username é o email usado no Cognito para sign-in e confirmação.

## Formas de teste
- Verificar na documentação Swagger/Scalar que o schema do request de criação de usuário não exibe mais o campo `email` e que a descrição do `username` menciona que deve ser um email.
- Build e execução da API; checagem visual da documentação OpenAPI.

## Critérios de aceite da subtask
- [ ] Documentação XML do POST de criação de usuário descreve username como email (formato válido) e não referencia campo email no body.
- [ ] Contrato da API (OpenAPI) reflete apenas username e password para o endpoint de create user.
