using CadastroDePessoas.Application.DTOs.Pessoa;
using CadastroDePessoas.Application.Factories;
using CadastroDePessoas.Application.Interfaces;
using CadastroDePessoas.Domain.Interfaces;

namespace CadastroDePessoas.Application.CQRS.Queries.Pessoa.ListarPessoa
{
    public class ListarPessoasQueryHandler(IRepositorioPessoa repositorioPessoa, IServiceCache servicoCache) : IHandlerConsulta<ListarPessoasQuery, IEnumerable<PessoaDTO>>
    {
        public async Task<IEnumerable<PessoaDTO>> Handle(ListarPessoasQuery consulta, CancellationToken cancellationToken)
        {
            var pessoasCache = await servicoCache.ObterAsync<IEnumerable<PessoaDTO>>("pessoas_lista");
            if (pessoasCache != null) return pessoasCache;

            var pessoas = await repositorioPessoa.ObterTodosAsync();
            var pessoasDto = pessoas.Select(PessoaFactory.CriarDTO).ToList();

            await servicoCache.DefinirAsync("pessoas_lista", pessoasDto, TimeSpan.FromMinutes(5));

            return pessoasDto;
        }
    }
}
