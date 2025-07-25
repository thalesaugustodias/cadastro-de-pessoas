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
    [Tags("?? Autenticação V1")]
    public class AuthController(IMediator mediator) : ControllerBase
    {
        /// <summary>
        /// Realiza login do usuário
        /// </summary>
        /// <param name="loginDto">Dados de login (email e senha)</param>
        /// <returns>Token JWT para autenticação</returns>
        /// <remarks>
        /// **?? Endpoint PÚBLICO** - Não requer autenticação
        /// 
        /// **Usuários padrão para teste:**
        /// - Email: admin@exemplo.com / Senha: Admin@123
        /// - Email: user@teste.com / Senha: User@123
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
                expiresIn = "1h"
            });
        }

        /// <summary>
        /// Cria um novo usuário no sistema
        /// </summary>
        /// <param name="criarUsuarioDto">Dados do novo usuário</param>
        /// <returns>Confirmação de criação</returns>
        /// <remarks>
        /// **?? Endpoint PÚBLICO TEMPORÁRIO** - Para facilitar testes iniciais
        /// 
        /// **Regras de senha:**
        /// - Mínimo 6 caracteres
        /// - Pelo menos 1 letra minúscula
        /// - Pelo menos 1 letra maiúscula  
        /// - Pelo menos 1 número
        /// - Pelo menos 1 caractere especial (@$!%*?&)
        /// 
        /// **Exemplo de senha válida:** MinhaSenh@123
        /// </remarks>
        [HttpPost("register")]
        [AllowAnonymous] // ?? Temporariamente público para facilitar testes
        public async Task<ActionResult<object>> Register([FromBody] CriarUsuarioComando criarUsuarioDto)
        {
            var sucesso = await mediator.Send(criarUsuarioDto);

            return Ok(new 
            { 
                success = sucesso,
                message = "Usuário criado com sucesso",
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
        /// **?? Endpoint PÚBLICO** - Logout pode ser feito sem token válido
        /// </remarks>
        [HttpPost("logout")]
        [AllowAnonymous] // ?? Público - Logout não precisa de token válido
        public ActionResult Logout()
        {
            return Ok(new { message = "Logout realizado com sucesso" });
        }

        /// <summary>
        /// Verifica se o token está válido
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
                user = new
                {
                    id = User.FindFirst("sub")?.Value,
                    email = User.FindFirst("email")?.Value,
                    name = User.FindFirst("name")?.Value
                }
            });
        }

        /// <summary>
        /// Reset de senha de emergência (APENAS DESENVOLVIMENTO)
        /// </summary>
        /// <remarks>
        /// **?? ENDPOINT DE EMERGÊNCIA** - Recria usuário admin padrão
        /// 
        /// Use apenas se perdeu acesso ao sistema!
        /// </remarks>
        [HttpPost("reset-admin")]
        [AllowAnonymous] // ?? Público apenas para emergência
        public async Task<ActionResult<object>> ResetAdmin()
        {
            var ambiente = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            
            if (ambiente != "Development")
            {
                return BadRequest(new { message = "Endpoint disponível apenas em ambiente de desenvolvimento" });
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
                    message = "Usuário admin de emergência criado",
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
                    message = "Erro ao criar usuário de emergência",
                    error = ex.Message
                });
            }
        }
    }
}