using CadastroDePessoas.Application.CQRS.Comandos.Pessoa.RemoverPessoa;
using CadastroDePessoas.Application.Interfaces;
using CadastroDePessoas.Domain.Enums;
using CadastroDePessoas.Domain.Interfaces;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CadastroDePessoas.Application.Testes.Comandos.Pessoa
{
    public class RemoverPessoaComandoTests
    {
        private readonly Mock<IRepositorioPessoa> _repositorioPessoaMock;
        private readonly Mock<IServiceCache> _servicoCacheMock;
        private readonly RemoverPessoaComandoHandler _manipulador;
        private readonly Guid _pessoaId = Guid.NewGuid();

        public RemoverPessoaComandoTests()
        {
            _repositorioPessoaMock = new Mock<IRepositorioPessoa>();
            _servicoCacheMock = new Mock<IServiceCache>();
            _manipulador = new RemoverPessoaComandoHandler(_repositorioPessoaMock.Object, _servicoCacheMock.Object);
        }

        [Fact]
        public async Task Handle_ComPessoaExistente_DeveRemoverPessoa()
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

            var comando = new RemoverPessoaComando(_pessoaId);

            _repositorioPessoaMock
                .Setup(r => r.ObterPorIdAsync(_pessoaId))
                .ReturnsAsync(pessoaExistente);

            _repositorioPessoaMock
                .Setup(r => r.RemoverAsync(It.IsAny<Domain.Entidades.Pessoa>()))
                .Returns(Task.CompletedTask);

            // Act
            var resultado = await _manipulador.Handle(comando, CancellationToken.None);

            // Assert
            Assert.True(resultado);

            _repositorioPessoaMock.Verify(r => r.ObterPorIdAsync(_pessoaId), Times.Once);
            _repositorioPessoaMock.Verify(r => r.RemoverAsync(It.IsAny<Domain.Entidades.Pessoa>()), Times.Once);
            _servicoCacheMock.Verify(c => c.RemoverAsync("pessoas_lista"), Times.Once);
            _servicoCacheMock.Verify(c => c.RemoverAsync($"pessoa_{_pessoaId}"), Times.Once);
        }

        [Fact]
        public async Task Handle_ComPessoaInexistente_DeveLancarExcecao()
        {
            // Arrange
            var comando = new RemoverPessoaComando(_pessoaId);

            _repositorioPessoaMock
                .Setup(r => r.ObterPorIdAsync(_pessoaId))
                .ReturnsAsync((Domain.Entidades.Pessoa)null);

            // Act & Assert
            var excecao = await Assert.ThrowsAsync<Exception>(() => _manipulador.Handle(comando, CancellationToken.None));
            Assert.Equal("Pessoa não encontrada", excecao.Message);

            _repositorioPessoaMock.Verify(r => r.ObterPorIdAsync(_pessoaId), Times.Once);
            _repositorioPessoaMock.Verify(r => r.RemoverAsync(It.IsAny<Domain.Entidades.Pessoa>()), Times.Never);
            _servicoCacheMock.Verify(c => c.RemoverAsync(It.IsAny<string>()), Times.Never);
        }
    }
}
