using CadastroDePessoas.Application.CQRS.Comandos.Pessoa.AtualizarPessoa;
using CadastroDePessoas.Application.CQRS.Comandos.Pessoa.CriarPessoa;
using CadastroDePessoas.Application.CQRS.Comandos.Pessoa.RemoverPessoa;
using CadastroDePessoas.Application.CQRS.Queries.Pessoa.ListarPessoa;
using CadastroDePessoas.Application.CQRS.Queries.Pessoa.ObterPessoa;
using CadastroDePessoas.Application.DTOs.Pessoa;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CadastroDePessoas.API.V2.Controllers
{
    [ApiController]
    [Route("api/v2/[controller]")]
    [Authorize]
    public class PessoasController(IMediator mediator) : ControllerBase
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
            if (string.IsNullOrEmpty(comando.Endereco))
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
            if (string.IsNullOrEmpty(comando.Endereco))
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
    }
}
