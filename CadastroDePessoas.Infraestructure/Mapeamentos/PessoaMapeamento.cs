using CadastroDePessoas.Domain.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CadastroDePessoas.Infraestructure.Mapeamentos
{
    public class PessoaMapeamento : IEntityTypeConfiguration<Pessoa>
    {
        public void Configure(EntityTypeBuilder<Pessoa> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Nome)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(p => p.Email)
                .HasMaxLength(100);

            builder.Property(p => p.DataNascimento)
                .IsRequired();

            builder.Property(p => p.Naturalidade)
                .HasMaxLength(100);

            builder.Property(p => p.Nacionalidade)
                .HasMaxLength(100);

            builder.Property(p => p.CPF)
                .IsRequired()
                .HasMaxLength(14);

            builder.Property(p => p.Telefone)
                .IsRequired(false)
                .HasMaxLength(20);

            builder.HasIndex(p => p.CPF)
                .IsUnique();

            builder.HasOne(p => p.Endereco)
                .WithOne(e => e.Pessoa)
                .HasForeignKey<Endereco>(e => e.PessoaId);

            builder.Ignore(p => p.EnderecoCompleto);
        }
    }
}
