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
    [Tags("?? Autenticação")]
    public class AuthController(IMediator mediator, AppDbContexto dbContext) : ControllerBase
    {
        /// <summary>
        /// Realiza login do usuário
        /// </summary>
        /// <param name="loginDto">Dados de login</param>
        /// <returns>Token JWT e dados do usuário</returns>
        /// <remarks>
        /// **?? Endpoint PÚBLICO** - Para autenticação inicial
        /// 
        /// **Usuários padrão para teste:**
        /// - admin@exemplo.com / Admin@123
        /// - user@teste.com / User@123
        /// </remarks>
        [HttpPost("login")]
        [AllowAnonymous] // ?? Público - Login precisa ser acessível sem token
        public async Task<ActionResult<object>> Login([FromBody] AutenticarUsuarioComando loginDto)
        {
            try
            {
                var token = await mediator.Send(loginDto);
                
                // Buscar dados do usuário para retornar
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
        /// <param name="criarUsuarioDto">Dados do usuário</param>
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
        /// Obtém dados do perfil do usuário autenticado
        /// </summary>
        /// <remarks>
        /// **?? Endpoint PROTEGIDO** - Requer autenticação JWT
        /// </remarks>
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
                        ultimoAcesso = DateTime.UtcNow // Pode ser implementado tracking real
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
        /// <remarks>
        /// **?? Endpoint PROTEGIDO** - Requer autenticação JWT
        /// </remarks>
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

                // Verificar se email já existe para outro usuário
                if (await dbContext.Usuarios.AnyAsync(u => u.Email == request.Email && u.Id != userGuid))
                {
                    return BadRequest(new { message = "E-mail já está em uso por outro usuário" });
                }

                // Usar o método da entidade para atualizar
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
                    message = "Admin de emergência criado com sucesso",
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
                    message = "Erro ao criar admin de emergência",
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