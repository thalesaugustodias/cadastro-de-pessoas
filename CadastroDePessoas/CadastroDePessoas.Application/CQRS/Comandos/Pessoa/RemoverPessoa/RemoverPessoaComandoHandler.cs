using CadastroDePessoas.Application.Interfaces;
using CadastroDePessoas.Domain.Interfaces;

namespace CadastroDePessoas.Application.CQRS.Comandos.Pessoa.RemoverPessoa
{
    public class RemoverPessoaComandoHandler(IRepositorioPessoa repositorioPessoa, IServiceCache servicoCache) : IHandlerComando<RemoverPessoaComando, bool>
    {
        public async Task<bool> Handle(RemoverPessoaComando comando, CancellationToken cancellationToken)
        {
            var pessoa = await repositorioPessoa.ObterPorIdAsync(comando.Id) ?? throw new Exception("Pessoa não encontrada");

            await repositorioPessoa.RemoverAsync(pessoa);

            await servicoCache.RemoverAsync("pessoas_lista");
            await servicoCache.RemoverAsync($"pessoa_{comando.Id}");

            return true;
        }
    }
}
