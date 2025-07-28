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
        public string Telefone { get; private set; }
        public Endereco Endereco { get; private set; }
        public DateTime DataCadastro { get; private set; }
        public DateTime? DataAtualizacao { get; private set; }

        public Pessoa(string nome, Sexo? sexo, string email, DateTime dataNascimento, string naturalidade, string nacionalidade, string cpf, string telefone = null, Endereco endereco = null)
        {
            Id = Guid.NewGuid();
            Nome = nome;
            Sexo = sexo;
            Email = email;
            DataNascimento = dataNascimento;
            Naturalidade = naturalidade;
            Nacionalidade = nacionalidade;
            CPF = cpf;
            Telefone = telefone;
            Endereco = endereco;
            DataCadastro = DateTime.UtcNow;
            
            endereco?.DefinirPessoaId(Id);
        }

        protected Pessoa() { }

        public void Atualizar(string nome, Sexo? sexo, string email, DateTime dataNascimento,string naturalidade, string nacionalidade, string telefone = null)
        {
            Nome = nome;
            Sexo = sexo;
            Email = email;
            DataNascimento = dataNascimento;
            Naturalidade = naturalidade;
            Nacionalidade = nacionalidade;
            Telefone = telefone;
            DataAtualizacao = DateTime.UtcNow;
        }

        public void AtualizarEndereco(Endereco novoEndereco)
        {
            if (novoEndereco != null)
            {
                if (Endereco != null)
                {
                    Endereco.Atualizar(
                        novoEndereco.CEP,
                        novoEndereco.Logradouro,
                        novoEndereco.Numero,
                        novoEndereco.Complemento,
                        novoEndereco.Bairro,
                        novoEndereco.Cidade,
                        novoEndereco.Estado
                    );
                }
                else
                {
                    Endereco = novoEndereco;
                    novoEndereco.DefinirPessoaId(Id);
                }
                
                DataAtualizacao = DateTime.UtcNow;
            }
        }

        public string EnderecoCompleto => Endereco?.ToString();
    }
}
