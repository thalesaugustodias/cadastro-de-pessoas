using CadastroDePessoas.Application.DTOs.Endereco;
using CadastroDePessoas.Application.DTOs.Pessoa;
using CadastroDePessoas.Application.Services;
using CadastroDePessoas.Domain.Entidades;
using CadastroDePessoas.Domain.Enums;
using CadastroDePessoas.Domain.Interfaces;
using Moq;
using System.Reflection;
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
        public async Task ExportarParaExcel_ComCamposCompletos_DeveRetornarBytesDoArquivo()
        {
            // Arrange
            var pessoas = CriarListaDePessoasTeste();
            _repositorioPessoaMock.Setup(r => r.ObterTodosAsync()).ReturnsAsync(pessoas);

            var todosOsCampos = new[] { 
                "nome", "email", "cpf", "dataNascimento", "telefone", 
                "sexo", "naturalidade", "nacionalidade", 
                "endereco.cep", "endereco.logradouro", "endereco.numero", 
                "endereco.complemento", "endereco.bairro", "endereco.cidade", "endereco.estado" 
            };

            // Act
            var resultado = await _service.ExportarParaExcel(todosOsCampos);

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
        public async Task ExportarParaPdf_ComCamposCompletos_DeveRetornarBytesDoArquivo()
        {
            // Arrange
            var pessoas = CriarListaDePessoasTeste();
            _repositorioPessoaMock.Setup(r => r.ObterTodosAsync()).ReturnsAsync(pessoas);

            var todosOsCampos = new[] { 
                "nome", "email", "cpf", "dataNascimento", "telefone", 
                "sexo", "naturalidade", "nacionalidade", 
                "endereco.cep", "endereco.logradouro", "endereco.numero", 
                "endereco.complemento", "endereco.bairro", "endereco.cidade", "endereco.estado" 
            };

            // Act
            var resultado = await _service.ExportarParaPdf(todosOsCampos);

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

        [Theory]
        [InlineData("nome", "Nome")]
        [InlineData("email", "E-mail")]
        [InlineData("cpf", "CPF")]
        [InlineData("dataNascimento", "Data de Nascimento")]
        [InlineData("telefone", "Telefone")]
        [InlineData("sexo", "Sexo")]
        [InlineData("naturalidade", "Naturalidade")]
        [InlineData("nacionalidade", "Nacionalidade")]
        [InlineData("endereco.cep", "CEP")]
        [InlineData("endereco.logradouro", "Logradouro")]
        [InlineData("endereco.numero", "Número")]
        [InlineData("endereco.complemento", "Complemento")]
        [InlineData("endereco.bairro", "Bairro")]
        [InlineData("endereco.cidade", "Cidade")]
        [InlineData("endereco.estado", "Estado")]
        [InlineData("campo_desconhecido", "campo_desconhecido")]
        public void ObterNomeCampo_DeveRetornarNomeFormatado(string campo, string resultadoEsperado)
        {
            // Arrange & Act
            var method = typeof(ExportacaoService).GetMethod("ObterNomeCampo", BindingFlags.NonPublic | BindingFlags.Instance);
            var resultado = method.Invoke(_service, new object[] { campo }) as string;

            // Assert
            Assert.Equal(resultadoEsperado, resultado);
        }

        [Fact]
        public void ObterValorCampo_ComDadosCompletos_DeveRetornarValoresCorretos()
        {
            // Arrange
            var pessoa = CriarPessoaDTOTeste();
            var method = typeof(ExportacaoService).GetMethod("ObterValorCampo", BindingFlags.NonPublic | BindingFlags.Instance);

            // Act & Assert
            // Dados da pessoa
            Assert.Equal("João Silva", method.Invoke(_service, new object[] { pessoa, "nome" }));
            Assert.Equal("joao@exemplo.com", method.Invoke(_service, new object[] { pessoa, "email" }));
            Assert.Equal("52998224725", method.Invoke(_service, new object[] { pessoa, "cpf" }));
            Assert.Equal("01/01/1990", method.Invoke(_service, new object[] { pessoa, "dataNascimento" }));
            Assert.Equal("(11) 99999-9999", method.Invoke(_service, new object[] { pessoa, "telefone" }));
            Assert.Equal("Masculino", method.Invoke(_service, new object[] { pessoa, "sexo" }));
            Assert.Equal("São Paulo", method.Invoke(_service, new object[] { pessoa, "naturalidade" }));
            Assert.Equal("Brasileira", method.Invoke(_service, new object[] { pessoa, "nacionalidade" }));

            // Dados do endereço
            Assert.Equal("01310-100", method.Invoke(_service, new object[] { pessoa, "endereco.cep" }));
            Assert.Equal("Av. Paulista", method.Invoke(_service, new object[] { pessoa, "endereco.logradouro" }));
            Assert.Equal("1000", method.Invoke(_service, new object[] { pessoa, "endereco.numero" }));
            Assert.Equal("Apto 101", method.Invoke(_service, new object[] { pessoa, "endereco.complemento" }));
            Assert.Equal("Bela Vista", method.Invoke(_service, new object[] { pessoa, "endereco.bairro" }));
            Assert.Equal("São Paulo", method.Invoke(_service, new object[] { pessoa, "endereco.cidade" }));
            Assert.Equal("SP", method.Invoke(_service, new object[] { pessoa, "endereco.estado" }));

            // Campo desconhecido
            Assert.Equal("", method.Invoke(_service, new object[] { pessoa, "campo_desconhecido" }));
        }

        [Fact]
        public void ObterValorCampo_ComCamposNulos_DeveRetornarValoresVazios()
        {
            // Arrange
            var pessoa = new PessoaDTO
            {
                Id = Guid.NewGuid(),
                Nome = null,
                Email = null,
                CPF = null,
                DataNascimento = DateTime.Now,
                Telefone = null,
                Sexo = null,
                Naturalidade = null,
                Nacionalidade = null,
                Endereco = null
            };

            var method = typeof(ExportacaoService).GetMethod("ObterValorCampo", BindingFlags.NonPublic | BindingFlags.Instance);

            // Act & Assert
            Assert.Equal("", method.Invoke(_service, new object[] { pessoa, "nome" }));
            Assert.Equal("", method.Invoke(_service, new object[] { pessoa, "email" }));
            Assert.Equal("", method.Invoke(_service, new object[] { pessoa, "cpf" }));
            Assert.Equal("", method.Invoke(_service, new object[] { pessoa, "sexo" }));
            Assert.Equal("", method.Invoke(_service, new object[] { pessoa, "naturalidade" }));
            Assert.Equal("", method.Invoke(_service, new object[] { pessoa, "nacionalidade" }));

            // Valores de endereço quando o endereço é nulo
            Assert.Equal("", method.Invoke(_service, new object[] { pessoa, "endereco.cep" }));
            Assert.Equal("", method.Invoke(_service, new object[] { pessoa, "endereco.logradouro" }));
            Assert.Equal("", method.Invoke(_service, new object[] { pessoa, "endereco.numero" }));
            Assert.Equal("", method.Invoke(_service, new object[] { pessoa, "endereco.complemento" }));
            Assert.Equal("", method.Invoke(_service, new object[] { pessoa, "endereco.bairro" }));
            Assert.Equal("", method.Invoke(_service, new object[] { pessoa, "endereco.cidade" }));
            Assert.Equal("", method.Invoke(_service, new object[] { pessoa, "endereco.estado" }));
        }

        [Theory]
        [InlineData(Sexo.Masculino, "Masculino")]
        [InlineData(Sexo.Feminino, "Feminino")]
        [InlineData(Sexo.Outro, "Outro")]
        public void ObterValorCampo_ComDiferentesSexos_DeveRetornarDescricaoCorreta(Sexo sexo, string descricaoEsperada)
        {
            // Arrange
            var pessoa = new PessoaDTO
            {
                Id = Guid.NewGuid(),
                Nome = "Teste",
                Sexo = sexo
            };

            var method = typeof(ExportacaoService).GetMethod("ObterValorCampo", BindingFlags.NonPublic | BindingFlags.Instance);

            // Act
            var resultado = method.Invoke(_service, new object[] { pessoa, "sexo" }) as string;

            // Assert
            Assert.Equal(descricaoEsperada, resultado);
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

        private PessoaDTO CriarPessoaDTOTeste()
        {
            return new PessoaDTO
            {
                Id = Guid.NewGuid(),
                Nome = "João Silva",
                Email = "joao@exemplo.com",
                CPF = "52998224725",
                DataNascimento = new DateTime(1990, 1, 1),
                Telefone = "(11) 99999-9999",
                Sexo = Sexo.Masculino,
                Naturalidade = "São Paulo",
                Nacionalidade = "Brasileira",
                DataCadastro = DateTime.Now,
                Endereco = new EnderecoDTO
                {
                    Id = Guid.NewGuid(),
                    CEP = "01310-100",
                    Logradouro = "Av. Paulista",
                    Numero = "1000",
                    Complemento = "Apto 101",
                    Bairro = "Bela Vista",
                    Cidade = "São Paulo",
                    Estado = "SP"
                }
            };
        }
    }
}