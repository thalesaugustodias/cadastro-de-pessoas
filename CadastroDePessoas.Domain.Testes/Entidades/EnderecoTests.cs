using CadastroDePessoas.Domain.Entidades;
using Xunit;

namespace CadastroDePessoas.Domain.Testes.Entidades
{
    public class EnderecoTests
    {
        [Fact]
        public void Construtor_ComTodosOsParametros_DeveCriarEnderecoCorretamente()
        {
            // Arrange
            var cep = "01310-100";
            var logradouro = "Av. Paulista";
            var numero = "1000";
            var complemento = "Apto 101";
            var bairro = "Bela Vista";
            var cidade = "São Paulo";
            var estado = "SP";

            // Act
            var endereco = new Endereco(
                cep,
                logradouro,
                numero,
                complemento,
                bairro,
                cidade,
                estado
            );

            // Assert
            Assert.Equal(cep, endereco.CEP);
            Assert.Equal(logradouro, endereco.Logradouro);
            Assert.Equal(numero, endereco.Numero);
            Assert.Equal(complemento, endereco.Complemento);
            Assert.Equal(bairro, endereco.Bairro);
            Assert.Equal(cidade, endereco.Cidade);
            Assert.Equal(estado, endereco.Estado);
            Assert.NotEqual(Guid.Empty, endereco.Id);
        }

        [Fact]
        public void Atualizar_ComTodosOsParametros_DeveAtualizarPropriedades()
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

            var novoCep = "22050-002";
            var novoLogradouro = "Av. Copacabana";
            var novoNumero = "500";
            var novoComplemento = "Apto 202";
            var novoBairro = "Copacabana";
            var novaCidade = "Rio de Janeiro";
            var novoEstado = "RJ";

            // Act
            endereco.Atualizar(
                novoCep,
                novoLogradouro,
                novoNumero,
                novoComplemento,
                novoBairro,
                novaCidade,
                novoEstado
            );

            // Assert
            Assert.Equal(novoCep, endereco.CEP);
            Assert.Equal(novoLogradouro, endereco.Logradouro);
            Assert.Equal(novoNumero, endereco.Numero);
            Assert.Equal(novoComplemento, endereco.Complemento);
            Assert.Equal(novoBairro, endereco.Bairro);
            Assert.Equal(novaCidade, endereco.Cidade);
            Assert.Equal(novoEstado, endereco.Estado);
        }

        [Fact]
        public void ToString_DeveRetornarEnderecoFormatado()
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

            // Act
            var resultado = endereco.ToString();

            // Assert
            Assert.NotNull(resultado);
            Assert.Contains("Av. Paulista", resultado);
            Assert.Contains("1000", resultado);
            Assert.Contains("Apto 101", resultado);
            Assert.Contains("Bela Vista", resultado);
            Assert.Contains("São Paulo", resultado);
            Assert.Contains("SP", resultado);
            Assert.Contains("01310-100", resultado);
        }

        [Fact]
        public void ToString_SemComplemento_DeveRetornarEnderecoFormatadoSemComplemento()
        {
            // Arrange
            var endereco = new Endereco(
                "01310-100",
                "Av. Paulista",
                "1000",
                null,
                "Bela Vista",
                "São Paulo",
                "SP"
            );

            // Act
            var resultado = endereco.ToString();

            // Assert
            Assert.NotNull(resultado);
            Assert.Contains("Av. Paulista", resultado);
            Assert.Contains("1000", resultado);
            Assert.Contains("Bela Vista", resultado);
            Assert.Contains("São Paulo", resultado);
            Assert.Contains("SP", resultado);
            Assert.Contains("01310-100", resultado);
            Assert.DoesNotContain("null", resultado);
        }

        [Fact]
        public void ToString_ComComplementoVazio_DeveRetornarEnderecoFormatadoSemComplemento()
        {
            // Arrange
            var endereco = new Endereco(
                "01310-100",
                "Av. Paulista",
                "1000",
                "",
                "Bela Vista",
                "São Paulo",
                "SP"
            );

            // Act
            var resultado = endereco.ToString();

            // Assert
            Assert.NotNull(resultado);
            Assert.Contains("Av. Paulista", resultado);
            Assert.Contains("1000", resultado);
            Assert.DoesNotContain(", ,", resultado);
        }
    }
}