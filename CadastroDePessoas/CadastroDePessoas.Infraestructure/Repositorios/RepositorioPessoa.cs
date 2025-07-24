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
    }
}
