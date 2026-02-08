using Microsoft.AspNetCore.Mvc;
using VideoProcessing.Auth.Application.InputModels.Auth;
using VideoProcessing.Auth.Application.ResponseModels.Auth;
using VideoProcessing.Auth.Application.UseCases.Auth;

namespace VideoProcessing.Auth.Api.Controllers.Auth;

/// <summary>
/// Controller para operações de gerenciamento de usuários.
/// </summary>
[ApiController]
[Route("auth/users")]
public class UserController : ControllerBase
{
    private readonly CreateUserUseCase _createUserUseCase;
    private readonly ILogger<UserController> _logger;

    public UserController(
        CreateUserUseCase createUserUseCase,
        ILogger<UserController> logger)
    {
        _createUserUseCase = createUserUseCase;
        _logger = logger;
    }

    /// <summary>
    /// Cria novo usuário no sistema integrando com Amazon Cognito User Pool.
    /// </summary>
    /// <param name="input">Dados de criação do usuário contendo name (ex.: "Diego"), email (identificador de login quando username_attributes = ["email"]) e password (mínimo 8 caracteres conforme política do Cognito).</param>
    /// <param name="cancellationToken">Token de cancelamento para operação assíncrona.</param>
    /// <returns>Informações do usuário criado incluindo userId (identificador único no Cognito), username (email), userConfirmed e confirmationRequired.</returns>
    /// <response code="201">Usuário criado com sucesso no Cognito User Pool.</response>
    /// <response code="400">Dados de entrada inválidos. Validação falhou (name, email ou senha incorretos).</response>
    /// <response code="409">Usuário já existe no sistema. Email já cadastrado no Cognito User Pool.</response>
    /// <response code="422">Senha não atende aos requisitos de política do pool Cognito (ex.: complexidade insuficiente, muito curta).</response>
    /// <remarks>
    /// name: nome do usuário (ex.: "Diego"). email: endereço de email válido e único, usado como identificador de login no Cognito quando o pool usa sign-in por email.
    /// Se o Cognito User Pool estiver configurado para exigir confirmação, o usuário receberá um código por email/SMS e precisará confirmar a conta antes de poder fazer login.
    /// Após a criação bem-sucedida, use o email e a senha no endpoint POST /auth/login.
    /// </remarks>
    [HttpPost("create")]
    [ProducesResponseType(typeof(CreateUserResponseModel), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Create([FromBody] CreateUserInput input, CancellationToken cancellationToken)
    {
        _logger.LogInformation("POST /auth/users/create called for email {Email}", input.Email);

        var result = await _createUserUseCase.ExecuteAsync(input, cancellationToken);

        return StatusCode(StatusCodes.Status201Created, result);
    }
}
