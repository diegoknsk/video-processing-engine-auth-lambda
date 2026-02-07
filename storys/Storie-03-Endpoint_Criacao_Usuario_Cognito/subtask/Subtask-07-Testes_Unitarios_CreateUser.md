# Subtask 07: Testes Unitários Completos de CreateUser

## Descrição
Criar suite completa de testes unitários para todos os componentes do fluxo de criação de usuário (Validator, Presenter, UseCase, Service) garantindo cobertura ≥ 80% e validando todos os cenários de sucesso e erro.

## Passos de Implementação
1. **CreateUserInputValidatorTests:**
   - Testar username vazio → erro "Username é obrigatório"
   - Testar username com caracteres inválidos (ex.: "user@123") → erro "apenas letras, números, _ e -"
   - Testar username válido (ex.: "user_test-123") → sem erro
   - Testar password vazio → erro "Password é obrigatório"
   - Testar password com 7 caracteres → erro "pelo menos 8 caracteres"
   - Testar password com 257 caracteres → erro "no máximo 256 caracteres"
   - Testar email vazio → erro "Email é obrigatório"
   - Testar email inválido (ex.: "notanemail") → erro "formato inválido"
   - Testar email válido (ex.: "user@example.com") → sem erro
   - Testar input completamente válido → sem erros
2. **CreateUserPresenterTests:**
   - Testar mapeamento completo de CreateUserOutput → CreateUserResponseModel
   - Testar cenário com UserConfirmed = true (ConfirmationRequired = false)
   - Testar cenário com UserConfirmed = false (ConfirmationRequired = true)
3. **CreateUserUseCaseTests:**
   - Cenário sucesso: mockar service retornando CreateUserOutput válido; verificar que UseCase retorna CreateUserResponseModel correto
   - Cenário erro de conflito (usuário já existe): mockar service lançando exceção apropriada; verificar que exceção propaga
   - Cenário erro de senha inválida: mockar service lançando exceção apropriada; verificar que exceção propaga
   - Verificar que presenter é chamado corretamente
   - Verificar que logs são emitidos (mock de ILogger)
4. **CognitoAuthServiceSignUpTests:**
   - Mockar IAmazonCognitoIdentityProvider (interface wrapper ou técnica de mock)
   - Cenário sucesso: mock retorna SignUpResponse válido; verificar mapeamento para CreateUserOutput
   - Cenário erro UsernameExistsException: mock lança exceção; verificar que exceção apropriada (ConflictException ou equivalente) é lançada
   - Cenário erro InvalidPasswordException: mock lança exceção; verificar que exceção apropriada (UnprocessableEntityException ou equivalente) é lançada
   - Cenário erro InvalidParameterException: mock lança exceção; verificar que ArgumentException (ou equivalente) é lançada
   - Testar cálculo de SECRET_HASH: com e sem clientSecret presente (reutilizar testes de Subtask 03 da Story 02)
5. Executar `dotnet test --collect:"XPlat Code Coverage"` e verificar cobertura ≥ 80%

## Formas de Teste
1. Executar `dotnet test` e verificar que todos os testes passam
2. Executar `dotnet test --collect:"XPlat Code Coverage"` e gerar relatório de cobertura
3. Analisar relatório de cobertura e identificar linhas não cobertas (se houver)
4. Adicionar testes adicionais para cenários edge case se cobertura < 80%

## Critérios de Aceite da Subtask
- [ ] Suite completa de testes criada para CreateUserInputValidator (mínimo 10 testes cobrindo todas as regras)
- [ ] Testes de CreateUserPresenter criados (mapeamento completo e cenários UserConfirmed true/false)
- [ ] Testes de CreateUserUseCase criados (sucesso, erros, logs) usando Moq para mockar service e logger
- [ ] Testes de CognitoAuthService.SignUpAsync criados (sucesso, erros Cognito, SECRET_HASH) usando Moq para mockar SDK
- [ ] Todos os testes passando; `dotnet test` executa sem erros
- [ ] Cobertura de código ≥ 80% medida com coverlet; relatório gerado e revisado
- [ ] Nenhum warning ou erro de build nos projetos de teste
