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
            var endereco = new EnderecoComando
            {
                CEP = "01310-100",
                Logradouro = "Av. Paulista",
                Numero = "1000",
                Complemento = "Apto 101",
                Bairro = "Bela Vista",
                Cidade = "São Paulo",
                Estado = "SP"
            };

            var comando = new CriarPessoaComando
            {
                Nome = "João Silva",
                Sexo = Sexo.Masculino,
                Email = "joao@exemplo.com",
                DataNascimento = new DateTime(1990, 1, 1),
                Naturalidade = "São Paulo",
                Nacionalidade = "Brasileira",
                CPF = "52998224725",
                Telefone = "(11) 99999-9999",
                Endereco = endereco
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
            Assert.Equal(comando.Telefone, resultado.Telefone);
            Assert.NotNull(resultado.Endereco);
            Assert.Equal(endereco.CEP, resultado.Endereco.CEP);
            Assert.Equal(endereco.Logradouro, resultado.Endereco.Logradouro);
            Assert.Equal(endereco.Numero, resultado.Endereco.Numero);
            Assert.Equal(endereco.Cidade, resultado.Endereco.Cidade);

            _repositorioPessoaMock.Verify(r => r.CpfExisteAsync(It.IsAny<string>(), It.IsAny<Guid?>()), Times.Once);
            _repositorioPessoaMock.Verify(r => r.AdicionarAsync(It.IsAny<Domain.Entidades.Pessoa>()), Times.Once);
            _servicoCacheMock.Verify(c => c.RemoverAsync("pessoas_lista"), Times.Once);
        }

        [Fact]
        public async Task Handle_SemEndereco_DeveCriarPessoaSemEndereco()
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
                CPF = "52998224725",
                Telefone = "(11) 99999-9999",
                Endereco = null
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
            Assert.Equal(comando.Nome, resultado.Nome);
            Assert.Equal(comando.CPF, resultado.CPF);
            Assert.Equal(comando.Telefone, resultado.Telefone);
            Assert.Null(resultado.Endereco);

            _repositorioPessoaMock.Verify(r => r.AdicionarAsync(It.IsAny<Domain.Entidades.Pessoa>()), Times.Once);
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
                CPF = "52998224725",
                Telefone = "(11) 99999-9999"
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
