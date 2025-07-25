using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CadastroDePessoas.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        [AllowAnonymous]
        public ActionResult<object> Health()
        {
            return Ok(new
            {
                status = "healthy",
                timestamp = DateTime.UtcNow,
                version = "v1.0",
                environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Unknown",
                uptime = Environment.TickCount64
            });
        }

        [HttpGet("detailed")]
        [Authorize]
        public ActionResult<object> DetailedHealth()
        {
            return Ok(new
            {
                status = "healthy",
                timestamp = DateTime.UtcNow,
                version = "v1.0",
                environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Unknown",
                uptime = Environment.TickCount64,
                authenticated = User.Identity?.IsAuthenticated ?? false,
                user = User.Identity?.Name,
                claims = User.Claims.Select(c => new { c.Type, c.Value }),
                memory = GC.GetTotalMemory(false),
                processorCount = Environment.ProcessorCount
            });
        }
    }
}
