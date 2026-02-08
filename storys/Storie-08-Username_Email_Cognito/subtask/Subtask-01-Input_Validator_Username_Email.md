# Subtask 01: Remover Email do Input e validar Username como email

## Descrição
Remover a propriedade `Email` do modelo de entrada `CreateUserInput` e ajustar o `CreateUserInputValidator` para validar que `Username` seja um email válido (obrigatório, formato email, tamanho máximo), removendo todas as regras referentes ao campo Email.

## Passos de implementação
1. Em `CreateUserInput.cs`: remover a propriedade `Email` e o comentário XML associado; manter apenas `Username` e `Password`.
2. Em `CreateUserInputValidator.cs`: remover o bloco `RuleFor(x => x.Email)` (NotEmpty, EmailAddress, MaximumLength).
3. No mesmo validator: alterar as regras de `Username` — manter NotEmpty e MaximumLength(128 ou 256); remover `.Matches("^[a-zA-Z0-9_-]+$")` e adicionar `.EmailAddress()` com mensagem adequada (ex.: "Username deve ser um email válido.").
4. Garantir que a ordem das regras (Username depois Password) e as mensagens de erro estejam claras.

## Formas de teste
- Executar testes unitários do `CreateUserInputValidatorTests` após ajustar os testes (Subtask 04).
- Validar manualmente que um request com `username: "user@example.com"` e `password` válida passa; que `username: "notanemail"` retorna erro de validação.
- Verificar que nenhum código referencia mais `CreateUserInput.Email` na camada Application (Input/Validator).

## Critérios de aceite da subtask
- [ ] `CreateUserInput` possui apenas `Username` e `Password`.
- [ ] `CreateUserInputValidator` valida `Username` como email (obrigatório, formato email, tamanho máximo); não há regras para `Email`.
- [ ] Build da solução sem erros de compilação na camada Application.
