using CadastroDePessoas.Domain.Interfaces;
using CadastroDePessoas.Infraestructure.Contexto;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace CadastroDePessoas.Infraestructure.Repositorios
{
    public class RepositorioBase<T>(AppDbContexto contexto) : IRepositorioBase<T> where T : class
    {
        protected readonly AppDbContexto _contexto = contexto;
        protected readonly DbSet<T> _dbSet = contexto.Set<T>();

        public async Task<T> ObterPorIdAsync(Guid id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<IEnumerable<T>> ObterTodosAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<IEnumerable<T>> ObterAsync(Expression<Func<T, bool>> predicado)
        {
            return await _dbSet.Where(predicado).ToListAsync();
        }

        public async Task AdicionarAsync(T entidade)
        {
            await _dbSet.AddAsync(entidade);
            await _contexto.SaveChangesAsync();
        }

        public async Task AtualizarAsync(T entidade)
        {
            _dbSet.Update(entidade);
            await _contexto.SaveChangesAsync();
        }

        public async Task RemoverAsync(T entidade)
        {
            _dbSet.Remove(entidade);
            await _contexto.SaveChangesAsync();
        }

        public async Task<bool> ExisteAsync(Expression<Func<T, bool>> predicado)
        {
            return await _dbSet.AnyAsync(predicado);
        }
    }
}
