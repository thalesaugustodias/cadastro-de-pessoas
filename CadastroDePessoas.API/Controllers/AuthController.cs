using CadastroDePessoas.Application.CQRS.Comandos.Usuario.AutenticarUsuario;
using CadastroDePessoas.Application.CQRS.Comandos.Usuario.CriarUsuario;
using CadastroDePessoas.Infraestructure.Contexto;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CadastroDePessoas.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class AuthController(IMediator mediator, AppDbContexto dbContext) : ControllerBase
    {
        /// <summary>
        /// Realiza login do usuário
        /// </summary>       
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<object>> Login([FromBody] AutenticarUsuarioComando loginDto)
        {
            try
            {
                var token = await mediator.Send(loginDto);
                
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
                        message = "Usuário não encontrado"
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
        /// Cria novo usuário no sistema
        /// </summary>   
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult<object>> Register([FromBody] CriarUsuarioComando criarUsuarioDto)
        {
            try
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
        [HttpPost("logout")]
        [AllowAnonymous]
        public ActionResult Logout()
        {
            return Ok(new { message = "Logout realizado com sucesso" });
        }

        /// <summary>
        /// Verifica se o token está válido
        /// </summary>
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

        /// <summary>
        /// Obtém dados do perfil do usuário autenticado
        /// </summary>
        [HttpGet("profile")]
        [Authorize]
        public async Task<ActionResult<object>> GetProfile()
        {
            var userId = User.FindFirst("sub")?.Value;
            
            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var userGuid))
            {
                return BadRequest(new { message = "Token inválido" });
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
                        ultimoAcesso = DateTime.UtcNow
                    })
                    .FirstOrDefaultAsync();

                if (usuario == null)
                {
                    return NotFound(new { message = "Usuário não encontrado" });
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
        /// Atualiza dados do perfil do usuário
        /// </summary>
        [HttpPut("profile")]
        [Authorize]
        public async Task<ActionResult<object>> UpdateProfile([FromBody] UpdateProfileRequest request)
        {
            var userId = User.FindFirst("sub")?.Value;
            
            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var userGuid))
            {
                return BadRequest(new { message = "Token inválido" });
            }

            try
            {
                var usuario = await dbContext.Usuarios.FindAsync(userGuid);
                
                if (usuario == null)
                {
                    return NotFound(new { message = "Usuário não encontrado" });
                }

                if (await dbContext.Usuarios.AnyAsync(u => u.Email == request.Email && u.Id != userGuid))
                {
                    return BadRequest(new { message = "E-mail já está em uso por outro usuário" });
                }

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
    }

    public class UpdateProfileRequest
    {
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}