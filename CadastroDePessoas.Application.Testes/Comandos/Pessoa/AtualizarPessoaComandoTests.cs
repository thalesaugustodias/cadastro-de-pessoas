using CadastroDePessoas.Application.CQRS.Comandos.Pessoa.AtualizarPessoa;
using CadastroDePessoas.Application.DTOs.Pessoa;
using CadastroDePessoas.Application.Interfaces;
using CadastroDePessoas.Domain.Enums;
using CadastroDePessoas.Domain.Interfaces;
using Moq;

namespace CadastroDePessoas.Application.Testes.Comandos.Pessoa
{
    public class AtualizarPessoaComandoTests
    {
        private readonly Mock<IRepositorioPessoa> _repositorioPessoaMock;
        private readonly Mock<IServiceCache> _servicoCacheMock;
        private readonly AtualizarPessoaComandoHandler _manipulador;
        private readonly Guid _pessoaId = Guid.NewGuid();

        public AtualizarPessoaComandoTests()
        {
            _repositorioPessoaMock = new Mock<IRepositorioPessoa>();
            _servicoCacheMock = new Mock<IServiceCache>();
            _manipulador = new AtualizarPessoaComandoHandler(_repositorioPessoaMock.Object, _servicoCacheMock.Object);
        }

        [Fact]
        public async Task Handle_ComDadosValidos_DeveAtualizarPessoa()
        {
            // Arrange
            var pessoaExistente = new Domain.Entidades.Pessoa(
                "João Silva",
                Sexo.Masculino,
                "joao@exemplo.com",
                new DateTime(1990, 1, 1),
                "São Paulo",
                "Brasileira",
                "52998224725"
            );

            var comando = new AtualizarPessoaComando
            {
                Id = _pessoaId,
                Nome = "João Silva Atualizado",
                Sexo = Sexo.Masculino,
                Email = "joao.atualizado@exemplo.com",
                DataNascimento = new DateTime(1990, 1, 1),
                Naturalidade = "Rio de Janeiro",
                Nacionalidade = "Brasileira",
                Endereco = "Rua Exemplo, 123"
            };

            _repositorioPessoaMock
                .Setup(r => r.ObterPorIdAsync(_pessoaId))
                .ReturnsAsync(pessoaExistente);

            _repositorioPessoaMock
                .Setup(r => r.AtualizarAsync(It.IsAny<Domain.Entidades.Pessoa>()))
                .Returns(Task.CompletedTask);

            // Act
            var resultado = await _manipulador.Handle(comando, CancellationToken.None);

            // Assert
            Assert.NotNull(resultado);
            Assert.IsType<PessoaDTO>(resultado);
            Assert.Equal(comando.Nome, resultado.Nome);
            Assert.Equal(comando.Email, resultado.Email);
            Assert.Equal(comando.Naturalidade, resultado.Naturalidade);
            Assert.Equal(comando.Endereco, resultado.Endereco);

            _repositorioPessoaMock.Verify(r => r.ObterPorIdAsync(_pessoaId), Times.Once);
            _repositorioPessoaMock.Verify(r => r.AtualizarAsync(It.IsAny<Domain.Entidades.Pessoa>()), Times.Once);
            _servicoCacheMock.Verify(c => c.RemoverAsync("pessoas_lista"), Times.Once);
            _servicoCacheMock.Verify(c => c.RemoverAsync($"pessoa_{_pessoaId}"), Times.Once);
        }

        [Fact]
        public async Task Handle_ComPessoaInexistente_DeveLancarExcecao()
        {
            // Arrange
            var comando = new AtualizarPessoaComando
            {
                Id = _pessoaId,
                Nome = "João Silva Atualizado",
                Sexo = Sexo.Masculino,
                Email = "joao.atualizado@exemplo.com",
                DataNascimento = new DateTime(1990, 1, 1),
                Naturalidade = "Rio de Janeiro",
                Nacionalidade = "Brasileira"
            };

            _repositorioPessoaMock
                .Setup(r => r.ObterPorIdAsync(_pessoaId))
                .ReturnsAsync((Domain.Entidades.Pessoa)null);

            // Act & Assert
            var excecao = await Assert.ThrowsAsync<Exception>(() => _manipulador.Handle(comando, CancellationToken.None));
            Assert.Equal("Pessoa não encontrada", excecao.Message);

            _repositorioPessoaMock.Verify(r => r.ObterPorIdAsync(_pessoaId), Times.Once);
            _repositorioPessoaMock.Verify(r => r.AtualizarAsync(It.IsAny<Domain.Entidades.Pessoa>()), Times.Never);
            _servicoCacheMock.Verify(c => c.RemoverAsync(It.IsAny<string>()), Times.Never);
        }
    }
}
