using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CadastroDePessoas.Infraestructure.Contexto;

namespace CadastroDePessoas.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class HealthController(AppDbContexto dbContext) : ControllerBase
    {
        /// <summary>
        /// Verificação de saúde da API
        /// </summary>
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

        /// <summary>
        /// Verificação detalhada (requer autenticação)
        /// </summary>
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

        /// <summary>
        /// Verifica status do banco de dados
        /// </summary>
        [HttpGet("database")]
        [AllowAnonymous]
        public async Task<ActionResult<object>> DatabaseHealth()
        {
            try
            {
                var usuariosCount = await dbContext.Usuarios.CountAsync();
                var pessoasCount = await dbContext.Pessoas.CountAsync();

                return Ok(new
                {
                    status = "healthy",
                    database = "connected",
                    users = usuariosCount,
                    pessoas = pessoasCount,
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = "unhealthy",
                    database = "disconnected",
                    error = ex.Message,
                    timestamp = DateTime.UtcNow
                });
            }
        }
    }
}
