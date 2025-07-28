using CadastroDePessoas.Application.CQRS.Comandos.Pessoa.AtualizarPessoa;
using CadastroDePessoas.Application.CQRS.Comandos.Pessoa.CriarPessoa;
using CadastroDePessoas.Application.CQRS.Comandos.Pessoa.RemoverPessoa;
using CadastroDePessoas.Application.CQRS.Queries.Pessoa.ListarPessoa;
using CadastroDePessoas.Application.CQRS.Queries.Pessoa.ObterPessoa;
using CadastroDePessoas.Application.DTOs.Pessoa;
using CadastroDePessoas.Application.Services;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CadastroDePessoas.API.V2.Controllers
{
    [ApiController]
    [Route("api/v2/[controller]")]
    [Authorize]
    [Produces("application/json")]
    [Consumes("application/json")]
    public class PessoasController(IMediator mediator, ExportacaoService exportacaoService, ImportacaoService importacaoService) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PessoaDTO>>> ObterTodos()
        {
            var resultado = await mediator.Send(new ListarPessoasQuery());
            return Ok(resultado);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PessoaDTO>> ObterPorId(Guid id)
        {
            var resultado = await mediator.Send(new ObterPessoaQuery(id));
            return Ok(resultado);
        }

        [HttpPost]
        public async Task<ActionResult<PessoaDTO>> Criar(CriarPessoaComando comando)
        {            
            if (comando.Endereco == null)
            {
                throw new ValidationException(new[] { 
                    new FluentValidation.Results.ValidationFailure("Endereco", "O endereço é obrigatório na versão 2 da API") 
                });
            }

            var resultado = await mediator.Send(comando);
            return CreatedAtAction(nameof(ObterPorId), new { id = resultado.Id }, resultado);
        }

        [HttpPut]
        public async Task<ActionResult<PessoaDTO>> Atualizar(AtualizarPessoaComando comando)
        {
            if (comando.Endereco == null)
            {
                throw new ValidationException(new[] { 
                    new FluentValidation.Results.ValidationFailure("Endereco", "O endereço é obrigatório na versão 2 da API") 
                });
            }

            var resultado = await mediator.Send(comando);
            return Ok(resultado);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> Remover(Guid id)
        {
            var resultado = await mediator.Send(new RemoverPessoaComando(id));
            return Ok(resultado);
        }

        [HttpPost("exportar/excel")]
        [Produces("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")]
        [Consumes("application/json")]
        public async Task<IActionResult> ExportarExcel([FromBody] string[] campos)
        {
            if (campos == null || campos.Length == 0)
            {
                return BadRequest("Selecione pelo menos um campo para exportar");
            }

            var bytes = await exportacaoService.ExportarParaExcel(campos);
            return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"pessoas-{DateTime.Now:yyyy-MM-dd}.xlsx");
        }

        [HttpPost("exportar/pdf")]
        [Produces("application/pdf")]
        [Consumes("application/json")]
        public async Task<IActionResult> ExportarPdf([FromBody] string[] campos)
        {
            if (campos == null || campos.Length == 0)
            {
                return BadRequest("Selecione pelo menos um campo para exportar");
            }

            var bytes = await exportacaoService.ExportarParaPdf(campos);
            return File(bytes, "application/pdf", $"pessoas-{DateTime.Now:yyyy-MM-dd}.pdf");
        }

        [HttpGet("importar/template")]
        [Produces("text/csv")]
        public async Task<IActionResult> DownloadTemplateCsv()
        {
            var bytes = await exportacaoService.GerarTemplateCsv();
            return File(bytes, "text/csv", "template-importacao.csv");
        }

        [HttpPost("importar")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> ImportarCsv(IFormFile arquivo)
        {
            if (arquivo == null || arquivo.Length == 0)
            {
                return BadRequest("Nenhum arquivo fornecido");
            }

            if (!arquivo.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest("Apenas arquivos CSV são aceitos");
            }

            using var stream = arquivo.OpenReadStream();
            var resultado = await importacaoService.ImportarCsv(stream);

            return Ok(resultado);
        }
    }
}
