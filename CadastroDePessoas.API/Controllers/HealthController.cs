using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CadastroDePessoas.Infraestructure.Contexto;

namespace CadastroDePessoas.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Tags("🏥 Health Check")]
    public class HealthController(AppDbContexto dbContext) : ControllerBase
    {
        /// <summary>
        /// Verificação de saúde da API
        /// </summary>
        /// <remarks>
        /// **⚠️ Endpoint PÚBLICO** - Usado para monitoramento
        /// </remarks>
        [HttpGet]
        [AllowAnonymous] // 🔓 Público - Health check deve ser acessível
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
        /// <remarks>
        /// **🔒 Endpoint PROTEGIDO** - Informações sensíveis do sistema
        /// </remarks>
        [HttpGet("detailed")]
        [Authorize] // 🔒 Protegido - Informações sensíveis
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
        /// <remarks>
        /// **⚠️ Endpoint PÚBLICO** - Para verificar conectividade do banco
        /// </remarks>
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

        /// <summary>
        /// Recria o banco de dados (APENAS DESENVOLVIMENTO)
        /// </summary>
        /// <remarks>
        /// **🚨 ENDPOINT PERIGOSO** - Apaga todos os dados e recria o banco
        /// 
        /// Use apenas para resetar o banco em desenvolvimento!
        /// </remarks>
        [HttpPost("reset-database")]
        [AllowAnonymous] // 🔓 Público apenas para facilitar desenvolvimento
        public async Task<ActionResult<object>> ResetDatabase()
        {
            var ambiente = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            
            if (ambiente != "Development")
            {
                return BadRequest(new { message = "Endpoint disponível apenas em ambiente de desenvolvimento" });
            }

            try
            {
                // Apagar banco
                await dbContext.Database.EnsureDeletedAsync();
                
                // Recriar banco com seed data
                await dbContext.Database.EnsureCreatedAsync();

                var usuariosCount = await dbContext.Usuarios.CountAsync();

                return Ok(new
                {
                    message = "Banco de dados resetado com sucesso",
                    users_created = usuariosCount,
                    default_users = new[]
                    {
                        new { email = "admin@exemplo.com", password = "Admin@123" },
                        new { email = "user@teste.com", password = "User@123" }
                    },
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Erro ao resetar banco de dados",
                    error = ex.Message,
                    timestamp = DateTime.UtcNow
                });
            }
        }
    }
}
