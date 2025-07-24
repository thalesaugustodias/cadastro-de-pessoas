using CadastroDePessoas.Domain.Enums;

namespace CadastroDePessoas.Application.DTOs.Pessoa
{
    public class PessoaCriacaoDTO
    {
        public string Nome { get; set; }
        public Sexo? Sexo { get; set; }
        public string Email { get; set; }
        public DateTime DataNascimento { get; set; }
        public string Naturalidade { get; set; }
        public string Nacionalidade { get; set; }
        public string CPF { get; set; }
        public string Endereco { get; set; }
    }
}
