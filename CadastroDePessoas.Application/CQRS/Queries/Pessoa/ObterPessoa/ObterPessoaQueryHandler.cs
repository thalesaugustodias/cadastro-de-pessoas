using CadastroDePessoas.Application.DTOs.Pessoa;
using CadastroDePessoas.Application.Factories;
using CadastroDePessoas.Application.Interfaces;
using CadastroDePessoas.Domain.Interfaces;

namespace CadastroDePessoas.Application.CQRS.Queries.Pessoa.ObterPessoa
{
    public class ObterPessoaQueryHandler(IRepositorioPessoa repositorioPessoa, IServiceCache servicoCache) : IHandlerConsulta<ObterPessoaQuery, PessoaDTO>
    {
        public async Task<PessoaDTO> Handle(ObterPessoaQuery consulta, CancellationToken cancellationToken)
        {
            var cacheKey = $"pessoa_{consulta.Id}";
            var pessoaCache = await servicoCache.ObterAsync<PessoaDTO>(cacheKey);
            if (pessoaCache != null)
                return pessoaCache;

            var pessoa = await repositorioPessoa.ObterPorIdAsync(consulta.Id) ?? throw new Exception("Pessoa não encontrada");
            var pessoaDto = PessoaFactory.CriarDTO(pessoa);

            await servicoCache.DefinirAsync(cacheKey, pessoaDto, TimeSpan.FromMinutes(5));

            return pessoaDto;
        }
    }
}
