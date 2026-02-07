# Subtask 07: Testes Unitários Completos do Login

## Descrição
Criar suite completa de testes unitários para todos os componentes do fluxo de login (Validator, Presenter, UseCase, Service) garantindo cobertura ≥ 80% e validando todos os cenários de sucesso e erro.

## Passos de Implementação
1. **LoginInputValidatorTests:**
   - Testar username vazio → erro "Username é obrigatório"
   - Testar username com 129 caracteres → erro "no máximo 128 caracteres"
   - Testar password vazio → erro "Password é obrigatório"
   - Testar password com 7 caracteres → erro "pelo menos 8 caracteres"
   - Testar password com 257 caracteres → erro "no máximo 256 caracteres"
   - Testar input válido → sem erros
2. **LoginPresenterTests:**
   - Testar mapeamento completo de LoginOutput → LoginResponseModel
   - Testar que RefreshToken nullable é mapeado corretamente (com e sem valor)
3. **LoginUseCaseTests:**
   - Cenário sucesso: mockar service retornando LoginOutput válido; verificar que UseCase retorna LoginResponseModel correto
   - Cenário erro: mockar service lançando UnauthorizedAccessException; verificar que exceção propaga
   - Verificar que presenter é chamado corretamente
   - Verificar que logs são emitidos (mock de ILogger)
4. **CognitoAuthServiceTests:**
   - Mockar IAmazonCognitoIdentityProvider (criar interface wrapper ou usar técnica de mock)
   - Cenário sucesso: mock retorna InitiateAuthResponse válido; verificar mapeamento para LoginOutput
   - Cenário erro NotAuthorizedException: mock lança exceção; verificar que UnauthorizedAccessException é lançada com mensagem genérica
   - Cenário erro UserNotFoundException: mock lança exceção; verificar que UnauthorizedAccessException é lançada
   - Testar cálculo de SECRET_HASH: com e sem clientSecret presente
5. **SecretHashCalculatorTests:**
   - Testar `ComputeSecretHash` com valores conhecidos e verificar hash esperado (usar valores de exemplo da documentação AWS)
6. Executar `dotnet test --collect:"XPlat Code Coverage"` e verificar cobertura ≥ 80%

## Formas de Teste
1. Executar `dotnet test` e verificar que todos os testes passam
2. Executar `dotnet test --collect:"XPlat Code Coverage"` e gerar relatório de cobertura
3. Analisar relatório de cobertura e identificar linhas não cobertas (se houver)
4. Adicionar testes adicionais para cenários edge case se cobertura < 80%

## Critérios de Aceite da Subtask
- [ ] Suite completa de testes criada para LoginInputValidator (mínimo 6 testes cobrindo todas as regras)
- [ ] Testes de LoginPresenter criados (mapeamento completo e RefreshToken nullable)
- [ ] Testes de LoginUseCase criados (sucesso, erro, logs) usando Moq para mockar service e logger
- [ ] Testes de CognitoAuthService criados (sucesso, erros Cognito, SECRET_HASH) usando Moq para mockar SDK
- [ ] Testes de SecretHashCalculator criados (valores conhecidos e hash esperado)
- [ ] Todos os testes passando; `dotnet test` executa sem erros
- [ ] Cobertura de código ≥ 80% medida com coverlet; relatório gerado e revisado
- [ ] Nenhum warning ou erro de build nos projetos de teste
