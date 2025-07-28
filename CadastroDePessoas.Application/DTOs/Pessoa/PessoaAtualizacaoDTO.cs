using CadastroDePessoas.Application.DTOs.Endereco;
using CadastroDePessoas.Domain.Enums;

namespace CadastroDePessoas.Application.DTOs.Pessoa
{
    public class PessoaAtualizacaoDTO
    {
        public Guid Id { get; set; }
        public string Nome { get; set; }
        public Sexo? Sexo { get; set; }
        public string Email { get; set; }
        public DateTime DataNascimento { get; set; }
        public string Naturalidade { get; set; }
        public string Nacionalidade { get; set; }
        public string Telefone { get; set; }
        public EnderecoCriacaoDTO Endereco { get; set; }
    }
}
