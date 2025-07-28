using CadastroDePessoas.Domain.Entidades;
using CadastroDePessoas.Domain.Interfaces;
using CadastroDePessoas.Infraestructure.Contexto;
using Microsoft.EntityFrameworkCore;

namespace CadastroDePessoas.Infraestructure.Repositorios
{
    public class RepositorioPessoa(AppDbContexto contexto) : RepositorioBase<Pessoa>(contexto), IRepositorioPessoa
    {
        public async Task<bool> CpfExisteAsync(string cpf, Guid? ignorarId = null)
        {
            if (ignorarId.HasValue)
            {
                return await _dbSet.AnyAsync(p => p.CPF == cpf && p.Id != ignorarId.Value);
            }

            return await _dbSet.AnyAsync(p => p.CPF == cpf);
        }
        
        public override async Task<Pessoa> ObterPorIdAsync(Guid id)
        {
            return await _dbSet
                .Include(p => p.Endereco)
                .FirstOrDefaultAsync(p => p.Id == id);
        }
        
        public override async Task<IEnumerable<Pessoa>> ObterTodosAsync()
        {
            return await _dbSet
                .Include(p => p.Endereco)
                .ToListAsync();
        }
        
        public override async Task<IEnumerable<Pessoa>> ObterAsync(System.Linq.Expressions.Expression<Func<Pessoa, bool>> predicado)
        {
            return await _dbSet
                .Include(p => p.Endereco)
                .Where(predicado)
                .ToListAsync();
        }
        
        public async Task AtualizarPessoaComEnderecoAsync(Pessoa entidade)
        {
            var pessoaExistente = await _contexto.Pessoas
                .Include(p => p.Endereco)
                .FirstOrDefaultAsync(p => p.Id == entidade.Id);
                
            if (pessoaExistente == null)
            {
                throw new Exception($"Pessoa com ID {entidade.Id} não encontrada para atualização");
            }
            
            _contexto.Entry(pessoaExistente).State = EntityState.Detached;
            
            _contexto.Update(entidade);
            
            await _contexto.SaveChangesAsync();
        }
    }
}
