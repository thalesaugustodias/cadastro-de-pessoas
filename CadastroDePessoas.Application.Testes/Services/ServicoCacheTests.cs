using CadastroDePessoas.Application.Services;
using Microsoft.Extensions.Caching.Distributed;
using Moq;
using System.Text.Json;
using Xunit;

namespace CadastroDePessoas.Application.Testes.Services
{
    public class ServicoCacheTests
    {
        private readonly Mock<IDistributedCache> _cacheMock;
        private readonly ServicoCache _servicoCache;

        public ServicoCacheTests()
        {
            _cacheMock = new Mock<IDistributedCache>();
            _servicoCache = new ServicoCache(_cacheMock.Object);
        }

        [Fact]
        public async Task ObterAsync_QuandoValorExiste_DeveRetornarValorDeserializado()
        {
            // Arrange
            var chave = "teste_chave";
            var objetoEsperado = new { Id = 1, Nome = "Teste" };
            var jsonString = JsonSerializer.Serialize(objetoEsperado);
            
            // Em vez de usar GetStringAsync, use GetAsync e retorne os bytes
            var bytes = System.Text.Encoding.UTF8.GetBytes(jsonString);
            _cacheMock.Setup(c => c.GetAsync(chave, It.IsAny<CancellationToken>()))
                .ReturnsAsync(bytes);

            // Act
            var resultado = await _servicoCache.ObterAsync<object>(chave);

            // Assert
            Assert.NotNull(resultado);
            var resultadoConvertido = JsonSerializer.Deserialize<dynamic>(
                JsonSerializer.Serialize(resultado));
            Assert.Equal(1, (int)resultadoConvertido.GetProperty("Id").GetInt32());
            Assert.Equal("Teste", resultadoConvertido.GetProperty("Nome").GetString());
            
            _cacheMock.Verify(c => c.GetAsync(chave, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task ObterAsync_QuandoValorNaoExiste_DeveRetornarDefault()
        {
            // Arrange
            var chave = "chave_inexistente";
            
            // Em vez de usar GetStringAsync, use GetAsync e retorne null
            _cacheMock.Setup(c => c.GetAsync(chave, It.IsAny<CancellationToken>()))
                .ReturnsAsync((byte[])null);

            // Act
            var resultado = await _servicoCache.ObterAsync<object>(chave);

            // Assert
            Assert.Null(resultado);
            _cacheMock.Verify(c => c.GetAsync(chave, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DefinirAsync_ComTempoExpiracaoDefinido_DeveChamarSetStringAsync()
        {
            // Arrange
            var chave = "chave_teste";
            var valor = new { Id = 1, Nome = "Teste" };
            var tempoExpiracao = TimeSpan.FromMinutes(5);
            
            // Em vez de usar SetStringAsync, use SetAsync
            _cacheMock.Setup(c => c.SetAsync(
                It.IsAny<string>(), 
                It.IsAny<byte[]>(), 
                It.IsAny<DistributedCacheEntryOptions>(), 
                It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            await _servicoCache.DefinirAsync(chave, valor, tempoExpiracao);

            // Assert
            _cacheMock.Verify(c => c.SetAsync(
                It.Is<string>(s => s == chave),
                It.IsAny<byte[]>(),
                It.Is<DistributedCacheEntryOptions>(opt => 
                    opt.AbsoluteExpirationRelativeToNow == tempoExpiracao),
                It.IsAny<CancellationToken>()), 
                Times.Once);
        }

        [Fact]
        public async Task DefinirAsync_SemTempoExpiracao_DeveUsarTempoDefault()
        {
            // Arrange
            var chave = "chave_teste";
            var valor = new { Id = 1, Nome = "Teste" };
            
            // Em vez de usar SetStringAsync, use SetAsync
            _cacheMock.Setup(c => c.SetAsync(
                It.IsAny<string>(), 
                It.IsAny<byte[]>(), 
                It.IsAny<DistributedCacheEntryOptions>(), 
                It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            await _servicoCache.DefinirAsync(chave, valor);

            // Assert
            _cacheMock.Verify(c => c.SetAsync(
                It.Is<string>(s => s == chave),
                It.IsAny<byte[]>(),
                It.Is<DistributedCacheEntryOptions>(opt => 
                    opt.AbsoluteExpirationRelativeToNow.HasValue && 
                    Math.Abs((opt.AbsoluteExpirationRelativeToNow.Value - TimeSpan.FromMinutes(10)).TotalSeconds) < 1),
                It.IsAny<CancellationToken>()), 
                Times.Once);
        }

        [Fact]
        public async Task RemoverAsync_DeveChamarRemoveAsync()
        {
            // Arrange
            var chave = "chave_para_remover";
            
            _cacheMock.Setup(c => c.RemoveAsync(
                It.IsAny<string>(), 
                It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            await _servicoCache.RemoverAsync(chave);

            // Assert
            _cacheMock.Verify(c => c.RemoveAsync(
                It.Is<string>(s => s == chave), 
                It.IsAny<CancellationToken>()), 
                Times.Once);
        }
        
        [Fact]
        public async Task DefinirAsync_ComValorNulo_DeveLancarArgumentNullException()
        {
            // Arrange
            var chave = "chave_teste";
            object valor = null;

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => 
                _servicoCache.DefinirAsync(chave, valor));
                
            _cacheMock.Verify(c => c.SetAsync(
                It.IsAny<string>(),
                It.IsAny<byte[]>(),
                It.IsAny<DistributedCacheEntryOptions>(),
                It.IsAny<CancellationToken>()), 
                Times.Never);
        }
        
        [Fact]
        public async Task DefinirAsync_ComChaveVazia_DeveLancarArgumentException()
        {
            // Arrange
            var chave = "";
            var valor = new { Id = 1, Nome = "Teste" };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => 
                _servicoCache.DefinirAsync(chave, valor));
                
            _cacheMock.Verify(c => c.SetAsync(
                It.IsAny<string>(),
                It.IsAny<byte[]>(),
                It.IsAny<DistributedCacheEntryOptions>(),
                It.IsAny<CancellationToken>()), 
                Times.Never);
        }
        
        [Fact]
        public async Task RemoverAsync_ComChaveVazia_DeveLancarArgumentException()
        {
            // Arrange
            var chave = "";
            
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => 
                _servicoCache.RemoverAsync(chave));
                
            _cacheMock.Verify(c => c.RemoveAsync(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()), 
                Times.Never);
        }
        
        [Fact]
        public async Task ObterAsync_ComChaveVazia_DeveLancarArgumentException()
        {
            // Arrange
            var chave = "";
            
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => 
                _servicoCache.ObterAsync<object>(chave));
                
            _cacheMock.Verify(c => c.GetAsync(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()), 
                Times.Never);
        }
        
        [Fact]
        public async Task ObterAsync_ComSerializacaoInvalida_DeveRetornarDefault()
        {
            // Arrange
            var chave = "chave_com_json_invalido";
            var jsonInvalido = "{invalidJson:}";
            
            // Em vez de usar GetStringAsync, use GetAsync e retorne os bytes
            var bytes = System.Text.Encoding.UTF8.GetBytes(jsonInvalido);
            _cacheMock.Setup(c => c.GetAsync(chave, It.IsAny<CancellationToken>()))
                .ReturnsAsync(bytes);

            // Act
            var resultado = await _servicoCache.ObterAsync<TestModel>(chave);

            // Assert
            Assert.Null(resultado);
            _cacheMock.Verify(c => c.GetAsync(chave, It.IsAny<CancellationToken>()), Times.Once);
        }
        
        private class TestModel
        {
            public int Id { get; set; }
            public string? Nome { get; set; }
        }
    }
}