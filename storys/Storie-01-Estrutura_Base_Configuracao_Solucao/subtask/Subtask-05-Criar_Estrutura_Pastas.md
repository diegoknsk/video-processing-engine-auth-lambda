# Subtask 05: Criar Estrutura de Pastas

## Descrição
Criar a estrutura completa de pastas nos projetos Api, Application e Infra conforme plano arquitetural, seguindo Clean Architecture e organização por contexto (Auth).

## Passos de Implementação
1. No projeto **Api**, criar pastas:
   - `Controllers/Auth/` (para AuthController e UserController)
   - `Filters/` (para ApiResponseFilter e GlobalExceptionFilter)
   - `Models/` (para ApiResponse e DTOs de resposta)
2. No projeto **Application**, criar pastas:
   - `UseCases/Auth/` (para LoginUseCase e CreateUserUseCase)
   - `InputModels/Auth/` (para LoginInput e CreateUserInput)
   - `OutputModels/Auth/` (para LoginOutput e CreateUserOutput)
   - `Validators/Auth/` (para LoginInputValidator e CreateUserInputValidator)
   - `Presenters/Auth/` (para LoginPresenter e CreateUserPresenter)
   - `Ports/` (para ICognitoAuthService)
3. No projeto **Infra**, criar pastas:
   - `Services/` (para CognitoAuthService)
   - `Models/` (para CognitoOptions e SecretHashCalculator)
4. No projeto **Tests.Unit**, criar pastas:
   - `UseCases/Auth/`
   - `Validators/Auth/`
   - `Presenters/Auth/`
   - `Services/`
5. Adicionar arquivo `.gitkeep` em cada pasta vazia para garantir que sejam versionadas
6. Documentar estrutura em comentário no README.md (se existir) ou criar arquivo `docs/estrutura-projetos.md`

## Formas de Teste
1. Executar `dotnet build` e confirmar que pastas vazias não causam erros de compilação
2. Listar estrutura de diretórios com `tree` (PowerShell) ou explorador de arquivos e comparar com plano arquitetural
3. Verificar que todas as pastas foram criadas nos locais corretos
4. Confirmar que arquivos `.gitkeep` estão presentes (se usado Git)

## Critérios de Aceite da Subtask
- [ ] Todas as pastas listadas criadas nos projetos corretos (Api, Application, Infra, Tests.Unit)
- [ ] Estrutura alinhada ao plano arquitetural (organização por contexto Auth)
- [ ] Pastas vazias contêm `.gitkeep` para serem versionadas
- [ ] `dotnet build` executa sem erros
- [ ] Documentação da estrutura criada ou atualizada (README ou doc separado)
