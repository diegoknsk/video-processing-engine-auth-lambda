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
    /// Cria novo usuário no sistema.
    /// </summary>
    /// <param name="input">Dados de criação (username, password, email).</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Informações do usuário criado (userId, username, userConfirmed, confirmationRequired).</returns>
    /// <response code="201">Usuário criado com sucesso.</response>
    /// <response code="400">Dados de entrada inválidos.</response>
    /// <response code="409">Usuário já existe.</response>
    /// <response code="422">Senha não atende aos requisitos de política do pool.</response>
    [HttpPost("create")]
    [ProducesResponseType(typeof(CreateUserResponseModel), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Create([FromBody] CreateUserInput input, CancellationToken cancellationToken)
    {
        _logger.LogInformation("POST /auth/users/create called for username {Username}", input.Username);

        var result = await _createUserUseCase.ExecuteAsync(input, cancellationToken);

        return StatusCode(StatusCodes.Status201Created, result);
    }
}
