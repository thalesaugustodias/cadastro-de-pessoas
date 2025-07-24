using CadastroDePessoas.Application.DTOs.Pessoa;
using CadastroDePessoas.Domain.Enums;
using MediatR;

namespace CadastroDePessoas.Application.CQRS.Comandos.Pessoa.CriarPessoa
{
    public class CriarPessoaComando : IRequest<PessoaDTO>
    {
        public string Nome { get; set; }
        public Sexo? Sexo { get; set; }
        public string Email { get; set; }
        public DateTime DataNascimento { get; set; }
        public string Naturalidade { get; set; }
        public string Nacionalidade { get; set; }
        public string CPF { get; set; }
        public string Endereco { get; set; }

        public PessoaCriacaoDTO ParaDTO()
        {
            return new PessoaCriacaoDTO
            {
                Nome = Nome,
                Sexo = Sexo,
                Email = Email,
                DataNascimento = DataNascimento,
                Naturalidade = Naturalidade,
                Nacionalidade = Nacionalidade,
                CPF = CPF,
                Endereco = Endereco
            };
        }
    }
}
