using CadastroDePessoas.Application.CQRS.Comandos.Pessoa.AtualizarPessoa;
using CadastroDePessoas.Application.CQRS.Comandos.Pessoa.CriarPessoa;
using CadastroDePessoas.Application.CQRS.Comandos.Pessoa.RemoverPessoa;
using CadastroDePessoas.Application.CQRS.Queries.Pessoa.ListarPessoa;
using CadastroDePessoas.Application.CQRS.Queries.Pessoa.ObterPessoa;
using CadastroDePessoas.Application.DTOs.Pessoa;
using CadastroDePessoas.Application.Interfaces;
using CadastroDePessoas.Application.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CadastroDePessoas.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Authorize]
    public class PessoasController(IMediator mediator,ExportacaoService exportacaoService,ImportacaoService importacaoService,ILogger<PessoasController> logger,IServiceCache serviceCache) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PessoaDTO>>> ObterTodos()
        {
            try
            {
                await serviceCache.RemoverAsync("pessoas_lista");                
                var resultado = await mediator.Send(new ListarPessoasQuery());
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno ao buscar pessoas", detail = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PessoaDTO>> ObterPorId(Guid id)
        {
            logger.LogInformation("Requisição recebida para ObterPorId: {Id}", id);
            try
            {
                await serviceCache.RemoverAsync($"pessoa_{id}");
                
                var resultado = await mediator.Send(new ObterPessoaQuery(id));
                if (resultado == null)
                {
                    return NotFound(new { message = $"Pessoa com ID {id} não encontrada" });
                }
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno ao buscar pessoa", detail = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult<PessoaDTO>> Criar(CriarPessoaComando comando)
        {
            try
            {
                var resultado = await mediator.Send(comando);
                await serviceCache.RemoverAsync("pessoas_lista");
                
                return CreatedAtAction(nameof(ObterPorId), new { id = resultado.Id }, resultado);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Erro ao criar pessoa", detail = ex.Message });
            }
        }

        [HttpPut]
        public async Task<ActionResult<PessoaDTO>> Atualizar(AtualizarPessoaComando comando)
        {
            try
            {
                var resultado = await mediator.Send(comando);                
                await serviceCache.RemoverAsync($"pessoa_{comando.Id}");
                await serviceCache.RemoverAsync("pessoas_lista");                
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Erro ao atualizar pessoa", detail = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> Remover(Guid id)
        {
            try
            {
                var resultado = await mediator.Send(new RemoverPessoaComando(id));                
                await serviceCache.RemoverAsync($"pessoa_{id}");
                await serviceCache.RemoverAsync("pessoas_lista");                
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Erro ao remover pessoa", detail = ex.Message });
            }
        }

        [HttpPost("exportar/excel")]
        public async Task<IActionResult> ExportarExcel([FromBody] string[] campos)
        {
            if (campos == null || campos.Length == 0)
            {
                return BadRequest(new { message = "Selecione pelo menos um campo para exportar" });
            }

            try
            {
                var bytes = await exportacaoService.ExportarParaExcel(campos);
                return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"pessoas-{DateTime.Now:yyyy-MM-dd}.xlsx");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao exportar para Excel", detail = ex.Message });
            }
        }

        [HttpPost("exportar/pdf")]
        public async Task<IActionResult> ExportarPdf([FromBody] string[] campos)
        {
            if (campos == null || campos.Length == 0)
            {
                return BadRequest(new { message = "Selecione pelo menos um campo para exportar" });
            }

            try
            {
                var bytes = await exportacaoService.ExportarParaPdf(campos);
                return File(bytes, "application/pdf", $"pessoas-{DateTime.Now:yyyy-MM-dd}.pdf");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao exportar para PDF", detail = ex.Message });
            }
        }

        [HttpGet("importar/template")]
        public async Task<IActionResult> DownloadTemplateCsv()
        {
            try
            {
                var bytes = await exportacaoService.GerarTemplateCsv();
                return File(bytes, "text/csv", "template-importacao.csv");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao gerar template CSV", detail = ex.Message });
            }
        }

        [HttpPost("importar")]
        public async Task<IActionResult> ImportarCsv(IFormFile arquivo)
        {
            if (arquivo == null || arquivo.Length == 0)
            {
                return BadRequest(new { message = "Nenhum arquivo fornecido" });
            }

            if (!arquivo.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest(new { message = "Apenas arquivos CSV são aceitos" });
            }

            try
            {
                using var stream = arquivo.OpenReadStream();
                var resultado = await importacaoService.ImportarCsv(stream);                
                await serviceCache.RemoverAsync("pessoas_lista");                
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao importar CSV", detail = ex.Message });
            }
        }
        
        [HttpPost("limpar-cache")]
        [AllowAnonymous]
        public async Task<IActionResult> LimparCache()
        {
            try
            {
                await serviceCache.RemoverAsync("pessoas_lista");
                return Ok(new { message = "Cache limpo com sucesso" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao limpar cache", detail = ex.Message });
            }
        }
    }
}
