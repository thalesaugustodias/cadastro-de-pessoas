using CadastroDePessoas.Domain.Entidades;
using CadastroDePessoas.Domain.Enums;
using Xunit;

namespace CadastroDePessoas.Domain.Testes.Entidades
{
    public class PessoaTests
    {
        [Fact]
        public void Construtor_ComTodosOsParametros_DeveCriarPessoaCorretamente()
        {
            // Arrange
            var nome = "João Silva";
            var sexo = Sexo.Masculino;
            var email = "joao@exemplo.com";
            var dataNascimento = new DateTime(1990, 1, 1);
            var naturalidade = "São Paulo";
            var nacionalidade = "Brasileira";
            var cpf = "52998224725";
            var telefone = "(11) 99999-9999";
            var endereco = new Endereco(
                "01310-100",
                "Av. Paulista",
                "1000",
                "Apto 101",
                "Bela Vista",
                "São Paulo",
                "SP"
            );

            // Act
            var pessoa = new Pessoa(
                nome,
                sexo,
                email,
                dataNascimento,
                naturalidade,
                nacionalidade,
                cpf,
                telefone,
                endereco
            );

            // Assert
            Assert.Equal(nome, pessoa.Nome);
            Assert.Equal(sexo, pessoa.Sexo);
            Assert.Equal(email, pessoa.Email);
            Assert.Equal(dataNascimento, pessoa.DataNascimento);
            Assert.Equal(naturalidade, pessoa.Naturalidade);
            Assert.Equal(nacionalidade, pessoa.Nacionalidade);
            Assert.Equal(cpf, pessoa.CPF);
            Assert.Equal(telefone, pessoa.Telefone);
            Assert.Same(endereco, pessoa.Endereco);
            Assert.NotEqual(Guid.Empty, pessoa.Id);
            Assert.True(DateTime.UtcNow.AddMinutes(-1) <= pessoa.DataCadastro && pessoa.DataCadastro <= DateTime.UtcNow);
            Assert.Null(pessoa.DataAtualizacao);
        }

        [Fact]
        public void Construtor_SemEndereco_DeveCriarPessoaComEnderecoNulo()
        {
            // Arrange
            var nome = "João Silva";
            var sexo = Sexo.Masculino;
            var email = "joao@exemplo.com";
            var dataNascimento = new DateTime(1990, 1, 1);
            var naturalidade = "São Paulo";
            var nacionalidade = "Brasileira";
            var cpf = "52998224725";
            var telefone = "(11) 99999-9999";

            // Act
            var pessoa = new Pessoa(
                nome,
                sexo,
                email,
                dataNascimento,
                naturalidade,
                nacionalidade,
                cpf,
                telefone,
                null
            );

            // Assert
            Assert.Equal(nome, pessoa.Nome);
            Assert.Equal(cpf, pessoa.CPF);
            Assert.Equal(telefone, pessoa.Telefone);
            Assert.Null(pessoa.Endereco);
        }

        [Fact]
        public void Atualizar_ComTodosOsParametros_DeveAtualizarPropriedades()
        {
            // Arrange
            var pessoa = new Pessoa(
                "João Silva",
                Sexo.Masculino,
                "joao@exemplo.com",
                new DateTime(1990, 1, 1),
                "São Paulo",
                "Brasileira",
                "52998224725"
            );

            var novoNome = "João Silva Atualizado";
            var novoSexo = Sexo.Outro;
            var novoEmail = "joao.atualizado@exemplo.com";
            var novaDataNascimento = new DateTime(1990, 1, 2);
            var novaNaturalidade = "Rio de Janeiro";
            var novaNacionalidade = "Brasileira Atualizada";
            var novoTelefone = "(11) 98888-8888";

            // Act
            pessoa.Atualizar(
                novoNome,
                novoSexo,
                novoEmail,
                novaDataNascimento,
                novaNaturalidade,
                novaNacionalidade,
                novoTelefone
            );

            // Assert
            Assert.Equal(novoNome, pessoa.Nome);
            Assert.Equal(novoSexo, pessoa.Sexo);
            Assert.Equal(novoEmail, pessoa.Email);
            Assert.Equal(novaDataNascimento, pessoa.DataNascimento);
            Assert.Equal(novaNaturalidade, pessoa.Naturalidade);
            Assert.Equal(novaNacionalidade, pessoa.Nacionalidade);
            Assert.Equal(novoTelefone, pessoa.Telefone);
            Assert.NotNull(pessoa.DataAtualizacao);
            Assert.True(DateTime.UtcNow.AddMinutes(-1) <= pessoa.DataAtualizacao && pessoa.DataAtualizacao <= DateTime.UtcNow);
        }

        [Fact]
        public void AtualizarEndereco_ComEnderecoValido_DeveAtualizarEndereco()
        {
            // Arrange
            var endereco = new Endereco(
                "01310-100",
                "Av. Paulista",
                "1000",
                "Apto 101",
                "Bela Vista",
                "São Paulo",
                "SP"
            );

            var pessoa = new Pessoa(
                "João Silva",
                Sexo.Masculino,
                "joao@exemplo.com",
                new DateTime(1990, 1, 1),
                "São Paulo",
                "Brasileira",
                "52998224725",
                "(11) 99999-9999",
                endereco
            );

            var novoEndereco = new Endereco(
                "22050-002",
                "Av. Copacabana",
                "500",
                "Apto 202",
                "Copacabana",
                "Rio de Janeiro",
                "RJ"
            );

            // Act
            pessoa.AtualizarEndereco(novoEndereco);

            // Assert
            Assert.Equal(novoEndereco.CEP, pessoa.Endereco.CEP);
            Assert.Equal(novoEndereco.Logradouro, pessoa.Endereco.Logradouro);
            Assert.Equal(novoEndereco.Numero, pessoa.Endereco.Numero);
            Assert.Equal(novoEndereco.Complemento, pessoa.Endereco.Complemento);
            Assert.Equal(novoEndereco.Bairro, pessoa.Endereco.Bairro);
            Assert.Equal(novoEndereco.Cidade, pessoa.Endereco.Cidade);
            Assert.Equal(novoEndereco.Estado, pessoa.Endereco.Estado);
            Assert.Equal(novoEndereco.ToString(), pessoa.Endereco.ToString());
            Assert.NotNull(pessoa.DataAtualizacao);
            Assert.True(DateTime.UtcNow.AddMinutes(-1) <= pessoa.DataAtualizacao && pessoa.DataAtualizacao <= DateTime.UtcNow);
        }

        [Fact]
        public void AtualizarEndereco_ComEnderecoNulo_NaoDeveAtualizarEndereco()
        {
            // Arrange
            var endereco = new Endereco(
                "01310-100",
                "Av. Paulista",
                "1000",
                "Apto 101",
                "Bela Vista",
                "São Paulo",
                "SP"
            );

            var pessoa = new Pessoa(
                "João Silva",
                Sexo.Masculino,
                "joao@exemplo.com",
                new DateTime(1990, 1, 1),
                "São Paulo",
                "Brasileira",
                "52998224725",
                "(11) 99999-9999",
                endereco
            );

            var dataAtualizacaoAntes = pessoa.DataAtualizacao;

            // Act
            pessoa.AtualizarEndereco(null);

            // Assert
            Assert.Same(endereco, pessoa.Endereco);
            Assert.Equal(dataAtualizacaoAntes, pessoa.DataAtualizacao);
        }

        [Fact]
        public void EnderecoCompleto_ComEndereco_DeveRetornarEnderecoFormatado()
        {
            // Arrange
            var endereco = new Endereco(
                "01310-100",
                "Av. Paulista",
                "1000",
                "Apto 101",
                "Bela Vista",
                "São Paulo",
                "SP"
            );

            var pessoa = new Pessoa(
                "João Silva",
                Sexo.Masculino,
                "joao@exemplo.com",
                new DateTime(1990, 1, 1),
                "São Paulo",
                "Brasileira",
                "52998224725",
                "(11) 99999-9999",
                endereco
            );

            // Act
            var enderecoCompleto = pessoa.EnderecoCompleto;

            // Assert
            Assert.NotNull(enderecoCompleto);
            Assert.Contains("Av. Paulista", enderecoCompleto);
            Assert.Contains("1000", enderecoCompleto);
            Assert.Contains("Bela Vista", enderecoCompleto);
            Assert.Contains("São Paulo", enderecoCompleto);
            Assert.Contains("SP", enderecoCompleto);
            Assert.Contains("01310-100", enderecoCompleto);
        }

        [Fact]
        public void EnderecoCompleto_SemEndereco_DeveRetornarNull()
        {
            // Arrange
            var pessoa = new Pessoa(
                "João Silva",
                Sexo.Masculino,
                "joao@exemplo.com",
                new DateTime(1990, 1, 1),
                "São Paulo",
                "Brasileira",
                "52998224725",
                "(11) 99999-9999",
                null
            );

            // Act
            var enderecoCompleto = pessoa.EnderecoCompleto;

            // Assert
            Assert.Null(enderecoCompleto);
        }
    }
}