using CadastroDePessoas.Domain.Validacoes;
using Xunit;

namespace CadastroDePessoas.Domain.Testes.Validacoes
{
    public class ValidadorCPFTests
    {
        [Theory]
        [InlineData("529.982.247-25")]
        [InlineData("52998224725")]
        [InlineData("111.444.777-35")]
        public void Validar_CPFValido_RetornaTrue(string cpf)
        {
            // Act
            var resultado = ValidadorCPF.Validar(cpf);

            // Assert
            Assert.True(resultado);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("12345")]
        [InlineData("123.456.789-10")]
        [InlineData("111.111.111-11")]
        [InlineData("222.222.222-22")]
        public void Validar_CPFInvalido_RetornaFalse(string cpf)
        {
            // Act
            var resultado = ValidadorCPF.Validar(cpf);

            // Assert
            Assert.False(resultado);
        }

        [Theory]
        [InlineData("52998224725", "529.982.247-25")]
        [InlineData("11144477735", "111.444.777-35")]
        public void Formatar_CPFSemFormatacao_RetornaCPFFormatado(string cpfSemFormatacao, string cpfFormatado)
        {
            // Act
            var resultado = ValidadorCPF.Formatar(cpfSemFormatacao);

            // Assert
            Assert.Equal(cpfFormatado, resultado);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("12345")]
        public void Formatar_CPFInvalido_RetornaMesmoValor(string cpfInvalido)
        {
            // Act
            var resultado = ValidadorCPF.Formatar(cpfInvalido);

            // Assert
            Assert.Equal(cpfInvalido, resultado);
        }
    }
}
