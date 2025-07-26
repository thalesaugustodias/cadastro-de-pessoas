using CadastroDePessoas.Application.CQRS.Comandos.Pessoa.CriarPessoa;
using CadastroDePessoas.Application.DTOs.Pessoa;
using CadastroDePessoas.Application.Interfaces;
using CadastroDePessoas.Domain.Enums;
using CadastroDePessoas.Domain.Interfaces;
using Moq;

namespace CadastroDePessoas.Application.Testes.Comandos.Pessoa
{
    public class CriarPessoaComandoTests
    {
        private readonly Mock<IRepositorioPessoa> _repositorioPessoaMock;
        private readonly Mock<IServiceCache> _servicoCacheMock;
        private readonly CriarPessoaComandoHandler _manipulador;

        public CriarPessoaComandoTests()
        {
            _repositorioPessoaMock = new Mock<IRepositorioPessoa>();
            _servicoCacheMock = new Mock<IServiceCache>();
            _manipulador = new CriarPessoaComandoHandler(_repositorioPessoaMock.Object, _servicoCacheMock.Object);
        }

        [Fact]
        public async Task Handle_ComDadosValidos_DeveCriarPessoa()
        {
            // Arrange
            var comando = new CriarPessoaComando
            {
                Nome = "João Silva",
                Sexo = Sexo.Masculino,
                Email = "joao@exemplo.com",
                DataNascimento = new DateTime(1990, 1, 1),
                Naturalidade = "São Paulo",
                Nacionalidade = "Brasileira",
                CPF = "52998224725"
            };

            _repositorioPessoaMock
                .Setup(r => r.CpfExisteAsync(It.IsAny<string>(), It.IsAny<Guid?>()))
                .ReturnsAsync(false);

            _repositorioPessoaMock
                .Setup(r => r.AdicionarAsync(It.IsAny<Domain.Entidades.Pessoa>()))
                .Returns(Task.CompletedTask);

            // Act
            var resultado = await _manipulador.Handle(comando, CancellationToken.None);

            // Assert
            Assert.NotNull(resultado);
            Assert.IsType<PessoaDTO>(resultado);
            Assert.Equal(comando.Nome, resultado.Nome);
            Assert.Equal(comando.Sexo, resultado.Sexo);
            Assert.Equal(comando.Email, resultado.Email);
            Assert.Equal(comando.DataNascimento, resultado.DataNascimento);
            Assert.Equal(comando.Naturalidade, resultado.Naturalidade);
            Assert.Equal(comando.Nacionalidade, resultado.Nacionalidade);
            Assert.Equal(comando.CPF, resultado.CPF);

            _repositorioPessoaMock.Verify(r => r.CpfExisteAsync(It.IsAny<string>(), It.IsAny<Guid?>()), Times.Once);
            _repositorioPessoaMock.Verify(r => r.AdicionarAsync(It.IsAny<Domain.Entidades.Pessoa>()), Times.Once);
            _servicoCacheMock.Verify(c => c.RemoverAsync("pessoas_lista"), Times.Once);
        }

        [Fact]
        public async Task Handle_ComCPFExistente_DeveLancarExcecao()
        {
            // Arrange
            var comando = new CriarPessoaComando
            {
                Nome = "João Silva",
                Sexo = Sexo.Masculino,
                Email = "joao@exemplo.com",
                DataNascimento = new DateTime(1990, 1, 1),
                Naturalidade = "São Paulo",
                Nacionalidade = "Brasileira",
                CPF = "52998224725"
            };

            _repositorioPessoaMock
                .Setup(r => r.CpfExisteAsync(It.IsAny<string>(), It.IsAny<Guid?>()))
                .ReturnsAsync(true);

            // Act & Assert
            var excecao = await Assert.ThrowsAsync<Exception>(() => _manipulador.Handle(comando, CancellationToken.None));
            Assert.Equal("CPF já está em uso", excecao.Message);

            _repositorioPessoaMock.Verify(r => r.CpfExisteAsync(It.IsAny<string>(), It.IsAny<Guid?>()), Times.Once);
            _repositorioPessoaMock.Verify(r => r.AdicionarAsync(It.IsAny<Domain.Entidades.Pessoa>()), Times.Never);
            _servicoCacheMock.Verify(c => c.RemoverAsync("pessoas_lista"), Times.Never);
        }
    }
}
