using CadastroDePessoas.Application.CQRS.Comandos.Usuario.AutenticarUsuario;
using CadastroDePessoas.Application.DTOs.Usuario;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CadastroDePessoas.API.V2.Controllers
{
    [ApiController]
    [Route("api/v2/[controller]")]
    public class AuthController(IMediator mediator) : ControllerBase
    {
        [HttpPost("login")]
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

        [HttpPost("logout")]
        public ActionResult Logout()
        {
            return Ok(new 
            { 
                message = "Logout realizado com sucesso",
                version = "v2",
                timestamp = DateTime.UtcNow
            });
        }

        [HttpGet("verify")]
        [Microsoft.AspNetCore.Authorization.Authorize]
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