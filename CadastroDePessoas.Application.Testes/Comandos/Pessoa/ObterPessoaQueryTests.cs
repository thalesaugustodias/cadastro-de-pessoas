using CadastroDePessoas.Application.CQRS.Queries.Pessoa.ObterPessoa;
using CadastroDePessoas.Application.DTOs.Endereco;
using CadastroDePessoas.Application.DTOs.Pessoa;
using CadastroDePessoas.Application.Interfaces;
using CadastroDePessoas.Domain.Entidades;
using CadastroDePessoas.Domain.Enums;
using CadastroDePessoas.Domain.Interfaces;
using Moq;

namespace CadastroDePessoas.Application.Testes.Comandos.Pessoa
{
    public class ObterPessoaQueryTests
    {
        private readonly Mock<IRepositorioPessoa> _repositorioPessoaMock;
        private readonly Mock<IServiceCache> _servicoCacheMock;
        private readonly ObterPessoaQueryHandler _manipulador;
        private readonly Guid _pessoaId = Guid.NewGuid();

        public ObterPessoaQueryTests()
        {
            _repositorioPessoaMock = new Mock<IRepositorioPessoa>();
            _servicoCacheMock = new Mock<IServiceCache>();
            _manipulador = new ObterPessoaQueryHandler(_repositorioPessoaMock.Object, _servicoCacheMock.Object);
        }

        [Fact]
        public async Task Handle_ComDadosNoCache_DeveRetornarDadosDoCache()
        {
            // Arrange
            var endereco = new EnderecoDTO
            {
                Id = Guid.NewGuid(),
                CEP = "01310-100",
                Logradouro = "Av. Paulista",
                Numero = "1000",
                Complemento = "Apto 101",
                Bairro = "Bela Vista",
                Cidade = "São Paulo",
                Estado = "SP"
            };

            var pessoaCache = new PessoaDTO
            {
                Id = _pessoaId,
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

            _servicoCacheMock
                .Setup(c => c.ObterAsync<PessoaDTO>($"pessoa_{_pessoaId}"))
                .ReturnsAsync(pessoaCache);

            // Act
            var resultado = await _manipulador.Handle(new ObterPessoaQuery(_pessoaId), CancellationToken.None);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(_pessoaId, resultado.Id);
            Assert.NotNull(resultado.Endereco);
            Assert.Equal(endereco.CEP, resultado.Endereco.CEP);
            Assert.Equal(endereco.Logradouro, resultado.Endereco.Logradouro);
            Assert.Equal("(11) 99999-9999", resultado.Telefone);

            _servicoCacheMock.Verify(c => c.ObterAsync<PessoaDTO>($"pessoa_{_pessoaId}"), Times.Once);
            _repositorioPessoaMock.Verify(r => r.ObterPorIdAsync(_pessoaId), Times.Never);
        }

        [Fact]
        public async Task Handle_SemDadosNoCache_DeveBuscarDoBanco()
        {
            // Arrange
            var endereco = new Endereco(
                "01310-100",
                "Av. Paulista",
                "1000",
                "Apto 101",
                "Bela Vista",
                "São Paulo",
                "SP"
            );

            var pessoa = new Domain.Entidades.Pessoa(
                "João Silva",
                Sexo.Masculino,
                "joao@exemplo.com",
                new DateTime(1990, 1, 1),
                "São Paulo",
                "Brasileira",
                "52998224725",
                "(11) 99999-9999",
                endereco
            );

            _servicoCacheMock
                .Setup(c => c.ObterAsync<PessoaDTO>($"pessoa_{_pessoaId}"))
                .ReturnsAsync((PessoaDTO)null);

            _repositorioPessoaMock
                .Setup(r => r.ObterPorIdAsync(_pessoaId))
                .ReturnsAsync(pessoa);

            // Act
            var resultado = await _manipulador.Handle(new ObterPessoaQuery(_pessoaId), CancellationToken.None);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal("João Silva", resultado.Nome);
            Assert.NotNull(resultado.Endereco);
            Assert.Equal(endereco.CEP, resultado.Endereco.CEP);
            Assert.Equal(endereco.Logradouro, resultado.Endereco.Logradouro);
            Assert.Equal("(11) 99999-9999", resultado.Telefone);

            _servicoCacheMock.Verify(c => c.ObterAsync<PessoaDTO>($"pessoa_{_pessoaId}"), Times.Once);
            _repositorioPessoaMock.Verify(r => r.ObterPorIdAsync(_pessoaId), Times.Once);
            _servicoCacheMock.Verify(c => c.DefinirAsync(
                $"pessoa_{_pessoaId}",
                It.IsAny<PessoaDTO>(),
                It.IsAny<TimeSpan?>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ComPessoaInexistente_DeveLancarExcecao()
        {
            // Arrange
            _servicoCacheMock
                .Setup(c => c.ObterAsync<PessoaDTO>($"pessoa_{_pessoaId}"))
                .ReturnsAsync((PessoaDTO)null);

            _repositorioPessoaMock
                .Setup(r => r.ObterPorIdAsync(_pessoaId))
                .ReturnsAsync((Domain.Entidades.Pessoa)null);

            // Act & Assert
            var excecao = await Assert.ThrowsAsync<Exception>(() => _manipulador.Handle(new ObterPessoaQuery(_pessoaId), CancellationToken.None));
            Assert.Equal("Pessoa não encontrada", excecao.Message);

            _servicoCacheMock.Verify(c => c.ObterAsync<PessoaDTO>($"pessoa_{_pessoaId}"), Times.Once);
            _repositorioPessoaMock.Verify(r => r.ObterPorIdAsync(_pessoaId), Times.Once);
        }

        [Fact]
        public async Task Handle_ComPessoaSemEndereco_DeveRetornarPessoaSemEndereco()
        {
            // Arrange
            var pessoa = new Domain.Entidades.Pessoa(
                "João Silva",
                Sexo.Masculino,
                "joao@exemplo.com",
                new DateTime(1990, 1, 1),
                "São Paulo",
                "Brasileira",
                "52998224725",
                "(11) 99999-9999",
                null
            );

            _servicoCacheMock
                .Setup(c => c.ObterAsync<PessoaDTO>($"pessoa_{_pessoaId}"))
                .ReturnsAsync((PessoaDTO)null);

            _repositorioPessoaMock
                .Setup(r => r.ObterPorIdAsync(_pessoaId))
                .ReturnsAsync(pessoa);

            // Act
            var resultado = await _manipulador.Handle(new ObterPessoaQuery(_pessoaId), CancellationToken.None);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal("João Silva", resultado.Nome);
            Assert.Null(resultado.Endereco);
            Assert.Equal("(11) 99999-9999", resultado.Telefone);

            _servicoCacheMock.Verify(c => c.ObterAsync<PessoaDTO>($"pessoa_{_pessoaId}"), Times.Once);
            _repositorioPessoaMock.Verify(r => r.ObterPorIdAsync(_pessoaId), Times.Once);
        }
    }
}
