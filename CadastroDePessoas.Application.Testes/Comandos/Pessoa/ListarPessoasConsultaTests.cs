using CadastroDePessoas.Application.CQRS.Queries.Pessoa.ListarPessoa;
using CadastroDePessoas.Application.DTOs.Endereco;
using CadastroDePessoas.Application.DTOs.Pessoa;
using CadastroDePessoas.Application.Interfaces;
using CadastroDePessoas.Domain.Entidades;
using CadastroDePessoas.Domain.Enums;
using CadastroDePessoas.Domain.Interfaces;
using Moq;

namespace CadastroDePessoas.Application.Testes.Comandos.Pessoa
{
    public class ListarPessoasConsultaTests
    {
        private readonly Mock<IRepositorioPessoa> _repositorioPessoaMock;
        private readonly Mock<IServiceCache> _servicoCacheMock;
        private readonly ListarPessoasQueryHandler _manipulador;

        public ListarPessoasConsultaTests()
        {
            _repositorioPessoaMock = new Mock<IRepositorioPessoa>();
            _servicoCacheMock = new Mock<IServiceCache>();
            _manipulador = new ListarPessoasQueryHandler(_repositorioPessoaMock.Object, _servicoCacheMock.Object);
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

            var pessoasCache = new List<PessoaDTO>
            {
                new() {
                    Id = Guid.NewGuid(),
                    Nome = "João Silva",
                    Sexo = Sexo.Masculino,
                    Email = "joao@exemplo.com",
                    DataNascimento = new DateTime(1990, 1, 1),
                    Naturalidade = "São Paulo",
                    Nacionalidade = "Brasileira",
                    CPF = "52998224725",
                    Telefone = "(11) 99999-9999",
                    Endereco = endereco
                }
            };

            _servicoCacheMock
                .Setup(c => c.ObterAsync<IEnumerable<PessoaDTO>>("pessoas_lista"))
                .ReturnsAsync(pessoasCache);

            // Act
            var resultado = await _manipulador.Handle(new ListarPessoasQuery(), CancellationToken.None);

            // Assert
            Assert.NotNull(resultado);
            Assert.Single(resultado);
            var pessoa = resultado.First();
            Assert.NotNull(pessoa.Endereco);
            Assert.Equal(endereco.CEP, pessoa.Endereco.CEP);
            Assert.Equal(endereco.Logradouro, pessoa.Endereco.Logradouro);
            Assert.Equal("(11) 99999-9999", pessoa.Telefone);

            _servicoCacheMock.Verify(c => c.ObterAsync<IEnumerable<PessoaDTO>>("pessoas_lista"), Times.Once);
            _repositorioPessoaMock.Verify(r => r.ObterTodosAsync(), Times.Never);
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

            var pessoas = new List<Domain.Entidades.Pessoa>
            {
                new Domain.Entidades.Pessoa(
                    "João Silva",
                    Sexo.Masculino,
                    "joao@exemplo.com",
                    new DateTime(1990, 1, 1),
                    "São Paulo",
                    "Brasileira",
                    "52998224725",
                    "(11) 99999-9999",
                    endereco
                )
            };

            _servicoCacheMock
                .Setup(c => c.ObterAsync<IEnumerable<PessoaDTO>>("pessoas_lista"))
                .ReturnsAsync((IEnumerable<PessoaDTO>)null);

            _repositorioPessoaMock
                .Setup(r => r.ObterTodosAsync())
                .ReturnsAsync(pessoas);

            // Act
            var resultado = await _manipulador.Handle(new ListarPessoasQuery(), CancellationToken.None);

            // Assert
            Assert.NotNull(resultado);
            Assert.Single(resultado);
            var pessoa = resultado.First();
            Assert.NotNull(pessoa.Endereco);
            Assert.Equal(endereco.CEP, pessoa.Endereco.CEP);
            Assert.Equal(endereco.Logradouro, pessoa.Endereco.Logradouro);
            Assert.Equal("(11) 99999-9999", pessoa.Telefone);

            _servicoCacheMock.Verify(c => c.ObterAsync<IEnumerable<PessoaDTO>>("pessoas_lista"), Times.Once);
            _repositorioPessoaMock.Verify(r => r.ObterTodosAsync(), Times.Once);
            _servicoCacheMock.Verify(c => c.DefinirAsync(
                "pessoas_lista",
                It.IsAny<IEnumerable<PessoaDTO>>(),
                It.IsAny<TimeSpan?>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_SemPessoas_DeveRetornarListaVazia()
        {
            // Arrange
            _servicoCacheMock
                .Setup(c => c.ObterAsync<IEnumerable<PessoaDTO>>("pessoas_lista"))
                .ReturnsAsync((IEnumerable<PessoaDTO>)null);

            _repositorioPessoaMock
                .Setup(r => r.ObterTodosAsync())
                .ReturnsAsync(new List<Domain.Entidades.Pessoa>());

            // Act
            var resultado = await _manipulador.Handle(new ListarPessoasQuery(), CancellationToken.None);

            // Assert
            Assert.NotNull(resultado);
            Assert.Empty(resultado);

            _servicoCacheMock.Verify(c => c.ObterAsync<IEnumerable<PessoaDTO>>("pessoas_lista"), Times.Once);
            _repositorioPessoaMock.Verify(r => r.ObterTodosAsync(), Times.Once);
        }
    }
}
