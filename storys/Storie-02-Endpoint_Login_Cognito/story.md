# Storie-02: Endpoint de Login com Cognito (InitiateAuth)

## Status
- **Estado:** 🔄 Em desenvolvimento
- **Data de Conclusão:** —

## Descrição
Como usuário final da plataforma, quero fazer login com username e password, para obter tokens de autenticação (AccessToken, IdToken, RefreshToken) que me permitam acessar funcionalidades protegidas do sistema.

## Objetivo
Implementar endpoint POST /auth/login integrando com Amazon Cognito via InitiateAuth usando fluxo USER_PASSWORD_AUTH, incluindo cálculo condicional de SECRET_HASH, validação de input com FluentValidation, tratamento de erros do Cognito, logging estruturado sem dados sensíveis, e testes unitários completos.

## Escopo Técnico
- Tecnologias: .NET 10, ASP.NET Core, AWS SDK (AWSSDK.CognitoIdentityProvider), FluentValidation
- Arquivos afetados:
  - `src/VideoProcessing.Auth.Api/Controllers/Auth/AuthController.cs` (novo)
  - `src/VideoProcessing.Auth.Application/UseCases/Auth/LoginUseCase.cs` (novo)
  - `src/VideoProcessing.Auth.Application/InputModels/Auth/LoginInput.cs` (novo)
  - `src/VideoProcessing.Auth.Application/OutputModels/Auth/LoginOutput.cs` (novo)
  - `src/VideoProcessing.Auth.Application/Validators/Auth/LoginInputValidator.cs` (novo)
  - `src/VideoProcessing.Auth.Application/Presenters/Auth/LoginPresenter.cs` (novo)
  - `src/VideoProcessing.Auth.Application/Ports/ICognitoAuthService.cs` (novo)
  - `src/VideoProcessing.Auth.Infra/Services/CognitoAuthService.cs` (novo)
  - `src/VideoProcessing.Auth.Infra/Models/SecretHashCalculator.cs` (novo)
  - `tests/VideoProcessing.Auth.Tests.Unit/UseCases/Auth/LoginUseCaseTests.cs` (novo)
  - `tests/VideoProcessing.Auth.Tests.Unit/Validators/Auth/LoginInputValidatorTests.cs` (novo)
  - `tests/VideoProcessing.Auth.Tests.Unit/Presenters/Auth/LoginPresenterTests.cs` (novo)
  - `tests/VideoProcessing.Auth.Tests.Unit/Services/CognitoAuthServiceTests.cs` (novo)
- Componentes/Recursos: AuthController, LoginUseCase, ICognitoAuthService, CognitoAuthService, SECRET_HASH calculator, FluentValidation
- Pacotes/Dependências (já instalados na Story 01):
  - AWSSDK.CognitoIdentityProvider (3.7.400.16)
  - FluentValidation (11.11.0)
  - FluentValidation.AspNetCore (11.3.0)
  - xUnit (2.9.2)
  - Moq (4.20.72)
  - FluentAssertions (7.0.0)

## Dependências e Riscos (para estimativa)
- Dependências: Story 01 concluída (estrutura base, projetos, DI, FluentValidation)
- Riscos/Pré-condições:
  - Cognito User Pool criado e App Client configurado (via infra)
  - Variáveis de ambiente configuradas (AWS_REGION, COGNITO_APP_CLIENT_ID, COGNITO_APP_CLIENT_SECRET se aplicável)
  - App Client com USER_PASSWORD_AUTH habilitado
  - Conhecimento de cálculo de SECRET_HASH (HMAC-SHA256)

## Subtasks
- [Subtask 01: Criar LoginInput e LoginInputValidator](./subtask/Subtask-01-LoginInput_Validator.md)
- [Subtask 02: Criar ICognitoAuthService (Port)](./subtask/Subtask-02-ICognitoAuthService_Port.md)
- [Subtask 03: Implementar CognitoAuthService LoginAsync](./subtask/Subtask-03-CognitoAuthService_Login.md)
- [Subtask 04: Criar LoginOutput LoginResponseModel e LoginPresenter](./subtask/Subtask-04-LoginOutput_Presenter.md)
- [Subtask 05: Implementar LoginUseCase](./subtask/Subtask-05-LoginUseCase.md)
- [Subtask 06: Criar AuthController com endpoint POST login](./subtask/Subtask-06-AuthController_Login.md)
- [Subtask 07: Testes unitários completos](./subtask/Subtask-07-Testes_Unitarios_Login.md)

## Critérios de Aceite da História
- [ ] POST /auth/login aceita `{ "username": "user", "password": "pass" }` e retorna 200 OK com tokens JWT (accessToken, idToken, refreshToken, expiresIn, tokenType)
- [ ] LoginInputValidator valida username e password (obrigatórios, tamanhos mínimos/máximos); retorna 400 Bad Request com erros claros quando inválido
- [ ] CognitoAuthService calcula SECRET_HASH somente se `COGNITO_APP_CLIENT_SECRET` estiver presente; chamada ao Cognito SDK InitiateAuth funciona corretamente
- [ ] Tratamento de erros: 401 Unauthorized para `NotAuthorizedException` e `UserNotFoundException` com mensagem genérica "Credenciais inválidas" (não vazar se usuário existe)
- [ ] Resposta de sucesso encapsulada em `ApiResponse<LoginResponseModel>` pelo filtro global (formato: `{ "success": true, "data": {...}, "timestamp": "..." }`)
- [ ] Logs estruturados: logar tentativa de login (username) e resultado (sucesso/falha); NUNCA logar password, secret ou tokens
- [ ] Testes unitários: cobertura ≥ 80% para Validator, UseCase, Presenter e Service (mocking IAmazonCognitoIdentityProvider SDK)
- [ ] `dotnet build` e `dotnet test` executam sem erros; todos os testes passando

## Rastreamento (dev tracking)
- **Início:** —
- **Fim:** —
- **Tempo total de desenvolvimento:** —
