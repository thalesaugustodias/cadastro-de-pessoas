using CadastroDePessoas.Application.DTOs.Pessoa;
using MediatR;

namespace CadastroDePessoas.Application.CQRS.Queries.Pessoa.ListarPessoa
{
    public class ListarPessoasQuery : IRequest<IEnumerable<PessoaDTO>>
    {
    }
}
