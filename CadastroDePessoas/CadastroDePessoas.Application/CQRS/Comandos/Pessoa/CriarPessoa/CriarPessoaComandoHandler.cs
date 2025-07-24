using CadastroDePessoas.Application.DTOs.Pessoa;
using CadastroDePessoas.Application.Factories;
using CadastroDePessoas.Application.Interfaces;
using CadastroDePessoas.Domain.Interfaces;

namespace CadastroDePessoas.Application.CQRS.Comandos.Pessoa.CriarPessoa
{
    public class CriarPessoaComandoHandler(IRepositorioPessoa repositorioPessoa, IServiceCache servicoCache) : IHandlerComando<CriarPessoaComando, PessoaDTO>
    {
        public async Task<PessoaDTO> Handle(CriarPessoaComando comando, CancellationToken cancellationToken)
        {
            if (await repositorioPessoa.CpfExisteAsync(comando.CPF))
                throw new System.Exception("CPF já está em uso");

            var pessoaDto = comando.ParaDTO();
            var pessoa = PessoaFactory.CriarEntidade(pessoaDto);

            await repositorioPessoa.AdicionarAsync(pessoa);

            await servicoCache.RemoverAsync("pessoas_lista");

            return PessoaFactory.CriarDTO(pessoa);
        }
    }
}
