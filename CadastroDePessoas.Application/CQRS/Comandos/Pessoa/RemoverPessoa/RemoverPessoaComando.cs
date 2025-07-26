using MediatR;

namespace CadastroDePessoas.Application.CQRS.Comandos.Pessoa.RemoverPessoa
{
    public class RemoverPessoaComando(Guid id) : IRequest<bool>
    {
        public Guid Id { get; set; } = id;
    }
}
