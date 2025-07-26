using CadastroDePessoas.Domain.Entidades;
using Microsoft.EntityFrameworkCore;
using BC = BCrypt.Net.BCrypt;

namespace CadastroDePessoas.Infraestructure.Contexto
{
    public class AppDbContexto : DbContext
    {
        public DbSet<Pessoa> Pessoas { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }

        public AppDbContexto(DbContextOptions<AppDbContexto> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContexto).Assembly);

            var senhaHasheada = BC.HashPassword("Admin@123", 12);
            var adminId = Guid.Parse("11111111-1111-1111-1111-111111111111");

            modelBuilder.Entity<Usuario>().HasData(
                new
                {
                    Id = adminId,
                    Nome = "Administrador",
                    Email = "admin@exemplo.com",
                    Senha = senhaHasheada,
                    DataCadastro = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new
                {
                    Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                    Nome = "Usuário Teste",
                    Email = "user@teste.com",
                    Senha = BC.HashPassword("User@123", 12),
                    DataCadastro = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                }
            );
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
