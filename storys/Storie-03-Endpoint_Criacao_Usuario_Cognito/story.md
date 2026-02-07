# Storie-03: Endpoint de Criação de Usuário com Cognito (SignUp)

## Status
- **Estado:** 🔄 Em desenvolvimento
- **Data de Conclusão:** —

## Descrição
Como usuário novo da plataforma, quero criar uma conta com username, password e email, para poder me registrar no sistema e posteriormente fazer login para acessar funcionalidades protegidas.

## Objetivo
Implementar endpoint POST /auth/users/create integrando com Amazon Cognito via SignUp, incluindo envio de atributo email como UserAttribute, cálculo condicional de SECRET_HASH, validação de input (formato de email, username alfanumérico), tratamento de erros do Cognito (usuário já existe, senha inválida), logging estruturado sem dados sensíveis, e testes unitários completos.

## Escopo Técnico
- Tecnologias: .NET 10, ASP.NET Core, AWS SDK (AWSSDK.CognitoIdentityProvider), FluentValidation
- Arquivos afetados:
  - `src/VideoProcessing.Auth.Api/Controllers/Auth/UserController.cs` (novo)
  - `src/VideoProcessing.Auth.Application/UseCases/Auth/CreateUserUseCase.cs` (novo)
  - `src/VideoProcessing.Auth.Application/InputModels/Auth/CreateUserInput.cs` (novo)
  - `src/VideoProcessing.Auth.Application/OutputModels/Auth/CreateUserOutput.cs` (novo)
  - `src/VideoProcessing.Auth.Application/Validators/Auth/CreateUserInputValidator.cs` (novo)
  - `src/VideoProcessing.Auth.Application/Presenters/Auth/CreateUserPresenter.cs` (novo)
  - `src/VideoProcessing.Auth.Application/Ports/ICognitoAuthService.cs` (atualizar)
  - `src/VideoProcessing.Auth.Infra/Services/CognitoAuthService.cs` (atualizar)
  - `tests/VideoProcessing.Auth.Tests.Unit/UseCases/Auth/CreateUserUseCaseTests.cs` (novo)
  - `tests/VideoProcessing.Auth.Tests.Unit/Validators/Auth/CreateUserInputValidatorTests.cs` (novo)
  - `tests/VideoProcessing.Auth.Tests.Unit/Presenters/Auth/CreateUserPresenterTests.cs` (novo)
  - `tests/VideoProcessing.Auth.Tests.Unit/Services/CognitoAuthServiceSignUpTests.cs` (novo)
- Componentes/Recursos: UserController, CreateUserUseCase, ICognitoAuthService (método SignUpAsync), CognitoAuthService, FluentValidation
- Pacotes/Dependências (já instalados na Story 01):
  - AWSSDK.CognitoIdentityProvider (3.7.400.16)
  - FluentValidation (11.11.0)
  - FluentValidation.AspNetCore (11.3.0)
  - xUnit (2.9.2)
  - Moq (4.20.72)
  - FluentAssertions (7.0.0)

## Dependências e Riscos (para estimativa)
- Dependências: Story 01 concluída; Story 02 desejável (mas não bloqueante, pois usa mesma infra Cognito)
- Riscos/Pré-condições:
  - Cognito User Pool criado com atributo email configurado (obrigatório ou opcional, conforme config do pool)
  - App Client com USER_PASSWORD_AUTH habilitado
  - Variáveis de ambiente configuradas (AWS_REGION, COGNITO_APP_CLIENT_ID, COGNITO_APP_CLIENT_SECRET se aplicável)
  - Se o pool exigir confirmação por email/SMS, documentar que fluxo de confirmação (ConfirmSignUp) fica fora do escopo inicial

## Subtasks
- [Subtask 01: Criar CreateUserInput e CreateUserInputValidator](./subtask/Subtask-01-CreateUserInput_Validator.md)
- [Subtask 02: Adicionar método SignUpAsync em ICognitoAuthService](./subtask/Subtask-02-ICognitoAuthService_SignUp.md)
- [Subtask 03: Implementar CognitoAuthService SignUpAsync](./subtask/Subtask-03-CognitoAuthService_SignUp.md)
- [Subtask 04: Criar CreateUserOutput CreateUserResponseModel e CreateUserPresenter](./subtask/Subtask-04-CreateUserOutput_Presenter.md)
- [Subtask 05: Implementar CreateUserUseCase](./subtask/Subtask-05-CreateUserUseCase.md)
- [Subtask 06: Criar UserController com endpoint POST users create](./subtask/Subtask-06-UserController_Create.md)
- [Subtask 07: Testes unitários completos](./subtask/Subtask-07-Testes_Unitarios_CreateUser.md)

## Critérios de Aceite da História
- [ ] POST /auth/users/create aceita `{ "username": "user", "password": "pass", "email": "user@example.com" }` e retorna 201 Created com userId, username, userConfirmed, confirmationRequired
- [ ] CreateUserInputValidator valida username (formato alfanumérico + _ e -), password (mínimo 8 caracteres), email (formato válido); retorna 400 Bad Request se inválido
- [ ] CognitoAuthService.SignUpAsync envia email como UserAttribute; calcula SECRET_HASH condicionalmente se clientSecret presente
- [ ] Tratamento de erros: 409 Conflict para `UsernameExistsException`, 422 Unprocessable Entity para `InvalidPasswordException`, 400 Bad Request para `InvalidParameterException`
- [ ] Resposta de sucesso encapsulada em `ApiResponse<CreateUserResponseModel>` pelo filtro global; status HTTP 201 Created
- [ ] Logs estruturados: logar tentativa de criação de usuário (username/email) e resultado (sucesso/falha); NUNCA logar password ou secret
- [ ] Testes unitários: cobertura ≥ 80% para Validator, UseCase, Presenter e Service (mocking IAmazonCognitoIdentityProvider SDK)
- [ ] `dotnet build` e `dotnet test` executam sem erros; todos os testes passando

## Rastreamento (dev tracking)
- **Início:** —
- **Fim:** —
- **Tempo total de desenvolvimento:** —
