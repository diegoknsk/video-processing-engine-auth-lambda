using Microsoft.AspNetCore.Mvc;

namespace VideoProcessing.Auth.Api.Controllers;

/// <summary>
/// Controller para health check da aplicação.
/// </summary>
[ApiController]
[Route("health")]
public class HealthController : ControllerBase
{
    /// <summary>
    /// Endpoint de health check para monitoramento da aplicação e verificação de disponibilidade.
    /// </summary>
    /// <returns>Status da aplicação incluindo status "Healthy" e timestamp UTC da verificação.</returns>
    /// <response code="200">Aplicação está saudável e respondendo normalmente.</response>
    /// <remarks>
    /// Este endpoint é utilizado por sistemas de monitoramento, load balancers e orquestradores para verificar a saúde da aplicação.
    /// Retorna sempre 200 OK quando a aplicação está em execução, indicando que o serviço está disponível para processar requisições.
    /// O timestamp retornado indica o momento exato da verificação de saúde.
    /// </remarks>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult Health()
    {
        return Ok(new { status = "Healthy", timestamp = DateTime.UtcNow });
    }
}
