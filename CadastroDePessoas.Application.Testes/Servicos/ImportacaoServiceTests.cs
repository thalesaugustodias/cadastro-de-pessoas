using CadastroDePessoas.Application.CQRS.Comandos.Pessoa.CriarPessoa;
using CadastroDePessoas.Application.Services;
using CadastroDePessoas.Domain.Enums;
using CadastroDePessoas.Domain.Interfaces;
using MediatR;
using Moq;
using System.Text;

namespace CadastroDePessoas.Application.Testes.Servicos
{
    public class ImportacaoServiceTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<IRepositorioPessoa> _repositorioPessoaMock;
        private readonly ImportacaoService _service;

        public ImportacaoServiceTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _repositorioPessoaMock = new Mock<IRepositorioPessoa>();
            _service = new ImportacaoService(_mediatorMock.Object, _repositorioPessoaMock.Object);
        }

        [Fact]
        public async Task ImportarCsv_ComDadosValidos_DeveRetornarResultadoSucesso()
        {
            // Arrange
            var csvContent = @"Nome,Email,CPF,DataNascimento,Telefone,Sexo,Naturalidade,Nacionalidade,CEP,Logradouro,Numero,Complemento,Bairro,Cidade,Estado
""João Silva"",""joao@email.com"",""52998224725"",""1990-01-01"",""(11) 99999-9999"",""0"",""São Paulo"",""Brasil"",""01310-100"",""Av. Paulista"",""1000"",""Apto 101"",""Bela Vista"",""São Paulo"",""SP""
""Maria Souza"",""maria@email.com"",""11144477735"",""1985-05-15"",""(21) 98888-8888"",""1"",""Rio de Janeiro"",""Brasil"",""22050-002"",""Av. Copacabana"",""500"","""",""Copacabana"",""Rio de Janeiro"",""RJ""";

            var csvStream = new MemoryStream(Encoding.UTF8.GetBytes(csvContent));

            _repositorioPessoaMock
                .Setup(r => r.CpfExisteAsync(It.IsAny<string>(), It.IsAny<Guid?>()))
                .ReturnsAsync(false);

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<CriarPessoaComando>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Application.DTOs.Pessoa.PessoaDTO());

            // Act
            var resultado = await _service.ImportarCsv(csvStream);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(2, resultado.Total);
            Assert.Equal(2, resultado.Sucesso);
            Assert.Equal(0, resultado.Erros);
            Assert.Empty(resultado.Detalhes);

            _mediatorMock.Verify(m => m.Send(It.IsAny<CriarPessoaComando>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
        }

        [Fact]
        public async Task ImportarCsv_ComCPFJaExistente_DeveRetornarErro()
        {
            // Arrange
            var csvContent = @"Nome,Email,CPF,DataNascimento,Telefone,Sexo,Naturalidade,Nacionalidade
""João Silva"",""joao@email.com"",""52998224725"",""1990-01-01"",""(11) 99999-9999"",""0"",""São Paulo"",""Brasil""";

            var csvStream = new MemoryStream(Encoding.UTF8.GetBytes(csvContent));

            _repositorioPessoaMock
                .Setup(r => r.CpfExisteAsync("52998224725", It.IsAny<Guid?>()))
                .ReturnsAsync(true);

            // Act
            var resultado = await _service.ImportarCsv(csvStream);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(1, resultado.Total);
            Assert.Equal(0, resultado.Sucesso);
            Assert.Equal(1, resultado.Erros);
            Assert.Single(resultado.Detalhes);
            Assert.Contains("CPF já cadastrado", resultado.Detalhes.First().Mensagem);

            _mediatorMock.Verify(m => m.Send(It.IsAny<CriarPessoaComando>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task ImportarCsv_ComDataInvalida_DeveRetornarErro()
        {
            // Arrange
            var csvContent = @"Nome,Email,CPF,DataNascimento,Telefone,Sexo,Naturalidade,Nacionalidade
""João Silva"",""joao@email.com"",""52998224725"",""data-invalida"",""(11) 99999-9999"",""0"",""São Paulo"",""Brasil""";

            var csvStream = new MemoryStream(Encoding.UTF8.GetBytes(csvContent));

            _repositorioPessoaMock
                .Setup(r => r.CpfExisteAsync(It.IsAny<string>(), It.IsAny<Guid?>()))
                .ReturnsAsync(false);

            // Act
            var resultado = await _service.ImportarCsv(csvStream);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(1, resultado.Total);
            Assert.Equal(0, resultado.Sucesso);
            Assert.Equal(1, resultado.Erros);
            Assert.Single(resultado.Detalhes);
            Assert.Contains("Data", resultado.Detalhes.First().Mensagem);

            _mediatorMock.Verify(m => m.Send(It.IsAny<CriarPessoaComando>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task ImportarCsv_ComCPFNulo_DeveRetornarErro()
        {
            // Arrange
            var csvContent = @"Nome,Email,CPF,DataNascimento,Telefone,Sexo,Naturalidade,Nacionalidade
""João Silva"",""joao@email.com"","""",""1990-01-01"",""(11) 99999-9999"",""0"",""São Paulo"",""Brasil""";

            var csvStream = new MemoryStream(Encoding.UTF8.GetBytes(csvContent));

            // Act
            var resultado = await _service.ImportarCsv(csvStream);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(1, resultado.Total);
            Assert.Equal(0, resultado.Sucesso);
            Assert.Equal(1, resultado.Erros);
            Assert.Single(resultado.Detalhes);
            Assert.Contains("CPF é obrigatório", resultado.Detalhes.First().Mensagem);

            _mediatorMock.Verify(m => m.Send(It.IsAny<CriarPessoaComando>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task ImportarCsv_ComNomeNulo_DeveRetornarErro()
        {
            // Arrange
            var csvContent = @"Nome,Email,CPF,DataNascimento,Telefone,Sexo,Naturalidade,Nacionalidade
"""",""joao@email.com"",""52998224725"",""1990-01-01"",""(11) 99999-9999"",""0"",""São Paulo"",""Brasil""";

            var csvStream = new MemoryStream(Encoding.UTF8.GetBytes(csvContent));

            _repositorioPessoaMock
                .Setup(r => r.CpfExisteAsync(It.IsAny<string>(), It.IsAny<Guid?>()))
                .ReturnsAsync(false);

            // Act
            var resultado = await _service.ImportarCsv(csvStream);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(1, resultado.Total);
            Assert.Equal(0, resultado.Sucesso);
            Assert.Equal(1, resultado.Erros);
            Assert.Single(resultado.Detalhes);
            Assert.Contains("Nome é obrigatório", resultado.Detalhes.First().Mensagem);

            _mediatorMock.Verify(m => m.Send(It.IsAny<CriarPessoaComando>(), It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}