using CadastroDePessoas.Domain.Entidades;

namespace CadastroDePessoas.Domain.Interfaces
{
    public interface IRepositorioPessoa : IRepositorioBase<Pessoa>
    {
        Task<bool> CpfExisteAsync(string cpf, Guid? ignorarId = null);
        
        Task AtualizarPessoaComEnderecoAsync(Pessoa pessoa);
    }
}
