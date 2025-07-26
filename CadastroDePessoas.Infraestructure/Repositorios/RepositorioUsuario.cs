using CadastroDePessoas.Domain.Entidades;
using CadastroDePessoas.Domain.Interfaces;
using CadastroDePessoas.Infraestructure.Contexto;
using Microsoft.EntityFrameworkCore;

namespace CadastroDePessoas.Infraestructure.Repositorios
{
    public class RepositorioUsuario(AppDbContexto contexto) : RepositorioBase<Usuario>(contexto), IRepositorioUsuario
    {
        public async Task<Usuario> ObterPorEmailAsync(string email)
        {
            return await _dbSet.FirstOrDefaultAsync(u => u.Email == email);
        }
    }
}
