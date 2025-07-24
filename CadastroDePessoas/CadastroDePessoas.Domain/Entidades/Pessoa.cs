using CadastroDePessoas.Domain.Enums;

namespace CadastroDePessoas.Domain.Entidades
{
    public class Pessoa
    {
        public Guid Id { get; private set; }
        public string Nome { get; private set; }
        public Sexo? Sexo { get; private set; }
        public string Email { get; private set; }
        public DateTime DataNascimento { get; private set; }
        public string Naturalidade { get; private set; }
        public string Nacionalidade { get; private set; }
        public string CPF { get; private set; }
        public string Endereco { get; private set; }
        public DateTime DataCadastro { get; private set; }
        public DateTime? DataAtualizacao { get; private set; }

        public Pessoa(string nome,Sexo? sexo,string email,DateTime dataNascimento,string naturalidade,string nacionalidade,string cpf,string endereco = null)
        {
            Id = Guid.NewGuid();
            Nome = nome;
            Sexo = sexo;
            Email = email;
            DataNascimento = dataNascimento;
            Naturalidade = naturalidade;
            Nacionalidade = nacionalidade;
            CPF = cpf;
            Endereco = endereco;
            DataCadastro = DateTime.UtcNow;
        }

        protected Pessoa() { }

        public void Atualizar(string nome,Sexo? sexo,string email,DateTime dataNascimento,string naturalidade,string nacionalidade,string endereco = null)
        {
            Nome = nome;
            Sexo = sexo;
            Email = email;
            DataNascimento = dataNascimento;
            Naturalidade = naturalidade;
            Nacionalidade = nacionalidade;
            if (endereco != null)
            {
                Endereco = endereco;
            }
            DataAtualizacao = DateTime.UtcNow;
        }
    }
}
