using CadastroDePessoas.Application.DTOs.Endereco;
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
        public string Telefone { get; set; }
        public EnderecoAtualizacaoComando Endereco { get; set; }

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
                Telefone = Telefone,
                Endereco = Endereco?.ParaDTO()
            };
        }
    }

    public class EnderecoAtualizacaoComando
    {
        public string CEP { get; set; }
        public string Logradouro { get; set; }
        public string Numero { get; set; }
        public string Complemento { get; set; }
        public string Bairro { get; set; }
        public string Cidade { get; set; }
        public string Estado { get; set; }

        public EnderecoCriacaoDTO ParaDTO()
        {
            return new EnderecoCriacaoDTO
            {
                CEP = CEP,
                Logradouro = Logradouro,
                Numero = Numero,
                Complemento = Complemento,
                Bairro = Bairro,
                Cidade = Cidade,
                Estado = Estado
            };
        }
    }
}
