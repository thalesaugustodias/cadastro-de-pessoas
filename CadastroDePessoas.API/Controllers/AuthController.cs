using CadastroDePessoas.Application.CQRS.Comandos.Usuario.AutenticarUsuario;
using CadastroDePessoas.Application.CQRS.Comandos.Usuario.CriarUsuario;
using CadastroDePessoas.Infraestructure.Contexto;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using BC = BCrypt.Net.BCrypt;

namespace CadastroDePessoas.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class AuthController(IMediator mediator, AppDbContexto dbContext) : ControllerBase
    {   
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
                    token,
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
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("sub")?.Value;
            var email = User.FindFirst(ClaimTypes.Email)?.Value ?? User.FindFirst("email")?.Value;
            var name = User.FindFirst(ClaimTypes.Name)?.Value ?? User.FindFirst("name")?.Value;

            return Ok(new 
            { 
                valid = true,
                user = new
                {
                    id = userId,
                    email,
                    name
                }
            });
        }

        [HttpGet("profile")]
        [Authorize]
        public async Task<ActionResult<object>> GetProfile()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("sub")?.Value;
            
            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var userGuid))
            {
                // Log de debug para ajudar a identificar o problema
                var availableClaims = User.Claims.Select(c => $"{c.Type}: {c.Value}").ToList();
                var claimsInfo = string.Join(", ", availableClaims);
                
                return BadRequest(new { 
                    message = "Token inválido ou sem identificador de usuário",
                    debug = $"Claims disponíveis: {claimsInfo}"
                });
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
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", detail = ex.Message });
            }
        }

        [HttpPut("profile")]
        [Authorize]
        public async Task<ActionResult<object>> UpdateProfile([FromBody] UpdateProfileRequest request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("sub")?.Value;
            
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
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", detail = ex.Message });
            }
        }

        [HttpPut("password")]
        [Authorize]
        public async Task<ActionResult<object>> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("sub")?.Value;
            
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

                if (!BC.Verify(request.SenhaAtual, usuario.Senha))
                {
                    return BadRequest(new { message = "Senha atual incorreta" });
                }

                if (request.NovaSenha.Length < 6)
                {
                    return BadRequest(new { message = "A nova senha deve ter pelo menos 6 caracteres" });
                }

                if (request.NovaSenha != request.ConfirmarSenha)
                {
                    return BadRequest(new { message = "A nova senha e a confirmação não coincidem" });
                }

                usuario.AlterarSenha(BC.HashPassword(request.NovaSenha, 12));
                
                await dbContext.SaveChangesAsync();

                return Ok(new
                {
                    success = true,
                    message = "Senha alterada com sucesso"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Erro interno do servidor: {ex.Message}" });
            }
        }
    }
}