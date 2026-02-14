using Microsoft.AspNetCore.Mvc;
using VideoProcessing.Auth.Application.InputModels.Auth;
using VideoProcessing.Auth.Application.ResponseModels.Auth;
using VideoProcessing.Auth.Application.UseCases.Auth;

namespace VideoProcessing.Auth.Api.Controllers.Auth;

/// <summary>
/// Controller para operações de autenticação.
/// </summary>
[ApiController]
[Route("login")]
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
    /// Autentica usuário no sistema usando Amazon Cognito e retorna tokens JWT para acesso às APIs protegidas.
    /// </summary>
    /// <param name="input">Credenciais de login contendo email (identificador de login quando o pool usa sign-in por email) e password (mínimo 8 caracteres conforme política do Cognito).</param>
    /// <param name="cancellationToken">Token de cancelamento para operação assíncrona.</param>
    /// <returns>Tokens de autenticação incluindo AccessToken (para autorização em APIs), IdToken (identidade do usuário), RefreshToken (para renovação de tokens) e ExpiresIn (tempo de expiração em segundos).</returns>
    /// <response code="200">Login realizado com sucesso. Retorna tokens JWT válidos.</response>
    /// <response code="400">Dados de entrada inválidos. Validação falhou (email ou password em formato incorreto).</response>
    /// <response code="401">Credenciais inválidas. Usuário não encontrado ou senha incorreta.</response>
    /// <remarks>
    /// O email deve ser um endereço válido (é o identificador de login quando o Cognito User Pool está configurado com username_attributes = ["email"]).
    /// A senha deve atender aos requisitos de política configurados no Cognito User Pool.
    /// Os tokens retornados devem ser incluídos no header Authorization das requisições subsequentes às APIs protegidas.
    /// </remarks>
    [HttpPost]
    [ProducesResponseType(typeof(LoginResponseModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginInput input, CancellationToken cancellationToken)
    {
        _logger.LogInformation("POST /login called for user {Email}", input.Email);

        var result = await _loginUseCase.ExecuteAsync(input, cancellationToken);

        return Ok(result);
    }
}
