using CadastroDePessoas.Application.CQRS.Comandos.Usuario.AutenticarUsuario;
using CadastroDePessoas.Application.DTOs.Usuario;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CadastroDePessoas.API.V2.Controllers
{
    [ApiController]
    [Route("api/v2/[controller]")]
    [Tags("?? Autentica��o V2")]
    public class AuthController(IMediator mediator) : ControllerBase
    {
        /// <summary>
        /// Realiza login do usu�rio - V2
        /// </summary>
        /// <param name="loginDto">Dados de login (email e senha)</param>
        /// <returns>Token JWT para autentica��o com informa��es extras</returns>
        /// <remarks>
        /// **?? Endpoint P�BLICO** - N�o requer autentica��o
        /// 
        /// **Melhorias na V2:**
        /// - ? Timestamp na resposta
        /// - ? Informa��es do usu�rio
        /// - ? Vers�o identificada
        /// 
        /// **Usu�rio padr�o para teste:**
        /// - Email: admin@exemplo.com
        /// - Senha: Admin@123
        /// </remarks>
        [HttpPost("login")]
        [AllowAnonymous] // ?? P�blico - Login n�o pode exigir autentica��o
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
        /// **?? Endpoint P�BLICO** - Logout pode ser feito sem token v�lido
        /// </remarks>
        [HttpPost("logout")]
        [AllowAnonymous] // ?? P�blico - Logout n�o precisa de token v�lido
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
        /// Verifica se o token est� v�lido - V2 com mais informa��es
        /// </summary>
        /// <remarks>
        /// **?? Endpoint PROTEGIDO** - Requer autentica��o JWT
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