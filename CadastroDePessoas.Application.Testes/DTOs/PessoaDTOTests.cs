using CadastroDePessoas.Application.DTOs.Endereco;
using CadastroDePessoas.Application.DTOs.Pessoa;
using CadastroDePessoas.Domain.Enums;
using System;
using Xunit;

namespace CadastroDePessoas.Application.Testes.DTOs
{
    public class PessoaDTOTests
    {
        [Fact]
        public void PessoaDTO_PropriedadesDevemSerCorretamenteSetadas()
        {
            // Arrange
            var id = Guid.NewGuid();
            var nome = "João Silva";
            var email = "joao.silva@exemplo.com";
            var cpf = "12345678900";
            var rg = "1234567";
            var dataNascimento = new DateTime(1990, 1, 1);
            var sexo = Sexo.Masculino;
            var naturalidade = "São Paulo";
            var nacionalidade = "Brasileira";
            var created = DateTime.Now;
            var updated = DateTime.Now.AddDays(1);

            // Act
            var pessoaDTO = new PessoaDTO
            {
                Id = id,
                Nome = nome,
                Email = email,
                CPF = cpf,
                DataNascimento = dataNascimento,
                Sexo = sexo,
                Naturalidade = naturalidade,
                Nacionalidade = nacionalidade,
                DataCadastro = created,
                DataAtualizacao = updated
            };

            // Assert
            Assert.Equal(id, pessoaDTO.Id);
            Assert.Equal(nome, pessoaDTO.Nome);
            Assert.Equal(email, pessoaDTO.Email);
            Assert.Equal(cpf, pessoaDTO.CPF);
            Assert.Equal(dataNascimento, pessoaDTO.DataNascimento);
            Assert.Equal(sexo, pessoaDTO.Sexo);
            Assert.Equal(naturalidade, pessoaDTO.Naturalidade);
            Assert.Equal(nacionalidade, pessoaDTO.Nacionalidade);
            Assert.Equal(created, pessoaDTO.DataCadastro);
            Assert.Equal(updated, pessoaDTO.DataAtualizacao);
        }

        [Fact]
        public void PessoaDTO_DevePermitirPropriedadesNulas()
        {
            // Arrange & Act
            var pessoaDTO = new PessoaDTO
            {
                Id = Guid.NewGuid(),
                Nome = "João Silva",
                DataNascimento = new DateTime(1990, 1, 1),
                Sexo = Sexo.Masculino
            };

            // Assert
            Assert.Null(pessoaDTO.Email);
            Assert.Null(pessoaDTO.CPF);
            Assert.Null(pessoaDTO.Naturalidade);
            Assert.Null(pessoaDTO.Nacionalidade);
        }
        
        [Fact]
        public void PessoaDTO_ConstrutorPadrao_DeveCriarInstanciaValida()
        {
            // Act
            var pessoaDTO = new PessoaDTO();
            
            // Assert
            Assert.NotEqual(Guid.Empty, pessoaDTO.Id);
            Assert.Null(pessoaDTO.Nome);
            Assert.Null(pessoaDTO.Email);
            Assert.Null(pessoaDTO.CPF);
            Assert.Equal(default, pessoaDTO.DataNascimento);
            Assert.Equal(default, pessoaDTO.Sexo);
        }
        
        [Fact]
        public void PessoaDTO_EnderecoDTO_DeveSerCorretamenteSetado()
        {
            // Arrange
            var endereco = new EnderecoDTO
            {
                Logradouro = "Rua Teste",
                Numero = "123",
                Complemento = "Apto 101",
                Bairro = "Centro",
                Cidade = "São Paulo",
                Estado = "SP",
                CEP = "01234-567"
            };
            
            // Act
            var pessoaDTO = new PessoaDTO
            {
                Nome = "João Silva",
                Endereco = endereco
            };
            
            // Assert
            Assert.NotNull(pessoaDTO.Endereco);
            Assert.Equal("Rua Teste", pessoaDTO.Endereco.Logradouro);
            Assert.Equal("123", pessoaDTO.Endereco.Numero);
            Assert.Equal("Apto 101", pessoaDTO.Endereco.Complemento);
            Assert.Equal("Centro", pessoaDTO.Endereco.Bairro);
            Assert.Equal("São Paulo", pessoaDTO.Endereco.Cidade);
            Assert.Equal("SP", pessoaDTO.Endereco.Estado);
            Assert.Equal("01234-567", pessoaDTO.Endereco.CEP);
        }
        
        [Fact]
        public void PessoaDTO_IdadeCalculada_DeveRetornarIdadeCorreta()
        {
            // Arrange
            var dataNascimento = DateTime.Now.AddYears(-30);
            
            // Act
            var pessoaDTO = new PessoaDTO
            {
                Nome = "João Silva",
                DataNascimento = dataNascimento
            };
            
            // Assert
            Assert.Equal(30, pessoaDTO.Idade);
        }
        
        [Fact]
        public void PessoaDTO_DataNascimentoFutura_IdadeDeveSerZero()
        {
            // Arrange
            var dataNascimentoFutura = DateTime.Now.AddYears(1);
            
            // Act
            var pessoaDTO = new PessoaDTO
            {
                Nome = "João Silva",
                DataNascimento = dataNascimentoFutura
            };
            
            // Assert
            Assert.Equal(0, pessoaDTO.Idade);
        }
    }
}