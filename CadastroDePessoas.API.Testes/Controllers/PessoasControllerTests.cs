using CadastroDePessoas.API.Controllers;
using CadastroDePessoas.Application.CQRS.Comandos.Pessoa.AtualizarPessoa;
using CadastroDePessoas.Application.CQRS.Comandos.Pessoa.CriarPessoa;
using CadastroDePessoas.Application.CQRS.Comandos.Pessoa.RemoverPessoa;
using CadastroDePessoas.Application.CQRS.Queries.Pessoa.ListarPessoa;
using CadastroDePessoas.Application.CQRS.Queries.Pessoa.ObterPessoa;
using CadastroDePessoas.Application.DTOs.Pessoa;
using CadastroDePessoas.Application.Servicos;
using CadastroDePessoas.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Text;

namespace CadastroDePessoas.API.Testes.Controllers
{
    public class PessoasControllerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<ExportacaoService> _exportacaoServiceMock;
        private readonly Mock<ImportacaoService> _importacaoServiceMock;
        private readonly PessoasController _controller;

        public PessoasControllerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _exportacaoServiceMock = new Mock<ExportacaoService>(Mock.Of<Domain.Interfaces.IRepositorioPessoa>());
            _importacaoServiceMock = new Mock<ImportacaoService>(
                Mock.Of<IMediator>(), 
                Mock.Of<Domain.Interfaces.IRepositorioPessoa>()
            );
            _controller = new PessoasController(
                _mediatorMock.Object,
                _exportacaoServiceMock.Object,
                _importacaoServiceMock.Object
            );
        }

        [Fact]
        public async Task ObterTodos_DeveRetornarOk()
        {
            // Arrange
            var pessoasLista = new List<PessoaDTO>
            {
                new PessoaDTO
                {
                    Id = Guid.NewGuid(),
                    Nome = "João Silva",
                    Sexo = Sexo.Masculino,
                    Email = "joao@exemplo.com",
                    DataNascimento = new DateTime(1990, 1, 1),
                    CPF = "52998224725"
                }
            };

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ListarPessoasQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(pessoasLista);

            // Act
            var resultado = await _controller.ObterTodos();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(resultado.Result);
            var pessoas = Assert.IsAssignableFrom<IEnumerable<PessoaDTO>>(okResult.Value);
            Assert.Single(pessoas);
        }

        [Fact]
        public async Task ObterPorId_DeveRetornarOk()
        {
            // Arrange
            var id = Guid.NewGuid();
            var pessoa = new PessoaDTO
            {
                Id = id,
                Nome = "João Silva",
                Sexo = Sexo.Masculino,
                Email = "joao@exemplo.com",
                DataNascimento = new DateTime(1990, 1, 1),
                CPF = "52998224725"
            };

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ObterPessoaQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(pessoa);

            // Act
            var resultado = await _controller.ObterPorId(id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(resultado.Result);
            var pessoaRetornada = Assert.IsType<PessoaDTO>(okResult.Value);
            Assert.Equal(id, pessoaRetornada.Id);
        }

        [Fact]
        public async Task Criar_DeveRetornarCreated()
        {
            // Arrange
            var comando = new CriarPessoaComando
            {
                Nome = "João Silva",
                Sexo = Sexo.Masculino,
                Email = "joao@exemplo.com",
                DataNascimento = new DateTime(1990, 1, 1),
                CPF = "52998224725"
            };

            var pessoaCriada = new PessoaDTO
            {
                Id = Guid.NewGuid(),
                Nome = comando.Nome,
                Sexo = comando.Sexo,
                Email = comando.Email,
                DataNascimento = comando.DataNascimento,
                CPF = comando.CPF
            };

            _mediatorMock
                .Setup(m => m.Send(comando, It.IsAny<CancellationToken>()))
                .ReturnsAsync(pessoaCriada);

            // Act
            var resultado = await _controller.Criar(comando);

            // Assert
            var createdAtResult = Assert.IsType<CreatedAtActionResult>(resultado.Result);
            var pessoa = Assert.IsType<PessoaDTO>(createdAtResult.Value);
            Assert.Equal(comando.Nome, pessoa.Nome);
            Assert.Equal("ObterPorId", createdAtResult.ActionName);
            Assert.Equal(pessoaCriada.Id, createdAtResult.RouteValues["id"]);
        }

        [Fact]
        public async Task Atualizar_DeveRetornarOk()
        {
            // Arrange
            var comando = new AtualizarPessoaComando
            {
                Id = Guid.NewGuid(),
                Nome = "João Silva Atualizado",
                Sexo = Sexo.Masculino,
                Email = "joao.atualizado@exemplo.com",
                DataNascimento = new DateTime(1990, 1, 1),
                Naturalidade = "Rio de Janeiro",
                Nacionalidade = "Brasileira"
            };

            var pessoaAtualizada = new PessoaDTO
            {
                Id = comando.Id,
                Nome = comando.Nome,
                Sexo = comando.Sexo,
                Email = comando.Email,
                DataNascimento = comando.DataNascimento,
                Naturalidade = comando.Naturalidade,
                Nacionalidade = comando.Nacionalidade
            };

            _mediatorMock
                .Setup(m => m.Send(comando, It.IsAny<CancellationToken>()))
                .ReturnsAsync(pessoaAtualizada);

            // Act
            var resultado = await _controller.Atualizar(comando);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(resultado.Result);
            var pessoa = Assert.IsType<PessoaDTO>(okResult.Value);
            Assert.Equal(comando.Nome, pessoa.Nome);
            Assert.Equal(comando.Email, pessoa.Email);
        }

        [Fact]
        public async Task Remover_DeveRetornarOk()
        {
            // Arrange
            var id = Guid.NewGuid();

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<RemoverPessoaComando>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var resultado = await _controller.Remover(id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(resultado.Result);
            var sucesso = Assert.IsType<bool>(okResult.Value);
            Assert.True(sucesso);
        }

        [Fact]
        public async Task ExportarExcel_DeveRetornarArquivo()
        {
            // Arrange
            var campos = new[] { "nome", "email", "cpf" };
            var bytes = Encoding.UTF8.GetBytes("conteúdo de teste");

            _exportacaoServiceMock
                .Setup(e => e.ExportarParaExcel(campos))
                .ReturnsAsync(bytes);

            // Act
            var resultado = await _controller.ExportarExcel(campos);

            // Assert
            var fileResult = Assert.IsType<FileContentResult>(resultado);
            Assert.Equal(bytes, fileResult.FileContents);
            Assert.Equal("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileResult.ContentType);
            Assert.Contains("pessoas-", fileResult.FileDownloadName);
            Assert.Contains(".xlsx", fileResult.FileDownloadName);
        }

        [Fact]
        public async Task ExportarPdf_DeveRetornarArquivo()
        {
            // Arrange
            var campos = new[] { "nome", "email", "cpf" };
            var bytes = Encoding.UTF8.GetBytes("conteúdo de teste");

            _exportacaoServiceMock
                .Setup(e => e.ExportarParaPdf(campos))
                .ReturnsAsync(bytes);

            // Act
            var resultado = await _controller.ExportarPdf(campos);

            // Assert
            var fileResult = Assert.IsType<FileContentResult>(resultado);
            Assert.Equal(bytes, fileResult.FileContents);
            Assert.Equal("application/pdf", fileResult.ContentType);
            Assert.Contains("pessoas-", fileResult.FileDownloadName);
            Assert.Contains(".pdf", fileResult.FileDownloadName);
        }

        [Fact]
        public async Task DownloadTemplateCsv_DeveRetornarArquivo()
        {
            // Arrange
            var bytes = Encoding.UTF8.GetBytes("conteúdo de teste");

            _exportacaoServiceMock
                .Setup(e => e.GerarTemplateCsv())
                .ReturnsAsync(bytes);

            // Act
            var resultado = await _controller.DownloadTemplateCsv();

            // Assert
            var fileResult = Assert.IsType<FileContentResult>(resultado);
            Assert.Equal(bytes, fileResult.FileContents);
            Assert.Equal("text/csv", fileResult.ContentType);
            Assert.Equal("template-importacao.csv", fileResult.FileDownloadName);
        }

        [Fact]
        public async Task ImportarCsv_DeveRetornarOk()
        {
            // Arrange
            var arquivo = new Mock<IFormFile>();
            var conteudo = "Nome,Email,CPF\nJoão,joao@exemplo.com,52998224725";
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(conteudo));
            
            arquivo.Setup(f => f.FileName).Returns("arquivo.csv");
            arquivo.Setup(f => f.Length).Returns(stream.Length);
            arquivo.Setup(f => f.OpenReadStream()).Returns(stream);
            
            var resultadoImportacao = new Application.Servicos.ImportacaoResultado
            {
                Total = 1,
                Sucesso = 1,
                Erros = 0,
                Detalhes = new List<Application.Servicos.DetalheErro>()
            };

            _importacaoServiceMock
                .Setup(i => i.ImportarCsv(It.IsAny<Stream>()))
                .ReturnsAsync(resultadoImportacao);

            // Act
            var resultado = await _controller.ImportarCsv(arquivo.Object);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(resultado);
            var importResult = Assert.IsType<Application.Servicos.ImportacaoResultado>(okResult.Value);
            Assert.Equal(1, importResult.Total);
            Assert.Equal(1, importResult.Sucesso);
            Assert.Equal(0, importResult.Erros);
        }

        [Fact]
        public async Task ImportarCsv_ComArquivoInvalido_DeveRetornarBadRequest()
        {
            // Arrange
            // Caso 1: Arquivo nulo
            IFormFile arquivoNulo = null;

            // Act & Assert para arquivo nulo
            var resultadoNulo = await _controller.ImportarCsv(arquivoNulo);
            Assert.IsType<BadRequestObjectResult>(resultadoNulo);

            // Caso 2: Arquivo vazio
            var arquivoVazio = new Mock<IFormFile>();
            arquivoVazio.Setup(f => f.Length).Returns(0);

            // Act & Assert para arquivo vazio
            var resultadoVazio = await _controller.ImportarCsv(arquivoVazio.Object);
            Assert.IsType<BadRequestObjectResult>(resultadoVazio);

            // Caso 3: Arquivo não-CSV
            var arquivoInvalido = new Mock<IFormFile>();
            arquivoInvalido.Setup(f => f.FileName).Returns("arquivo.txt");
            arquivoInvalido.Setup(f => f.Length).Returns(100);

            // Act & Assert para arquivo não-CSV
            var resultadoInvalido = await _controller.ImportarCsv(arquivoInvalido.Object);
            Assert.IsType<BadRequestObjectResult>(resultadoInvalido);
        }
    }
}