using CadastroDePessoas.Application.CQRS.Comandos.Usuario.AutenticarUsuario;
using CadastroDePessoas.Application.DTOs.Usuario;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CadastroDePessoas.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class AuthController(IMediator mediator) : ControllerBase
    {
        [HttpPost("login")]
        [AllowAnonymous]
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
                expiresIn = "1h"
            });
        }

        [HttpPost("logout")]
        [AllowAnonymous]
        public ActionResult Logout()
        {
            return Ok(new { message = "Logout realizado com sucesso" });
        }

        [HttpGet("verify")]
        [Authorize]
        public ActionResult VerifyToken()
        {
            return Ok(new 
            { 
                valid = true,
                user = new
                {
                    id = User.FindFirst("sub")?.Value,
                    email = User.FindFirst("email")?.Value,
                    name = User.FindFirst("name")?.Value
                }
            });
        }
    }
}