using CadastroDePessoas.Application.DTOs.Endereco;
using CadastroDePessoas.Application.DTOs.Pessoa;
using CadastroDePessoas.Application.Services;
using CadastroDePessoas.Domain.Entidades;
using CadastroDePessoas.Domain.Enums;
using CadastroDePessoas.Domain.Interfaces;
using Moq;
using System.Text;

namespace CadastroDePessoas.Application.Testes.Servicos
{
    public class ExportacaoServiceTests
    {
        private readonly Mock<IRepositorioPessoa> _repositorioPessoaMock;
        private readonly ExportacaoService _service;

        public ExportacaoServiceTests()
        {
            _repositorioPessoaMock = new Mock<IRepositorioPessoa>();
            _service = new ExportacaoService(_repositorioPessoaMock.Object);
        }

        [Fact]
        public async Task ExportarParaExcel_DeveRetornarBytesDoArquivo()
        {
            // Arrange
            var pessoas = CriarListaDePessoasTeste();
            _repositorioPessoaMock.Setup(r => r.ObterTodosAsync()).ReturnsAsync(pessoas);

            // Act
            var resultado = await _service.ExportarParaExcel(new[] { "nome", "email", "cpf" });

            // Assert
            Assert.NotNull(resultado);
            Assert.True(resultado.Length > 0);
            _repositorioPessoaMock.Verify(r => r.ObterTodosAsync(), Times.Once);
        }

        [Fact]
        public async Task ExportarParaPdf_DeveRetornarBytesDoArquivo()
        {
            // Arrange
            var pessoas = CriarListaDePessoasTeste();
            _repositorioPessoaMock.Setup(r => r.ObterTodosAsync()).ReturnsAsync(pessoas);

            // Act
            var resultado = await _service.ExportarParaPdf(new[] { "nome", "email", "cpf" });

            // Assert
            Assert.NotNull(resultado);
            Assert.True(resultado.Length > 0);
            _repositorioPessoaMock.Verify(r => r.ObterTodosAsync(), Times.Once);
        }

        [Fact]
        public async Task GerarTemplateCsv_DeveRetornarBytesDoArquivo()
        {
            // Act
            var resultado = await _service.GerarTemplateCsv();

            // Assert
            Assert.NotNull(resultado);
            Assert.True(resultado.Length > 0);
            
            var conteudo = Encoding.UTF8.GetString(resultado);
            Assert.Contains("Nome,Email,CPF,DataNascimento", conteudo);
            Assert.Contains("CEP,Logradouro,Numero", conteudo);
        }

        private List<Pessoa> CriarListaDePessoasTeste()
        {
            var endereco = new Endereco(
                "01310-100",
                "Av. Paulista",
                "1000",
                "Apto 101",
                "Bela Vista",
                "São Paulo",
                "SP"
            );

            var pessoa1 = new Pessoa(
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

            var pessoa2 = new Pessoa(
                "Maria Souza",
                Sexo.Feminino,
                "maria@exemplo.com",
                new DateTime(1985, 5, 15),
                "Rio de Janeiro",
                "Brasileira",
                "11144477735",
                "(21) 98888-8888",
                null
            );

            return new List<Pessoa> { pessoa1, pessoa2 };
        }
    }
}