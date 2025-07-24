using CadastroDePessoas.Application.DTOs.Pessoa;
using CadastroDePessoas.Domain.Entidades;

namespace CadastroDePessoas.Application.Factories
{
    public static class PessoaFactory
    {
        public static Pessoa CriarEntidade(PessoaCriacaoDTO dto)
        {
            return new Pessoa(
                dto.Nome,
                dto.Sexo,
                dto.Email,
                dto.DataNascimento,
                dto.Naturalidade,
                dto.Nacionalidade,
                dto.CPF,
                dto.Endereco
            );
        }

        public static PessoaDTO CriarDTO(Pessoa entidade)
        {
            return new PessoaDTO
            {
                Id = entidade.Id,
                Nome = entidade.Nome,
                Sexo = entidade.Sexo,
                Email = entidade.Email,
                DataNascimento = entidade.DataNascimento,
                Naturalidade = entidade.Naturalidade,
                Nacionalidade = entidade.Nacionalidade,
                CPF = entidade.CPF,
                Endereco = entidade.Endereco,
                DataCadastro = entidade.DataCadastro,
                DataAtualizacao = entidade.DataAtualizacao
            };
        }
    }
}
