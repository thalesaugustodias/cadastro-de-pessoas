using CadastroDePessoas.Application.DTOs.Endereco;
using Xunit;

namespace CadastroDePessoas.Application.Testes.DTOs
{
    public class EnderecoDTOTests
    {
        [Fact]
        public void EnderecoDTO_PropriedadesDevemSerCorretamenteSetadas()
        {
            // Arrange
            var logradouro = "Avenida Paulista";
            var numero = "1000";
            var complemento = "Sala 123";
            var bairro = "Bela Vista";
            var cidade = "São Paulo";
            var estado = "SP";
            var cep = "01310-100";

            // Act
            var enderecoDTO = new EnderecoDTO
            {
                Logradouro = logradouro,
                Numero = numero,
                Complemento = complemento,
                Bairro = bairro,
                Cidade = cidade,
                Estado = estado,
                CEP = cep
            };

            // Assert
            Assert.Equal(logradouro, enderecoDTO.Logradouro);
            Assert.Equal(numero, enderecoDTO.Numero);
            Assert.Equal(complemento, enderecoDTO.Complemento);
            Assert.Equal(bairro, enderecoDTO.Bairro);
            Assert.Equal(cidade, enderecoDTO.Cidade);
            Assert.Equal(estado, enderecoDTO.Estado);
            Assert.Equal(cep, enderecoDTO.CEP);
        }
        
        [Fact]
        public void EnderecoDTO_ConstrutorPadrao_DeveCriarInstanciaValida()
        {
            // Act
            var enderecoDTO = new EnderecoDTO();
            
            // Assert
            Assert.Null(enderecoDTO.Logradouro);
            Assert.Null(enderecoDTO.Numero);
            Assert.Null(enderecoDTO.Complemento);
            Assert.Null(enderecoDTO.Bairro);
            Assert.Null(enderecoDTO.Cidade);
            Assert.Null(enderecoDTO.Estado);
            Assert.Null(enderecoDTO.CEP);
        }
        
        [Fact]
        public void EnderecoDTO_ToString_DeveRetornarEnderecoFormatado()
        {
            // Arrange
            var enderecoDTO = new EnderecoDTO
            {
                Logradouro = "Avenida Paulista",
                Numero = "1000",
                Complemento = "Sala 123",
                Bairro = "Bela Vista",
                Cidade = "São Paulo",
                Estado = "SP",
                CEP = "01310-100"
            };
            
            // Act
            var resultado = enderecoDTO.ToString();
            
            // Assert
            Assert.Contains("Avenida Paulista", resultado);
            Assert.Contains("1000", resultado);
            Assert.Contains("Sala 123", resultado);
            Assert.Contains("Bela Vista", resultado);
            Assert.Contains("São Paulo", resultado);
            Assert.Contains("SP", resultado);
            Assert.Contains("01310-100", resultado);
        }
        
        [Fact]
        public void EnderecoDTO_ToString_ComCamposNulos_DeveRetornarValoresDisponiveis()
        {
            // Arrange
            var enderecoDTO = new EnderecoDTO
            {
                Logradouro = "Avenida Paulista",
                Numero = "1000",
                // Complemento omitido propositalmente
                Bairro = "Bela Vista",
                Cidade = "São Paulo",
                // Estado omitido propositalmente
                CEP = "01310-100"
            };
            
            // Act
            var resultado = enderecoDTO.ToString();
            
            // Assert
            Assert.Contains("Avenida Paulista", resultado);
            Assert.Contains("1000", resultado);
            Assert.Contains("Bela Vista", resultado);
            Assert.Contains("São Paulo", resultado);
            Assert.Contains("01310-100", resultado);
            Assert.DoesNotContain("null", resultado);
        }
        
        [Fact]
        public void EnderecoDTO_ToString_ComTodosCamposNulos_DeveRetornarStringVazia()
        {
            // Arrange
            var enderecoDTO = new EnderecoDTO();
            
            // Act
            var resultado = enderecoDTO.ToString();
            
            // Assert
            Assert.Equal("", resultado);
        }
        
        [Fact]
        public void EnderecoDTO_EnderecoCompleto_DeveRetornarTrue_QuandoTodosOsCamposPrincipaisPreenchidos()
        {
            // Arrange
            var enderecoDTO = new EnderecoDTO
            {
                Logradouro = "Avenida Paulista",
                Numero = "1000",
                Bairro = "Bela Vista",
                Cidade = "São Paulo",
                Estado = "SP",
                CEP = "01310-100"
            };
            
            // Act & Assert
            Assert.True(enderecoDTO.EnderecoCompleto);
        }
        
        [Fact]
        public void EnderecoDTO_EnderecoCompleto_DeveRetornarFalse_QuandoAlgumCampoPrincipalFaltando()
        {
            // Arrange - sem logradouro
            var enderecoDTOSemLogradouro = new EnderecoDTO
            {
                Numero = "1000",
                Bairro = "Bela Vista",
                Cidade = "São Paulo",
                Estado = "SP",
                CEP = "01310-100"
            };
            
            // Arrange - sem cidade
            var enderecoDTOSemCidade = new EnderecoDTO
            {
                Logradouro = "Avenida Paulista",
                Numero = "1000",
                Bairro = "Bela Vista",
                Estado = "SP",
                CEP = "01310-100"
            };
            
            // Act & Assert
            Assert.False(enderecoDTOSemLogradouro.EnderecoCompleto);
            Assert.False(enderecoDTOSemCidade.EnderecoCompleto);
        }
        
        [Fact]
        public void EnderecoDTO_EnderecoCompleto_DeveRetornarTrue_MesmoSemComplemento()
        {
            // Arrange
            var enderecoDTO = new EnderecoDTO
            {
                Logradouro = "Avenida Paulista",
                Numero = "1000",
                // Complemento omitido propositalmente
                Bairro = "Bela Vista",
                Cidade = "São Paulo",
                Estado = "SP",
                CEP = "01310-100"
            };
            
            // Act & Assert
            Assert.True(enderecoDTO.EnderecoCompleto);
        }
    }
}