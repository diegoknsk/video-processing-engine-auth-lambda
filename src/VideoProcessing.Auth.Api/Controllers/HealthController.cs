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
    /// Health check endpoint.
    /// </summary>
    /// <returns>Status da aplicação.</returns>
    /// <response code="200">Aplicação está saudável.</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult Health()
    {
        return Ok(new { status = "Healthy", timestamp = DateTime.UtcNow });
    }
}
