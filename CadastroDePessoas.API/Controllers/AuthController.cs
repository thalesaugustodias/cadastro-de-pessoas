using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CadastroDePessoas.Application.CQRS.Comandos.Usuario.AutenticarUsuario;
using CadastroDePessoas.Application.CQRS.Comandos.Usuario.CriarUsuario;
using CadastroDePessoas.Application.DTOs.Usuario;
using Microsoft.EntityFrameworkCore;
using CadastroDePessoas.Infraestructure.Contexto;

namespace CadastroDePessoas.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Tags("?? Autentica��o")]
    public class AuthController(IMediator mediator, AppDbContexto dbContext) : ControllerBase
    {
        /// <summary>
        /// Realiza login do usu�rio
        /// </summary>
        /// <param name="loginDto">Dados de login</param>
        /// <returns>Token JWT e dados do usu�rio</returns>
        /// <remarks>
        /// **?? Endpoint P�BLICO** - Para autentica��o inicial
        /// 
        /// **Usu�rios padr�o para teste:**
        /// - admin@exemplo.com / Admin@123
        /// - user@teste.com / User@123
        /// </remarks>
        [HttpPost("login")]
        [AllowAnonymous] // ?? P�blico - Login precisa ser acess�vel sem token
        public async Task<ActionResult<object>> Login([FromBody] AutenticarUsuarioComando loginDto)
        {
            try
            {
                var token = await mediator.Send(loginDto);
                
                // Buscar dados do usu�rio para retornar
                var usuario = await dbContext.Usuarios
                    .Where(u => u.Email == loginDto.Email)
                    .Select(u => new
                    {
                        u.Id,
                        u.Nome,
                        u.Email
                    })
                    .FirstOrDefaultAsync();

                if (usuario == null)
                {
                    return Unauthorized(new
                    {
                        success = false,
                        message = "Usu�rio n�o encontrado"
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = "Login realizado com sucesso",
                    token = token,
                    user = new
                    {
                        id = usuario.Id,
                        nome = usuario.Nome,
                        email = usuario.Email
                    }
                });
            }
            catch (Exception ex)
            {
                return Unauthorized(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        /// <summary>
        /// Cria novo usu�rio no sistema
        /// </summary>
        /// <param name="criarUsuarioDto">Dados do usu�rio</param>
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
            try
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
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
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
        /// Obt�m dados do perfil do usu�rio autenticado
        /// </summary>
        /// <remarks>
        /// **?? Endpoint PROTEGIDO** - Requer autentica��o JWT
        /// </remarks>
        [HttpGet("profile")]
        [Authorize]
        public async Task<ActionResult<object>> GetProfile()
        {
            var userId = User.FindFirst("sub")?.Value;
            
            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var userGuid))
            {
                return BadRequest(new { message = "Token inv�lido" });
            }

            try
            {
                var usuario = await dbContext.Usuarios
                    .Where(u => u.Id == userGuid)
                    .Select(u => new
                    {
                        id = u.Id,
                        nome = u.Nome,
                        email = u.Email,
                        dataCadastro = u.DataCadastro,
                        ultimoAcesso = DateTime.UtcNow // Pode ser implementado tracking real
                    })
                    .FirstOrDefaultAsync();

                if (usuario == null)
                {
                    return NotFound(new { message = "Usu�rio n�o encontrado" });
                }

                return Ok(new
                {
                    success = true,
                    user = usuario
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }

        /// <summary>
        /// Atualiza dados do perfil do usu�rio
        /// </summary>
        /// <remarks>
        /// **?? Endpoint PROTEGIDO** - Requer autentica��o JWT
        /// </remarks>
        [HttpPut("profile")]
        [Authorize]
        public async Task<ActionResult<object>> UpdateProfile([FromBody] UpdateProfileRequest request)
        {
            var userId = User.FindFirst("sub")?.Value;
            
            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var userGuid))
            {
                return BadRequest(new { message = "Token inv�lido" });
            }

            try
            {
                var usuario = await dbContext.Usuarios.FindAsync(userGuid);
                
                if (usuario == null)
                {
                    return NotFound(new { message = "Usu�rio n�o encontrado" });
                }

                // Verificar se email j� existe para outro usu�rio
                if (await dbContext.Usuarios.AnyAsync(u => u.Email == request.Email && u.Id != userGuid))
                {
                    return BadRequest(new { message = "E-mail j� est� em uso por outro usu�rio" });
                }

                // Usar o m�todo da entidade para atualizar
                usuario.AtualizarPerfil(request.Nome, request.Email);
                
                await dbContext.SaveChangesAsync();

                return Ok(new
                {
                    success = true,
                    message = "Perfil atualizado com sucesso",
                    user = new
                    {
                        id = usuario.Id,
                        nome = usuario.Nome,
                        email = usuario.Email,
                        dataCadastro = usuario.DataCadastro
                    }
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
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
                    message = "Admin de emerg�ncia criado com sucesso",
                    credentials = new
                    {
                        email = resetUsuario.Email,
                        password = resetUsuario.Senha
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Erro ao criar admin de emerg�ncia",
                    error = ex.Message
                });
            }
        }
    }

    public class UpdateProfileRequest
    {
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}