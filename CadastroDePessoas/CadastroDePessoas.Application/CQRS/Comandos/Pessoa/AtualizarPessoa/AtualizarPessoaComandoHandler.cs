using CadastroDePessoas.Application.DTOs.Pessoa;
using CadastroDePessoas.Application.Factories;
using CadastroDePessoas.Application.Interfaces;
using CadastroDePessoas.Domain.Interfaces;

namespace CadastroDePessoas.Application.CQRS.Comandos.Pessoa.AtualizarPessoa
{
    public class AtualizarPessoaComandoHandler(IRepositorioPessoa repositorioPessoa, IServiceCache servicoCache) : IHandlerComando<AtualizarPessoaComando, PessoaDTO>
    {
        private readonly IServiceCache _servicoCache = servicoCache;

        public async Task<PessoaDTO> Handle(AtualizarPessoaComando comando, CancellationToken cancellationToken)
        {
            var pessoa = await repositorioPessoa.ObterPorIdAsync(comando.Id) ?? throw new Exception("Pessoa não encontrada");

            pessoa.Atualizar(comando.Nome,comando.Sexo,comando.Email,comando.DataNascimento,comando.Naturalidade,comando.Nacionalidade,comando.Endereco);

            await repositorioPessoa.AtualizarAsync(pessoa);

            await _servicoCache.RemoverAsync("pessoas_lista");
            await _servicoCache.RemoverAsync($"pessoa_{comando.Id}");

            return PessoaFactory.CriarDTO(pessoa);
        }
    }
}
