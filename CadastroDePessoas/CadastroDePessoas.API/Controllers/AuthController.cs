using CadastroDePessoas.Application.CQRS.Comandos.Usuario.AutenticarUsuario;
using CadastroDePessoas.Application.CQRS.Comandos.Usuario.CriarUsuario;
using CadastroDePessoas.Application.DTOs.Usuario;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CadastroDePessoas.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Tags("?? Autentica��o V1")]
    public class AuthController(IMediator mediator) : ControllerBase
    {
        /// <summary>
        /// Realiza login do usu�rio
        /// </summary>
        /// <param name="loginDto">Dados de login (email e senha)</param>
        /// <returns>Token JWT para autentica��o</returns>
        /// <remarks>
        /// **?? Endpoint P�BLICO** - N�o requer autentica��o
        /// 
        /// **Usu�rios padr�o para teste:**
        /// - Email: admin@exemplo.com / Senha: Admin@123
        /// - Email: user@teste.com / Senha: User@123
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
                expiresIn = "1h"
            });
        }

        /// <summary>
        /// Cria um novo usu�rio no sistema
        /// </summary>
        /// <param name="criarUsuarioDto">Dados do novo usu�rio</param>
        /// <returns>Confirma��o de cria��o</returns>
        /// <remarks>
        /// **?? Endpoint P�BLICO TEMPOR�RIO** - Para facilitar testes iniciais
        /// 
        /// **Regras de senha:**
        /// - M�nimo 6 caracteres
        /// - Pelo menos 1 letra min�scula
        /// - Pelo menos 1 letra mai�scula  
        /// - Pelo menos 1 n�mero
        /// - Pelo menos 1 caractere especial (@$!%*?&)
        /// 
        /// **Exemplo de senha v�lida:** MinhaSenh@123
        /// </remarks>
        [HttpPost("register")]
        [AllowAnonymous] // ?? Temporariamente p�blico para facilitar testes
        public async Task<ActionResult<object>> Register([FromBody] CriarUsuarioComando criarUsuarioDto)
        {
            var sucesso = await mediator.Send(criarUsuarioDto);

            return Ok(new 
            { 
                success = sucesso,
                message = "Usu�rio criado com sucesso",
                user = new
                {
                    name = criarUsuarioDto.Nome,
                    email = criarUsuarioDto.Email
                }
            });
        }

        /// <summary>
        /// Endpoint para logout (apenas remove token no client-side)
        /// </summary>
        /// <remarks>
        /// **?? Endpoint P�BLICO** - Logout pode ser feito sem token v�lido
        /// </remarks>
        [HttpPost("logout")]
        [AllowAnonymous] // ?? P�blico - Logout n�o precisa de token v�lido
        public ActionResult Logout()
        {
            return Ok(new { message = "Logout realizado com sucesso" });
        }

        /// <summary>
        /// Verifica se o token est� v�lido
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
                user = new
                {
                    id = User.FindFirst("sub")?.Value,
                    email = User.FindFirst("email")?.Value,
                    name = User.FindFirst("name")?.Value
                }
            });
        }

        /// <summary>
        /// Reset de senha de emerg�ncia (APENAS DESENVOLVIMENTO)
        /// </summary>
        /// <remarks>
        /// **?? ENDPOINT DE EMERG�NCIA** - Recria usu�rio admin padr�o
        /// 
        /// Use apenas se perdeu acesso ao sistema!
        /// </remarks>
        [HttpPost("reset-admin")]
        [AllowAnonymous] // ?? P�blico apenas para emerg�ncia
        public async Task<ActionResult<object>> ResetAdmin()
        {
            var ambiente = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            
            if (ambiente != "Development")
            {
                return BadRequest(new { message = "Endpoint dispon�vel apenas em ambiente de desenvolvimento" });
            }

            try
            {
                var resetUsuario = new CriarUsuarioComando
                {
                    Nome = "Admin Reset",
                    Email = "admin.reset@exemplo.com",
                    Senha = "AdminReset@123"
                };

                await mediator.Send(resetUsuario);

                return Ok(new 
                { 
                    message = "Usu�rio admin de emerg�ncia criado",
                    credentials = new
                    {
                        email = "admin.reset@exemplo.com",
                        password = "AdminReset@123"
                    },
                    warning = "Use estas credenciais apenas para recuperar acesso!"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new 
                { 
                    message = "Erro ao criar usu�rio de emerg�ncia",
                    error = ex.Message
                });
            }
        }
    }
}