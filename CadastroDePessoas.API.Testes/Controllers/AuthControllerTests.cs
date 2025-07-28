using CadastroDePessoas.API.Controllers;
using CadastroDePessoas.Application.CQRS.Comandos.Usuario.AutenticarUsuario;
using CadastroDePessoas.Application.CQRS.Comandos.Usuario.CriarUsuario;
using CadastroDePessoas.Domain.Entidades;
using CadastroDePessoas.Infraestructure.Contexto;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Security.Claims;
using Xunit;

namespace CadastroDePessoas.API.Testes.Controllers
{
    public class AuthControllerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<AppDbContexto> _dbContextMock;
        private readonly AuthController _controller;
        private readonly Mock<DbSet<Usuario>> _usuariosDbSetMock;

        public AuthControllerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            
            var options = new DbContextOptionsBuilder<AppDbContexto>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            
            _dbContextMock = new Mock<AppDbContexto>(options);
            _usuariosDbSetMock = new Mock<DbSet<Usuario>>();
            _dbContextMock.Setup(c => c.Usuarios).Returns(_usuariosDbSetMock.Object);
            
            _controller = new AuthController(_mediatorMock.Object, _dbContextMock.Object);
        }

        [Fact]
        public async Task Login_ComCredenciaisValidas_DeveRetornarOk()
        {
            // Arrange
            var comando = new AutenticarUsuarioComando
            {
                Email = "joao@exemplo.com",
                Senha = "Senha123"
            };

            var usuarioId = Guid.NewGuid();
            var token = "token-jwt-valido";

            _mediatorMock
                .Setup(m => m.Send(comando, It.IsAny<CancellationToken>()))
                .ReturnsAsync(token);

            var usuario = new Usuario("João Silva", "joao@exemplo.com", "senha-hash");
            var usuarios = new List<Usuario> { usuario }.AsQueryable();
            
            _usuariosDbSetMock.As<IQueryable<Usuario>>().Setup(m => m.Provider).Returns(usuarios.Provider);
            _usuariosDbSetMock.As<IQueryable<Usuario>>().Setup(m => m.Expression).Returns(usuarios.Expression);
            _usuariosDbSetMock.As<IQueryable<Usuario>>().Setup(m => m.ElementType).Returns(usuarios.ElementType);
            _usuariosDbSetMock.As<IQueryable<Usuario>>().Setup(m => m.GetEnumerator()).Returns(usuarios.GetEnumerator);

            // Act
            var resultado = await _controller.Login(comando);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(resultado.Result);
            var response = Assert.IsAssignableFrom<object>(okResult.Value);
            
            // Verificar propriedades dinâmicas
            var properties = response.GetType().GetProperties();
            Assert.Contains(properties, p => p.Name == "success");
            Assert.Contains(properties, p => p.Name == "token");
            Assert.Contains(properties, p => p.Name == "user");

            var successProperty = properties.First(p => p.Name == "success");
            var tokenProperty = properties.First(p => p.Name == "token");
            
            Assert.Equal(true, successProperty.GetValue(response));
            Assert.Equal(token, tokenProperty.GetValue(response));
        }

        [Fact]
        public async Task Login_ComUsuarioNulo_DeveRetornarUnauthorized()
        {
            // Arrange
            var comando = new AutenticarUsuarioComando
            {
                Email = "naoexiste@exemplo.com",
                Senha = "Senha123"
            };

            var token = "token-jwt-valido";

            _mediatorMock
                .Setup(m => m.Send(comando, It.IsAny<CancellationToken>()))
                .ReturnsAsync(token);

            var usuarios = new List<Usuario>().AsQueryable();
            
            _usuariosDbSetMock.As<IQueryable<Usuario>>().Setup(m => m.Provider).Returns(usuarios.Provider);
            _usuariosDbSetMock.As<IQueryable<Usuario>>().Setup(m => m.Expression).Returns(usuarios.Expression);
            _usuariosDbSetMock.As<IQueryable<Usuario>>().Setup(m => m.ElementType).Returns(usuarios.ElementType);
            _usuariosDbSetMock.As<IQueryable<Usuario>>().Setup(m => m.GetEnumerator()).Returns(usuarios.GetEnumerator);

            // Act
            var resultado = await _controller.Login(comando);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(resultado.Result);
            var response = Assert.IsAssignableFrom<object>(unauthorizedResult.Value);
            
            var properties = response.GetType().GetProperties();
            Assert.Contains(properties, p => p.Name == "success");
            Assert.Contains(properties, p => p.Name == "message");

            var successProperty = properties.First(p => p.Name == "success");
            Assert.Equal(false, successProperty.GetValue(response));
        }

        [Fact]
        public async Task Login_ComCredenciaisInvalidas_DeveRetornarUnauthorized()
        {
            // Arrange
            var comando = new AutenticarUsuarioComando
            {
                Email = "joao@exemplo.com",
                Senha = "SenhaErrada"
            };

            _mediatorMock
                .Setup(m => m.Send(comando, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Credenciais inválidas"));

            // Act
            var resultado = await _controller.Login(comando);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(resultado.Result);
            var response = Assert.IsAssignableFrom<object>(unauthorizedResult.Value);
            
            var properties = response.GetType().GetProperties();
            Assert.Contains(properties, p => p.Name == "success");
            Assert.Contains(properties, p => p.Name == "message");

            var successProperty = properties.First(p => p.Name == "success");
            Assert.Equal(false, successProperty.GetValue(response));
        }

        [Fact]
        public async Task Register_ComDadosValidos_DeveRetornarOk()
        {
            // Arrange
            var comando = new CriarUsuarioComando
            {
                Nome = "João Silva",
                Email = "joao@exemplo.com",
                Senha = "Senha123"
            };

            _mediatorMock
                .Setup(m => m.Send(comando, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var resultado = await _controller.Register(comando);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(resultado.Result);
            var response = Assert.IsAssignableFrom<object>(okResult.Value);
            
            var properties = response.GetType().GetProperties();
            Assert.Contains(properties, p => p.Name == "success");
            Assert.Contains(properties, p => p.Name == "message");

            var successProperty = properties.First(p => p.Name == "success");
            Assert.Equal(true, successProperty.GetValue(response));
        }

        [Fact]
        public async Task Register_ComErro_DeveRetornarBadRequest()
        {
            // Arrange
            var comando = new CriarUsuarioComando
            {
                Nome = "João Silva",
                Email = "joao@exemplo.com",
                Senha = "Senha123"
            };

            _mediatorMock
                .Setup(m => m.Send(comando, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("E-mail já existe"));

            // Act
            var resultado = await _controller.Register(comando);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(resultado.Result);
            var response = Assert.IsAssignableFrom<object>(badRequestResult.Value);
            
            var properties = response.GetType().GetProperties();
            Assert.Contains(properties, p => p.Name == "success");
            Assert.Contains(properties, p => p.Name == "message");

            var successProperty = properties.First(p => p.Name == "success");
            Assert.Equal(false, successProperty.GetValue(response));
        }

        [Fact]
        public void Logout_DeveRetornarOk()
        {
            // Act
            var resultado = _controller.Logout();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(resultado);
            var response = Assert.IsAssignableFrom<object>(okResult.Value);
            
            var properties = response.GetType().GetProperties();
            Assert.Contains(properties, p => p.Name == "message");
        }

        [Fact]
        public void VerifyToken_ComTokenValido_DeveRetornarOk()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var claims = new List<Claim>
            {
                new Claim("sub", userId),
                new Claim("email", "joao@exemplo.com"),
                new Claim("name", "João Silva")
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var principal = new ClaimsPrincipal(identity);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = principal
                }
            };

            // Act
            var resultado = _controller.VerifyToken();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(resultado);
            var response = Assert.IsAssignableFrom<object>(okResult.Value);
            
            var properties = response.GetType().GetProperties();
            Assert.Contains(properties, p => p.Name == "valid");
            Assert.Contains(properties, p => p.Name == "user");

            var validProperty = properties.First(p => p.Name == "valid");
            Assert.Equal(true, validProperty.GetValue(response));
        }

        [Fact]
        public async Task GetProfile_ComUsuarioValido_DeveRetornarOk()
        {
            // Arrange
            var userId = Guid.NewGuid();
            ConfigurarUsuarioNaRequisicao(userId.ToString());

            // Criar usuário usando o construtor para definir propriedades privadas
            var usuario = new Usuario("João Silva", "joao@exemplo.com", "senha-hash");
            
            var usuariosMock = new List<Usuario> { usuario }.AsQueryable();
            _usuariosDbSetMock.As<IQueryable<Usuario>>().Setup(m => m.Provider).Returns(usuariosMock.Provider);
            _usuariosDbSetMock.As<IQueryable<Usuario>>().Setup(m => m.Expression).Returns(usuariosMock.Expression);
            _usuariosDbSetMock.As<IQueryable<Usuario>>().Setup(m => m.ElementType).Returns(usuariosMock.ElementType);
            _usuariosDbSetMock.As<IQueryable<Usuario>>().Setup(m => m.GetEnumerator()).Returns(usuariosMock.GetEnumerator);

            // Act
            var resultado = await _controller.GetProfile();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(resultado.Result);
            var response = Assert.IsAssignableFrom<object>(okResult.Value);
            
            var properties = response.GetType().GetProperties();
            Assert.Contains(properties, p => p.Name == "success");
            Assert.Contains(properties, p => p.Name == "user");

            var successProperty = properties.First(p => p.Name == "success");
            Assert.Equal(true, successProperty.GetValue(response));

            var userProperty = properties.First(p => p.Name == "user");
            var user = userProperty.GetValue(response);
            Assert.NotNull(user);
        }

        [Fact]
        public async Task GetProfile_ComUsuarioNaoEncontrado_DeveRetornarNotFound()
        {
            // Arrange
            var userId = Guid.NewGuid();
            ConfigurarUsuarioNaRequisicao(userId.ToString());

            var usuariosMock = new List<Usuario>().AsQueryable();
            _usuariosDbSetMock.As<IQueryable<Usuario>>().Setup(m => m.Provider).Returns(usuariosMock.Provider);
            _usuariosDbSetMock.As<IQueryable<Usuario>>().Setup(m => m.Expression).Returns(usuariosMock.Expression);
            _usuariosDbSetMock.As<IQueryable<Usuario>>().Setup(m => m.ElementType).Returns(usuariosMock.ElementType);
            _usuariosDbSetMock.As<IQueryable<Usuario>>().Setup(m => m.GetEnumerator()).Returns(usuariosMock.GetEnumerator);

            // Act
            var resultado = await _controller.GetProfile();

            // Assert
            Assert.IsType<NotFoundObjectResult>(resultado.Result);
        }

        [Fact]
        public async Task GetProfile_ComIdInvalido_DeveRetornarBadRequest()
        {
            // Arrange
            ConfigurarUsuarioNaRequisicao("id-invalido");

            // Act
            var resultado = await _controller.GetProfile();

            // Assert
            Assert.IsType<BadRequestObjectResult>(resultado.Result);
        }

        [Fact]
        public async Task UpdateProfile_ComDadosValidos_DeveRetornarOk()
        {
            // Arrange
            var userId = Guid.NewGuid();
            ConfigurarUsuarioNaRequisicao(userId.ToString());

            var usuario = new Usuario("João Silva", "joao@exemplo.com", "senha-hash");

            _dbContextMock.Setup(c => c.Usuarios.FindAsync(userId))
                .ReturnsAsync(usuario);

            _dbContextMock.Setup(c => c.Usuarios.AnyAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<Usuario, bool>>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            _dbContextMock.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            var request = new UpdateProfileRequest
            {
                Nome = "João Silva Atualizado",
                Email = "joao.atualizado@exemplo.com"
            };

            // Act
            var resultado = await _controller.UpdateProfile(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(resultado.Result);
            var response = Assert.IsAssignableFrom<object>(okResult.Value);
            
            var properties = response.GetType().GetProperties();
            Assert.Contains(properties, p => p.Name == "success");
            Assert.Contains(properties, p => p.Name == "message");

            var successProperty = properties.First(p => p.Name == "success");
            Assert.Equal(true, successProperty.GetValue(response));
        }

        [Fact]
        public async Task UpdateProfile_ComEmailJaExistente_DeveRetornarBadRequest()
        {
            // Arrange
            var userId = Guid.NewGuid();
            ConfigurarUsuarioNaRequisicao(userId.ToString());

            var usuario = new Usuario("João Silva", "joao@exemplo.com", "senha-hash");

            _dbContextMock.Setup(c => c.Usuarios.FindAsync(userId))
                .ReturnsAsync(usuario);

            _dbContextMock.Setup(c => c.Usuarios.AnyAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<Usuario, bool>>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var request = new UpdateProfileRequest
            {
                Nome = "João Silva Atualizado",
                Email = "outro@exemplo.com"
            };

            // Act
            var resultado = await _controller.UpdateProfile(request);

            // Assert
            Assert.IsType<BadRequestObjectResult>(resultado.Result);
        }

        [Fact]
        public async Task UpdateProfile_ComUsuarioNaoEncontrado_DeveRetornarNotFound()
        {
            // Arrange
            var userId = Guid.NewGuid();
            ConfigurarUsuarioNaRequisicao(userId.ToString());

            _dbContextMock.Setup(c => c.Usuarios.FindAsync(userId))
                .ReturnsAsync((Usuario?)null);

            var request = new UpdateProfileRequest
            {
                Nome = "João Silva Atualizado",
                Email = "joao.atualizado@exemplo.com"
            };

            // Act
            var resultado = await _controller.UpdateProfile(request);

            // Assert
            Assert.IsType<NotFoundObjectResult>(resultado.Result);
        }

        [Fact]
        public async Task ChangePassword_ComDadosValidos_DeveRetornarOk()
        {
            // Arrange
            var userId = Guid.NewGuid();
            ConfigurarUsuarioNaRequisicao(userId.ToString());

            var senhaAtual = BCrypt.Net.BCrypt.HashPassword("senha123", 12);
            var usuario = new Usuario("João Silva", "joao@exemplo.com", senhaAtual);

            _dbContextMock.Setup(c => c.Usuarios.FindAsync(userId))
                .ReturnsAsync(usuario);

            _dbContextMock.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            var request = new ChangePasswordRequest
            {
                SenhaAtual = "senha123",
                NovaSenha = "novaSenha123",
                ConfirmarSenha = "novaSenha123"
            };

            // Act
            var resultado = await _controller.ChangePassword(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(resultado.Result);
            var response = Assert.IsAssignableFrom<object>(okResult.Value);
            
            var properties = response.GetType().GetProperties();
            Assert.Contains(properties, p => p.Name == "success");
            Assert.Contains(properties, p => p.Name == "message");

            var successProperty = properties.First(p => p.Name == "success");
            Assert.Equal(true, successProperty.GetValue(response));
        }

        [Fact]
        public async Task ChangePassword_ComSenhaAtualIncorreta_DeveRetornarBadRequest()
        {
            // Arrange
            var userId = Guid.NewGuid();
            ConfigurarUsuarioNaRequisicao(userId.ToString());

            var senhaAtual = BCrypt.Net.BCrypt.HashPassword("senha123", 12);
            var usuario = new Usuario("João Silva", "joao@exemplo.com", senhaAtual);

            _dbContextMock.Setup(c => c.Usuarios.FindAsync(userId))
                .ReturnsAsync(usuario);

            var request = new ChangePasswordRequest
            {
                SenhaAtual = "senha_errada",
                NovaSenha = "novaSenha123",
                ConfirmarSenha = "novaSenha123"
            };

            // Act
            var resultado = await _controller.ChangePassword(request);

            // Assert
            Assert.IsType<BadRequestObjectResult>(resultado.Result);
        }

        [Fact]
        public async Task ChangePassword_ComSenhaFraca_DeveRetornarBadRequest()
        {
            // Arrange
            var userId = Guid.NewGuid();
            ConfigurarUsuarioNaRequisicao(userId.ToString());

            var senhaAtual = BCrypt.Net.BCrypt.HashPassword("senha123", 12);
            var usuario = new Usuario("João Silva", "joao@exemplo.com", senhaAtual);

            _dbContextMock.Setup(c => c.Usuarios.FindAsync(userId))
                .ReturnsAsync(usuario);

            var request = new ChangePasswordRequest
            {
                SenhaAtual = "senha123",
                NovaSenha = "123",
                ConfirmarSenha = "123"
            };

            // Act
            var resultado = await _controller.ChangePassword(request);

            // Assert
            Assert.IsType<BadRequestObjectResult>(resultado.Result);
        }

        [Fact]
        public async Task ChangePassword_ComSenhasNaoCoincidentes_DeveRetornarBadRequest()
        {
            // Arrange
            var userId = Guid.NewGuid();
            ConfigurarUsuarioNaRequisicao(userId.ToString());

            var senhaAtual = BCrypt.Net.BCrypt.HashPassword("senha123", 12);
            var usuario = new Usuario("João Silva", "joao@exemplo.com", senhaAtual);

            _dbContextMock.Setup(c => c.Usuarios.FindAsync(userId))
                .ReturnsAsync(usuario);

            var request = new ChangePasswordRequest
            {
                SenhaAtual = "senha123",
                NovaSenha = "novaSenha123",
                ConfirmarSenha = "outraSenha123"
            };

            // Act
            var resultado = await _controller.ChangePassword(request);

            // Assert
            Assert.IsType<BadRequestObjectResult>(resultado.Result);
        }

        private void ConfigurarUsuarioNaRequisicao(string userId)
        {
            var claims = new List<Claim>
            {
                new Claim("sub", userId),
                new Claim("email", "joao@exemplo.com"),
                new Claim("name", "João Silva")
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var principal = new ClaimsPrincipal(identity);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = principal
                }
            };
        }
    }
}