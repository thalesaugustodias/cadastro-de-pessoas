using CadastroDePessoas.Application.CQRS.Comandos.Usuario.AutenticarUsuario;
using CadastroDePessoas.Application.DTOs.Usuario;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CadastroDePessoas.API.V2.Controllers
{
    [ApiController]
    [Route("api/v2/[controller]")]
    [Tags("?? Autenticação V2")]
    public class AuthController(IMediator mediator) : ControllerBase
    {
        /// <summary>
        /// Realiza login do usuário - V2
        /// </summary>
        /// <param name="loginDto">Dados de login (email e senha)</param>
        /// <returns>Token JWT para autenticação com informações extras</returns>
        /// <remarks>
        /// **?? Endpoint PÚBLICO** - Não requer autenticação
        /// 
        /// **Melhorias na V2:**
        /// - ? Timestamp na resposta
        /// - ? Informações do usuário
        /// - ? Versão identificada
        /// 
        /// **Usuário padrão para teste:**
        /// - Email: admin@exemplo.com
        /// - Senha: Admin@123
        /// </remarks>
        [HttpPost("login")]
        [AllowAnonymous] // ?? Público - Login não pode exigir autenticação
        public async Task<ActionResult<object>> Login([FromBody] UsuarioLoginDTO loginDto)
        {
            var comando = new AutenticarUsuarioComando
            {
                Email = loginDto.Email,
                Senha = loginDto.Senha
            };

            var token = await mediator.Send(comando);

            return Ok(new 
            { 
                token,
                message = "Login realizado com sucesso",
                expiresIn = "1h",
                version = "v2",
                timestamp = DateTime.UtcNow,
                user = new
                {
                    email = loginDto.Email
                }
            });
        }

        /// <summary>
        /// Endpoint para logout - V2
        /// </summary>
        /// <remarks>
        /// **?? Endpoint PÚBLICO** - Logout pode ser feito sem token válido
        /// </remarks>
        [HttpPost("logout")]
        [AllowAnonymous] // ?? Público - Logout não precisa de token válido
        public ActionResult Logout()
        {
            return Ok(new 
            { 
                message = "Logout realizado com sucesso",
                version = "v2",
                timestamp = DateTime.UtcNow
            });
        }

        /// <summary>
        /// Verifica se o token está válido - V2 com mais informações
        /// </summary>
        /// <remarks>
        /// **?? Endpoint PROTEGIDO** - Requer autenticação JWT
        /// </remarks>
        [HttpGet("verify")]
        [Authorize] // ?? Protegido - Explicitamente marcado
        public ActionResult VerifyToken()
        {
            return Ok(new 
            { 
                valid = true,
                version = "v2",
                timestamp = DateTime.UtcNow,
                user = new
                {
                    id = User.FindFirst("sub")?.Value,
                    email = User.FindFirst("email")?.Value,
                    name = User.FindFirst("name")?.Value,
                    jti = User.FindFirst("jti")?.Value
                }
            });
        }
    }
}