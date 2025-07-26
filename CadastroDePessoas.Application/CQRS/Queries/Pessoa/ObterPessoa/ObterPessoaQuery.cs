using CadastroDePessoas.Application.DTOs.Pessoa;
using MediatR;

namespace CadastroDePessoas.Application.CQRS.Queries.Pessoa.ObterPessoa
{
    public class ObterPessoaQuery(Guid id) : IRequest<PessoaDTO>
    {
        public Guid Id { get; set; } = id;
    }
}
