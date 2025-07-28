using System.Text;
    using CadastroDePessoas.API.Controllers;
using CadastroDePessoas.Application.CQRS.Comandos.Pessoa.AtualizarPessoa;
using CadastroDePessoas.Application.CQRS.Comandos.Pessoa.CriarPessoa;
using CadastroDePessoas.Application.CQRS.Comandos.Pessoa.RemoverPessoa;
using CadastroDePessoas.Application.CQRS.Queries.Pessoa.ListarPessoa;
using CadastroDePessoas.Application.CQRS.Queries.Pessoa.ObterPessoa;
using CadastroDePessoas.Application.DTOs.Pessoa;
using CadastroDePessoas.Application.Interfaces;
using CadastroDePessoas.Application.Services;
using CadastroDePessoas.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace CadastroDePessoas.API.Testes.Controllers
{
    public class PessoasControllerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<ExportacaoService> _exportacaoServiceMock;
        private readonly Mock<ImportacaoService> _importacaoServiceMock;
        private readonly Mock<ILogger<PessoasController>> _loggerMock;
        private readonly Mock<IServiceCache> _serviceCacheMock;
        private readonly PessoasController _controller;

        public PessoasControllerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _exportacaoServiceMock = new Mock<ExportacaoService>(Mock.Of<Domain.Interfaces.IRepositorioPessoa>());
            _importacaoServiceMock = new Mock<ImportacaoService>(
                Mock.Of<IMediator>(), 
                Mock.Of<Domain.Interfaces.IRepositorioPessoa>()
            );
            _loggerMock = new Mock<ILogger<PessoasController>>();
            _serviceCacheMock = new Mock<IServiceCache>();
            
            _controller = new PessoasController(
                _mediatorMock.Object,
                _exportacaoServiceMock.Object,
                _importacaoServiceMock.Object,
                _loggerMock.Object,
                _serviceCacheMock.Object
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

            _serviceCacheMock
                .Setup(c => c.RemoverAsync(It.Is<string>(s => s == "pessoas_lista")))
                .Returns(Task.CompletedTask);

            // Act
            var resultado = await _controller.ObterTodos();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(resultado.Result);
            var pessoas = Assert.IsAssignableFrom<IEnumerable<PessoaDTO>>(okResult.Value);
            Assert.Single(pessoas);
            
            _serviceCacheMock.Verify(c => c.RemoverAsync(It.Is<string>(s => s == "pessoasListaCacheKey")), Times.Once);
        }

        [Fact]
        public async Task ObterTodos_QuandoOcorreExcecao_DeveRetornarStatusCode500()
        {
            // Arrange
            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ListarPessoasQuery>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Erro ao buscar pessoas"));

            _serviceCacheMock
                .Setup(c => c.RemoverAsync(It.Is<string>(s => s == "pessoas_lista")))
                .Returns(Task.CompletedTask);

            // Act
            var resultado = await _controller.ObterTodos();

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(resultado.Result);
            Assert.Equal(500, statusCodeResult.StatusCode);
        }

        [Fact]
        public async Task ObterPorId_DeveRetornarOk()
        {
            // Arrange
            var id = Guid.NewGuid();
            var chaveCache = $"pessoa_{id}";
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

            _serviceCacheMock
                .Setup(c => c.RemoverAsync(It.Is<string>(s => s == chaveCache)))
                .Returns(Task.CompletedTask);

            // Act
            var resultado = await _controller.ObterPorId(id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(resultado.Result);
            var pessoaRetornada = Assert.IsType<PessoaDTO>(okResult.Value);
            Assert.Equal(id, pessoaRetornada.Id);
            
            _serviceCacheMock.Verify(c => c.RemoverAsync(It.Is<string>(s => s == chaveCache)), Times.Once);
            
            _loggerMock.Verify(
                x => x.Log(
                    It.IsAny<LogLevel>(),
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => true),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
                Times.Once);
        }

        [Fact]
        public async Task ObterPorId_QuandoPessoaNaoEncontrada_DeveRetornarNotFound()
        {
            // Arrange
            var id = Guid.NewGuid();
            var chaveCache = $"pessoa_{id}";

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ObterPessoaQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((PessoaDTO?)null);

            _serviceCacheMock
                .Setup(c => c.RemoverAsync(It.Is<string>(s => s == chaveCache)))
                .Returns(Task.CompletedTask);

            // Act
            var resultado = await _controller.ObterPorId(id);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(resultado.Result);
            Assert.Contains($"ID {id}", notFoundResult.Value?.ToString() ?? string.Empty);
        }

        [Fact]
        public async Task ObterPorId_QuandoOcorreExcecao_DeveRetornarStatusCode500()
        {
            // Arrange
            var id = Guid.NewGuid();
            var chaveCache = $"pessoa_{id}";

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ObterPessoaQuery>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Erro ao buscar pessoa"));

            _serviceCacheMock
                .Setup(c => c.RemoverAsync(It.Is<string>(s => s == chaveCache)))
                .Returns(Task.CompletedTask);

            // Act
            var resultado = await _controller.ObterPorId(id);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(resultado.Result);
            Assert.Equal(500, statusCodeResult.StatusCode);
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

            _serviceCacheMock
                .Setup(c => c.RemoverAsync(It.Is<string>(s => s == "pessoas_lista")))
                .Returns(Task.CompletedTask);

            // Act
            var resultado = await _controller.Criar(comando);

            // Assert
            var createdAtResult = Assert.IsType<CreatedAtActionResult>(resultado.Result);
            var pessoa = Assert.IsType<PessoaDTO>(createdAtResult.Value);
            Assert.Equal(comando.Nome, pessoa.Nome);
            Assert.Equal("ObterPorId", createdAtResult.ActionName);
            
            if (createdAtResult.RouteValues != null && createdAtResult.RouteValues.TryGetValue("id", out var routeValue))
            {
                Assert.Equal(pessoaCriada.Id, routeValue);
            }
            else
            {
                Assert.Fail("RouteValues['id'] não encontrado");
            }
            
            _serviceCacheMock.Verify(c => c.RemoverAsync(It.Is<string>(s => s == "pessoas_lista")), Times.Once);
        }

        [Fact]
        public async Task Criar_QuandoOcorreExcecao_DeveRetornarBadRequest()
        {
            // Arrange
            var comando = new CriarPessoaComando
            {
                Nome = "João Silva",
                CPF = "CPF_Invalido"
            };

            _mediatorMock
                .Setup(m => m.Send(comando, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("CPF inválido"));

            // Act
            var resultado = await _controller.Criar(comando);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(resultado.Result);
            Assert.Contains("Erro ao criar pessoa", badRequestResult.Value?.ToString() ?? string.Empty);
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

            var chaveCachePessoa = $"pessoa_{comando.Id}";
            var chaveCacheLista = "pessoas_lista";

            _mediatorMock
                .Setup(m => m.Send(comando, It.IsAny<CancellationToken>()))
                .ReturnsAsync(pessoaAtualizada);

            _serviceCacheMock
                .Setup(c => c.RemoverAsync(It.Is<string>(s => s == chaveCachePessoa)))
                .Returns(Task.CompletedTask);
                
            _serviceCacheMock
                .Setup(c => c.RemoverAsync(It.Is<string>(s => s == chaveCacheLista)))
                .Returns(Task.CompletedTask);

            // Act
            var resultado = await _controller.Atualizar(comando);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(resultado.Result);
            var pessoa = Assert.IsType<PessoaDTO>(okResult.Value);
            Assert.Equal(comando.Nome, pessoa.Nome);
            Assert.Equal(comando.Email, pessoa.Email);
            
            _serviceCacheMock.Verify(c => c.RemoverAsync(It.Is<string>(s => s == chaveCachePessoa)), Times.Once);
            _serviceCacheMock.Verify(c => c.RemoverAsync(It.Is<string>(s => s == chaveCacheLista)), Times.Once);
        }

        [Fact]
        public async Task Atualizar_QuandoOcorreExcecao_DeveRetornarBadRequest()
        {
            // Arrange
            var comando = new AtualizarPessoaComando
            {
                Id = Guid.NewGuid(),
                Nome = "João Silva Atualizado"
            };

            _mediatorMock
                .Setup(m => m.Send(comando, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Pessoa não encontrada"));

            // Act
            var resultado = await _controller.Atualizar(comando);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(resultado.Result);
            Assert.Contains("Erro ao atualizar pessoa", badRequestResult.Value?.ToString() ?? string.Empty);
        }

        [Fact]
        public async Task Remover_DeveRetornarOk()
        {
            // Arrange
            var id = Guid.NewGuid();
            var chaveCachePessoa = $"pessoa_{id}";
            var chaveCacheLista = "pessoas_lista";

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<RemoverPessoaComando>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            _serviceCacheMock
                .Setup(c => c.RemoverAsync(It.Is<string>(s => s == chaveCachePessoa)))
                .Returns(Task.CompletedTask);
                
            _serviceCacheMock
                .Setup(c => c.RemoverAsync(It.Is<string>(s => s == chaveCacheLista)))
                .Returns(Task.CompletedTask);

            // Act
            var resultado = await _controller.Remover(id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(resultado.Result);
            var sucesso = Assert.IsType<bool>(okResult.Value);
            Assert.True(sucesso);
            
            _serviceCacheMock.Verify(c => c.RemoverAsync(It.Is<string>(s => s == chaveCachePessoa)), Times.Once);
            _serviceCacheMock.Verify(c => c.RemoverAsync(It.Is<string>(s => s == chaveCacheLista)), Times.Once);
        }

        [Fact]
        public async Task Remover_QuandoOcorreExcecao_DeveRetornarBadRequest()
        {
            // Arrange
            var id = Guid.NewGuid();

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<RemoverPessoaComando>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Pessoa não encontrada"));

            // Act
            var resultado = await _controller.Remover(id);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(resultado.Result);
            Assert.Contains("Erro ao remover pessoa", badRequestResult.Value?.ToString() ?? string.Empty);
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
        public async Task ExportarExcel_ComCamposVazios_DeveRetornarBadRequest()
        {
            // Arrange
            string[]? camposNulos = null;

            // Act
            var resultado = await _controller.ExportarExcel(camposNulos!);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(resultado);
            Assert.Contains("Selecione pelo menos um campo", badRequestResult.Value?.ToString() ?? string.Empty);
            
            // Testar também com array vazio
            var resultado2 = await _controller.ExportarExcel(new string[0]);
            var badRequestResult2 = Assert.IsType<BadRequestObjectResult>(resultado2);
            Assert.Contains("Selecione pelo menos um campo", badRequestResult2.Value?.ToString() ?? string.Empty);
        }

        [Fact]
        public async Task ExportarExcel_QuandoOcorreExcecao_DeveRetornarStatusCode500()
        {
            // Arrange
            var campos = new[] { "nome", "email", "cpf" };

            _exportacaoServiceMock
                .Setup(e => e.ExportarParaExcel(campos))
                .ThrowsAsync(new Exception("Erro ao gerar Excel"));

            // Act
            var resultado = await _controller.ExportarExcel(campos);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(resultado);
            Assert.Equal(500, statusCodeResult.StatusCode);
            Assert.Contains("Erro ao exportar para Excel", statusCodeResult.Value?.ToString() ?? string.Empty);
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
        public async Task ExportarPdf_ComCamposVazios_DeveRetornarBadRequest()
        {
            // Arrange
            string[]? camposNulos = null;

            // Act
            var resultado = await _controller.ExportarPdf(camposNulos!);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(resultado);
            Assert.Contains("Selecione pelo menos um campo", badRequestResult.Value?.ToString() ?? string.Empty);
        }

        [Fact]
        public async Task ExportarPdf_QuandoOcorreExcecao_DeveRetornarStatusCode500()
        {
            // Arrange
            var campos = new[] { "nome", "email", "cpf" };

            _exportacaoServiceMock
                .Setup(e => e.ExportarParaPdf(campos))
                .ThrowsAsync(new Exception("Erro ao gerar PDF"));

            // Act
            var resultado = await _controller.ExportarPdf(campos);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(resultado);
            Assert.Equal(500, statusCodeResult.StatusCode);
            Assert.Contains("Erro ao exportar para PDF", statusCodeResult.Value?.ToString() ?? string.Empty);
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
        public async Task DownloadTemplateCsv_QuandoOcorreExcecao_DeveRetornarStatusCode500()
        {
            // Arrange
            _exportacaoServiceMock
                .Setup(e => e.GerarTemplateCsv())
                .ThrowsAsync(new Exception("Erro ao gerar template"));

            // Act
            var resultado = await _controller.DownloadTemplateCsv();

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(resultado);
            Assert.Equal(500, statusCodeResult.StatusCode);
            Assert.Contains("Erro ao gerar template CSV", statusCodeResult.Value?.ToString() ?? string.Empty);
        }

        [Fact]
        public async Task ImportarCsv_DeveRetornarOk()
        {
            // Arrange
            var arquivo = new Mock<IFormFile>();
            var conteudo = "Nome,Email,CPF\nJoão,joao@exemplo.com,52998224725";
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(conteudo));
            var chaveCacheLista = "pessoas_lista";
            
            arquivo.Setup(f => f.FileName).Returns("arquivo.csv");
            arquivo.Setup(f => f.Length).Returns(stream.Length);
            arquivo.Setup(f => f.OpenReadStream()).Returns(stream);
            
            var resultadoImportacao = new Application.DTOs.ImportacaoResultado
            {
                Total = 1,
                Sucesso = 1,
                Erros = 0,
                Detalhes = new List<Application.DTOs.DetalheErro>()
            };

            _importacaoServiceMock
                .Setup(i => i.ImportarCsv(It.IsAny<Stream>()))
                .ReturnsAsync(resultadoImportacao);

            _serviceCacheMock
                .Setup(c => c.RemoverAsync(It.Is<string>(s => s == chaveCacheLista)))
                .Returns(Task.CompletedTask);

            // Act
            var resultado = await _controller.ImportarCsv(arquivo.Object);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(resultado);
            var importResult = Assert.IsType<Application.DTOs.ImportacaoResultado>(okResult.Value);
            Assert.Equal(1, importResult.Total);
            Assert.Equal(1, importResult.Sucesso);
            Assert.Equal(0, importResult.Erros);            
            
            _serviceCacheMock.Verify(c => c.RemoverAsync(It.Is<string>(s => s == chaveCacheLista)), Times.Once);
        }

        private void TesteArquivoNulo()
        {
            // Caso 1: Arquivo nulo
            IFormFile? arquivoNulo = null;
            
            var resultadoNulo = _controller.ImportarCsv(arquivoNulo!).GetAwaiter().GetResult();
            Assert.IsType<BadRequestObjectResult>(resultadoNulo);
        }

        private void TesteArquivoVazio()
        {
            var arquivoVazio = new Mock<IFormFile>();
            arquivoVazio.Setup(f => f.Length).Returns(0);
            
            var resultadoVazio = _controller.ImportarCsv(arquivoVazio.Object).GetAwaiter().GetResult();
            Assert.IsType<BadRequestObjectResult>(resultadoVazio);
        }

        private void TesteArquivoNaoCsv()
        {
            // Caso 3: Arquivo não-CSV
            var arquivoInvalido = new Mock<IFormFile>();
            arquivoInvalido.Setup(f => f.FileName).Returns("arquivo.txt");
            arquivoInvalido.Setup(f => f.Length).Returns(100);
            
            var resultadoInvalido = _controller.ImportarCsv(arquivoInvalido.Object).GetAwaiter().GetResult();
            Assert.IsType<BadRequestObjectResult>(resultadoInvalido);
        }

        [Fact]
        public void ImportarCsv_ComArquivoInvalido_DeveRetornarBadRequest()
        {
            TesteArquivoNulo();
            TesteArquivoVazio();
            TesteArquivoNaoCsv();
        }

        [Fact]
        public async Task ImportarCsv_QuandoOcorreExcecao_DeveRetornarStatusCode500()
        {
            // Arrange
            var arquivo = new Mock<IFormFile>();
            var conteudo = "Nome,Email,CPF\nJoão,joao@exemplo.com,52998224725";
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(conteudo));
            
            arquivo.Setup(f => f.FileName).Returns("arquivo.csv");
            arquivo.Setup(f => f.Length).Returns(stream.Length);
            arquivo.Setup(f => f.OpenReadStream()).Returns(stream);

            _importacaoServiceMock
                .Setup(i => i.ImportarCsv(It.IsAny<Stream>()))
                .ThrowsAsync(new Exception("Erro ao importar CSV"));

            // Act
            var resultado = await _controller.ImportarCsv(arquivo.Object);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(resultado);
            Assert.Equal(500, statusCodeResult.StatusCode);
            Assert.Contains("Erro ao importar CSV", statusCodeResult.Value?.ToString() ?? string.Empty);
        }

        [Fact]
        public async Task LimparCache_DeveRetornarOk()
        {
            // Arrange
            var chaveCacheLista = "pessoas_lista";
            
            _serviceCacheMock
                .Setup(c => c.RemoverAsync(It.Is<string>(s => s == chaveCacheLista)))
                .Returns(Task.CompletedTask);

            // Act
            var resultado = await _controller.LimparCache();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(resultado);
            var response = Assert.IsAssignableFrom<object>(okResult.Value);
            
            var properties = response.GetType().GetProperties();
            Assert.Contains(properties, p => p.Name == "message");
            
            _serviceCacheMock.Verify(c => c.RemoverAsync(It.Is<string>(s => s == chaveCacheLista)), Times.Once);
        }

        [Fact]
        public async Task LimparCache_QuandoOcorreExcecao_DeveRetornarStatusCode500()
        {
            // Arrange
            var chaveCacheLista = "pessoas_lista";
            
            _serviceCacheMock
                .Setup(c => c.RemoverAsync(It.Is<string>(s => s == chaveCacheLista)))
                .ThrowsAsync(new Exception("Erro ao limpar cache"));

            // Act
            var resultado = await _controller.LimparCache();

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(resultado);
            Assert.Equal(500, statusCodeResult.StatusCode);
            Assert.Contains("Erro ao limpar cache", statusCodeResult.Value?.ToString() ?? string.Empty);
        }
    }
}