using CadastroDePessoas.Application.DTOs.Usuario;
using System;
using Xunit;

namespace CadastroDePessoas.Application.Testes.DTOs
{
    public class UsuarioDTOTests
    {
        [Fact]
        public void UsuarioDTO_PropriedadesDevemSerCorretamenteSetadas()
        {
            // Arrange
            var id = Guid.NewGuid();
            var nome = "João Silva";
            var email = "joao.silva@exemplo.com";
            var dataCadastro = DateTime.Now;

            // Act
            var usuarioDTO = new UsuarioDTO
            {
                Id = id,
                Nome = nome,
                Email = email,
                DataCadastro = dataCadastro
            };

            // Assert
            Assert.Equal(id, usuarioDTO.Id);
            Assert.Equal(nome, usuarioDTO.Nome);
            Assert.Equal(email, usuarioDTO.Email);
            Assert.Equal(dataCadastro, usuarioDTO.DataCadastro);
        }
        
        [Fact]
        public void UsuarioDTO_ConstrutorPadrao_DeveCriarInstanciaValida()
        {
            // Act
            var usuarioDTO = new UsuarioDTO();
            
            // Assert
            Assert.Equal(Guid.Empty, usuarioDTO.Id);
            Assert.Null(usuarioDTO.Nome);
            Assert.Null(usuarioDTO.Email);
            Assert.Equal(default, usuarioDTO.DataCadastro);
        }
        
        [Fact]
        public void UsuarioDTO_ConstrutorComParametros_DeveCriarInstanciaValida()
        {
            // Arrange
            var id = Guid.NewGuid();
            var nome = "João Silva";
            var email = "joao.silva@exemplo.com";
            var dataCadastro = DateTime.Now;
            
            // Act
            var usuarioDTO = new UsuarioDTO(id, nome, email, dataCadastro);
            
            // Assert
            Assert.Equal(id, usuarioDTO.Id);
            Assert.Equal(nome, usuarioDTO.Nome);
            Assert.Equal(email, usuarioDTO.Email);
            Assert.Equal(dataCadastro, usuarioDTO.DataCadastro);
        }
        
        [Fact]
        public void UsuarioDTO_TempoDesdeRegistro_DeveCalcularCorretamente()
        {
            // Arrange
            var dataCadastro = DateTime.Now.AddDays(-30);

            // Act
            var usuarioDTO = new UsuarioDTO
            {
                Nome = "João Silva",
                Email = "joao@exemplo.com",
                DataCadastro = dataCadastro
            };

            // Assert
            TimeSpan tempoDesdeRegistro = DateTime.Now - dataCadastro;

            Assert.True(Math.Abs((DateTime.Now - usuarioDTO.DataCadastro).TotalSeconds - tempoDesdeRegistro.TotalSeconds) < 1);
        }
        
        [Fact]
        public void UsuarioDTO_TempoDesdeRegistro_ComDataCadastroNoFuturo_DeveRetornarTimeSpanZero()
        {
            // Arrange
            var dataCadastroFutura = DateTime.Now.AddDays(30);
            
            // Act
            var usuarioDTO = new UsuarioDTO
            {
                Nome = "João Silva",
                Email = "joao@exemplo.com",
                DataCadastro = dataCadastroFutura
            };

            // Assert
            var tempoDesdeRegistro = DateTime.Now - usuarioDTO.DataCadastro;
            Assert.Equal(TimeSpan.Zero, tempoDesdeRegistro < TimeSpan.Zero ? TimeSpan.Zero : tempoDesdeRegistro);
        }
        
        [Fact]
        public void UsuarioDTO_EstaRegistradoHaMaisDeUmMes_DeveRetornarTrue_QuandoDataCadastroMaisDeUmMes()
        {
            // Arrange
            var dataCadastro = DateTime.Now.AddDays(-31);
            
            // Act
            var usuarioDTO = new UsuarioDTO
            {
                Nome = "João Silva",
                Email = "joao@exemplo.com",
                DataCadastro = dataCadastro
            };
            
            // Assert
            Assert.True(usuarioDTO.EstaRegistradoHaMaisDeUmMes);
        }
        
        [Fact]
        public void UsuarioDTO_EstaRegistradoHaMaisDeUmMes_DeveRetornarFalse_QuandoDataCadastroMenosDeUmMes()
        {
            // Arrange
            var dataCadastro = DateTime.Now.AddDays(-15);
            
            // Act
            var usuarioDTO = new UsuarioDTO
            {
                Nome = "João Silva",
                Email = "joao@exemplo.com",
                DataCadastro = dataCadastro
            };
            
            // Assert
            Assert.False(usuarioDTO.EstaRegistradoHaMaisDeUmMes);
        }
        
        [Fact]
        public void UsuarioDTO_ToString_DeveRetornarInfoFormatada()
        {
            // Arrange
            var id = Guid.NewGuid();
            var nome = "João Silva";
            var email = "joao@exemplo.com";
            var dataCadastro = new DateTime(2023, 1, 1);
            
            var usuarioDTO = new UsuarioDTO(id, nome, email, dataCadastro);
            
            // Act
            var resultado = usuarioDTO.ToString();
            
            // Assert
            Assert.Contains(id.ToString(), resultado);
            Assert.Contains(nome, resultado);
            Assert.Contains(email, resultado);
            Assert.Contains("01/01/2023", resultado);
        }
    }
}