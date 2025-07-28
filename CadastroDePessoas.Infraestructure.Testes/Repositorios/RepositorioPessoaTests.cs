using CadastroDePessoas.Domain.Enums;
using CadastroDePessoas.Infraestructure.Contexto;
using CadastroDePessoas.Infraestructure.Repositorios;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace CadastroDePessoas.Infraestructure.Testes.Repositorios
{
    public class RepositorioPessoaTests
    {
        private static DbContextOptions<AppDbContexto> ObterOpcoesBancoDados()
        {
            return new DbContextOptionsBuilder<AppDbContexto>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
        }

        [Fact]
        public async Task AdicionarAsync_DevePersistirPessoa()
        {
            // Arrange
            var options = ObterOpcoesBancoDados();
            var pessoa = new Domain.Entidades.Pessoa(
                "João Silva",
                Sexo.Masculino,
                "joao@exemplo.com",
                new DateTime(1990, 1, 1),
                "São Paulo",
                "Brasileira",
                "52998224725",
                "(11) 98765-4321" // Adicionando o telefone
            );

            // Act
            using (var contexto = new AppDbContexto(options))
            {
                var repositorio = new RepositorioPessoa(contexto);
                await repositorio.AdicionarAsync(pessoa);
            }

            // Assert
            using (var contexto = new AppDbContexto(options))
            {
                Assert.Equal(1, await contexto.Pessoas.CountAsync());
                var pessoaPersistida = await contexto.Pessoas.FirstOrDefaultAsync();
                Assert.NotNull(pessoaPersistida);
                Assert.Equal("João Silva", pessoaPersistida.Nome);
                Assert.Equal(Sexo.Masculino, pessoaPersistida.Sexo);
                Assert.Equal("joao@exemplo.com", pessoaPersistida.Email);
                Assert.Equal(new DateTime(1990, 1, 1), pessoaPersistida.DataNascimento);
                Assert.Equal("São Paulo", pessoaPersistida.Naturalidade);
                Assert.Equal("Brasileira", pessoaPersistida.Nacionalidade);
                Assert.Equal("52998224725", pessoaPersistida.CPF);
                Assert.Equal("(11) 98765-4321", pessoaPersistida.Telefone);
            }
        }

        [Fact]
        public async Task CpfExisteAsync_ComCPFExistente_DeveRetornarTrue()
        {
            // Arrange
            var options = ObterOpcoesBancoDados();
            var pessoa = new Domain.Entidades.Pessoa(
                "João Silva",
                Sexo.Masculino,
                "joao@exemplo.com",
                new DateTime(1990, 1, 1),
                "São Paulo",
                "Brasileira",
                "52998224725",
                "(11) 98765-4321" // Adicionando o telefone
            );

            using (var contexto = new AppDbContexto(options))
            {
                await contexto.Pessoas.AddAsync(pessoa);
                await contexto.SaveChangesAsync();
            }

            // Act
            using (var contexto = new AppDbContexto(options))
            {
                var repositorio = new RepositorioPessoa(contexto);
                var resultado = await repositorio.CpfExisteAsync("52998224725");

                // Assert
                Assert.True(resultado);
            }
        }

        [Fact]
        public async Task CpfExisteAsync_ComCPFInexistente_DeveRetornarFalse()
        {
            // Arrange
            var options = ObterOpcoesBancoDados();
            var pessoa = new Domain.Entidades.Pessoa(
                "João Silva",
                Sexo.Masculino,
                "joao@exemplo.com",
                new DateTime(1990, 1, 1),
                "São Paulo",
                "Brasileira",
                "52998224725",
                "(11) 98765-4321" // Adicionando o telefone
            );

            using (var contexto = new AppDbContexto(options))
            {
                await contexto.Pessoas.AddAsync(pessoa);
                await contexto.SaveChangesAsync();
            }

            // Act
            using (var contexto = new AppDbContexto(options))
            {
                var repositorio = new RepositorioPessoa(contexto);
                var resultado = await repositorio.CpfExisteAsync("11144477735");

                // Assert
                Assert.False(resultado);
            }
        }

        [Fact]
        public async Task CpfExisteAsync_ComCPFExistenteEIgnorandoId_DeveRetornarFalse()
        {
            // Arrange
            var options = ObterOpcoesBancoDados();
            var pessoa = new Domain.Entidades.Pessoa(
                "João Silva",
                Sexo.Masculino,
                "joao@exemplo.com",
                new DateTime(1990, 1, 1),
                "São Paulo",
                "Brasileira",
                "52998224725",
                "(11) 98765-4321" // Adicionando o telefone
            );

            using (var contexto = new AppDbContexto(options))
            {
                await contexto.Pessoas.AddAsync(pessoa);
                await contexto.SaveChangesAsync();
            }

            // Act
            using (var contexto = new AppDbContexto(options))
            {
                var repositorio = new RepositorioPessoa(contexto);
                var pessoaExistente = await contexto.Pessoas.FirstOrDefaultAsync();
                var resultado = await repositorio.CpfExisteAsync("52998224725", pessoaExistente.Id);

                // Assert
                Assert.False(resultado);
            }
        }

        [Fact]
        public async Task ObterPorIdAsync_ComIdExistente_DeveRetornarPessoa()
        {
            // Arrange
            var options = ObterOpcoesBancoDados();
            var pessoa = new Domain.Entidades.Pessoa(
                "João Silva",
                Sexo.Masculino,
                "joao@exemplo.com",
                new DateTime(1990, 1, 1),
                "São Paulo",
                "Brasileira",
                "52998224725",
                "(11) 98765-4321" // Adicionando o telefone
            );

            using (var contexto = new AppDbContexto(options))
            {
                await contexto.Pessoas.AddAsync(pessoa);
                await contexto.SaveChangesAsync();
            }

            // Act
            using (var contexto = new AppDbContexto(options))
            {
                var repositorio = new RepositorioPessoa(contexto);
                var pessoaExistente = await contexto.Pessoas.FirstOrDefaultAsync();
                var resultado = await repositorio.ObterPorIdAsync(pessoaExistente.Id);

                // Assert
                Assert.NotNull(resultado);
                Assert.Equal(pessoaExistente.Id, resultado.Id);
                Assert.Equal("João Silva", resultado.Nome);
                Assert.Equal("(11) 98765-4321", resultado.Telefone);
            }
        }

        [Fact]
        public async Task ObterPorIdAsync_ComIdInexistente_DeveRetornarNull()
        {
            // Arrange
            var options = ObterOpcoesBancoDados();

            // Act
            using (var contexto = new AppDbContexto(options))
            {
                var repositorio = new RepositorioPessoa(contexto);
                var resultado = await repositorio.ObterPorIdAsync(Guid.NewGuid());

                // Assert
                Assert.Null(resultado);
            }
        }

        [Fact]
        public async Task ObterTodosAsync_DeveRetornarTodasPessoas()
        {
            // Arrange
            var options = ObterOpcoesBancoDados();
            var pessoa1 = new Domain.Entidades.Pessoa(
                "João Silva",
                Sexo.Masculino,
                "joao@exemplo.com",
                new DateTime(1990, 1, 1),
                "São Paulo",
                "Brasileira",
                "52998224725",
                "(11) 98765-4321" // Adicionando o telefone
            );

            var pessoa2 = new Domain.Entidades.Pessoa(
                "Maria Santos",
                Sexo.Feminino,
                "maria@exemplo.com",
                new DateTime(1992, 5, 10),
                "Rio de Janeiro",
                "Brasileira",
                "11144477735",
                "(21) 98765-4321" // Adicionando o telefone
            );

            using (var contexto = new AppDbContexto(options))
            {
                await contexto.Pessoas.AddRangeAsync(pessoa1, pessoa2);
                await contexto.SaveChangesAsync();
            }

            // Act
            using (var contexto = new AppDbContexto(options))
            {
                var repositorio = new RepositorioPessoa(contexto);
                var resultado = await repositorio.ObterTodosAsync();

                // Assert
                Assert.NotNull(resultado);
                Assert.Equal(2, resultado.Count());
            }
        }

        [Fact]
        public async Task AtualizarAsync_DeveAtualizarPessoa()
        {
            // Arrange
            var options = ObterOpcoesBancoDados();
            var pessoa = new Domain.Entidades.Pessoa(
                "João Silva",
                Sexo.Masculino,
                "joao@exemplo.com",
                new DateTime(1990, 1, 1),
                "São Paulo",
                "Brasileira",
                "52998224725",
                "(11) 98765-4321" // Adicionando o telefone
            );

            using (var contexto = new AppDbContexto(options))
            {
                await contexto.Pessoas.AddAsync(pessoa);
                await contexto.SaveChangesAsync();
            }

            // Act
            using (var contexto = new AppDbContexto(options))
            {
                var repositorio = new RepositorioPessoa(contexto);
                var pessoaExistente = await contexto.Pessoas.FirstOrDefaultAsync();

                pessoaExistente.Atualizar(
                    "João Silva Atualizado",
                    Sexo.Masculino,
                    "joao.atualizado@exemplo.com",
                    new DateTime(1990, 1, 1),
                    "Rio de Janeiro",
                    "Brasileira",
                    "(11) 99999-8888" // Atualizando o telefone
                );

                await repositorio.AtualizarAsync(pessoaExistente);
            }

            // Assert
            using (var contexto = new AppDbContexto(options))
            {
                var pessoaAtualizada = await contexto.Pessoas.FirstOrDefaultAsync();
                Assert.NotNull(pessoaAtualizada);
                Assert.Equal("João Silva Atualizado", pessoaAtualizada.Nome);
                Assert.Equal("joao.atualizado@exemplo.com", pessoaAtualizada.Email);
                Assert.Equal("Rio de Janeiro", pessoaAtualizada.Naturalidade);
                Assert.Equal("(11) 99999-8888", pessoaAtualizada.Telefone);
                Assert.NotNull(pessoaAtualizada.DataAtualizacao);
            }
        }

        [Fact]
        public async Task RemoverAsync_DeveRemoverPessoa()
        {
            // Arrange
            var options = ObterOpcoesBancoDados();
            var pessoa = new Domain.Entidades.Pessoa(
                "João Silva",
                Sexo.Masculino,
                "joao@exemplo.com",
                new DateTime(1990, 1, 1),
                "São Paulo",
                "Brasileira",
                "52998224725",
                "(11) 98765-4321" // Adicionando o telefone
            );

            using (var contexto = new AppDbContexto(options))
            {
                await contexto.Pessoas.AddAsync(pessoa);
                await contexto.SaveChangesAsync();
            }

            // Act
            using (var contexto = new AppDbContexto(options))
            {
                var repositorio = new RepositorioPessoa(contexto);
                var pessoaExistente = await contexto.Pessoas.FirstOrDefaultAsync();
                await repositorio.RemoverAsync(pessoaExistente);
            }

            // Assert
            using (var contexto = new AppDbContexto(options))
            {
                Assert.Equal(0, await contexto.Pessoas.CountAsync());
            }
        }
    }
}
