using CadastroDePessoas.Domain.Enums;

namespace CadastroDePessoas.Application.DTOs.Pessoa
{
    public class PessoaDTO
    {
        public Guid Id { get; set; }
        public string Nome { get; set; }
        public Sexo? Sexo { get; set; }
        public string SexoDescricao => Sexo?.ToString();
        public string Email { get; set; }
        public DateTime DataNascimento { get; set; }
        public string Naturalidade { get; set; }
        public string Nacionalidade { get; set; }
        public string CPF { get; set; }
        public string Endereco { get; set; }
        public DateTime DataCadastro { get; set; }
        public DateTime? DataAtualizacao { get; set; }
        public int Idade => CalcularIdade();

        private int CalcularIdade()
        {
            var hoje = DateTime.Today;
            var idade = hoje.Year - DataNascimento.Year;
            if (DataNascimento.Date > hoje.AddYears(-idade)) idade--;
            return idade;
        }
    }
}
