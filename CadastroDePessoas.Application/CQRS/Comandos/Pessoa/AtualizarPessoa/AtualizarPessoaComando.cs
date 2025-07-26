using CadastroDePessoas.Application.DTOs.Pessoa;
using CadastroDePessoas.Domain.Enums;
using MediatR;

namespace CadastroDePessoas.Application.CQRS.Comandos.Pessoa.AtualizarPessoa
{
    public class AtualizarPessoaComando : IRequest<PessoaDTO>
    {
        public Guid Id { get; set; }
        public string Nome { get; set; }
        public Sexo? Sexo { get; set; }
        public string Email { get; set; }
        public DateTime DataNascimento { get; set; }
        public string Naturalidade { get; set; }
        public string Nacionalidade { get; set; }
        public string Endereco { get; set; }

        public PessoaAtualizacaoDTO ParaDTO()
        {
            return new PessoaAtualizacaoDTO
            {
                Id = Id,
                Nome = Nome,
                Sexo = Sexo,
                Email = Email,
                DataNascimento = DataNascimento,
                Naturalidade = Naturalidade,
                Nacionalidade = Nacionalidade,
                Endereco = Endereco
            };
        }
    }
}
