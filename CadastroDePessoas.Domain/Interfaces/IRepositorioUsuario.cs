using CadastroDePessoas.Domain.Entidades;

namespace CadastroDePessoas.Domain.Interfaces
{
    public interface IRepositorioUsuario : IRepositorioBase<Usuario>
    {
        Task<Usuario> ObterPorEmailAsync(string email);
    }
}
