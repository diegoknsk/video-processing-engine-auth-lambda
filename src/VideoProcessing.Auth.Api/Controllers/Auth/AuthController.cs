using Microsoft.AspNetCore.Mvc;
using VideoProcessing.Auth.Application.InputModels.Auth;
using VideoProcessing.Auth.Application.ResponseModels.Auth;
using VideoProcessing.Auth.Application.UseCases.Auth;

namespace VideoProcessing.Auth.Api.Controllers.Auth;

/// <summary>
/// Controller para operações de autenticação.
/// </summary>
[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly LoginUseCase _loginUseCase;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        LoginUseCase loginUseCase,
        ILogger<AuthController> logger)
    {
        _loginUseCase = loginUseCase;
        _logger = logger;
    }

    /// <summary>
    /// Autentica usuário e retorna tokens JWT.
    /// </summary>
    /// <param name="input">Credenciais de login (username e password).</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Tokens de autenticação (AccessToken, IdToken, RefreshToken, ExpiresIn).</returns>
    /// <response code="200">Login realizado com sucesso.</response>
    /// <response code="400">Dados de entrada inválidos.</response>
    /// <response code="401">Credenciais inválidas.</response>
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponseModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginInput input, CancellationToken cancellationToken)
    {
        _logger.LogInformation("POST /auth/login called for user {Username}", input.Username);

        var result = await _loginUseCase.ExecuteAsync(input, cancellationToken);

        return Ok(result);
    }
}
