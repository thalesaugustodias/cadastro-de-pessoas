using CadastroDePessoas.Domain.Entidades;
using Microsoft.EntityFrameworkCore;

namespace CadastroDePessoas.Infraestructure.Contexto
{
    public class AppDbContexto : DbContext
    {
        public DbSet<Pessoa> Pessoas { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }

        public AppDbContexto(DbContextOptions<AppDbContexto> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // Configuração para H2
                optionsBuilder.UseH2("jdbc:h2:mem:cadastro_pessoas;DB_CLOSE_DELAY=-1", option => {
                    option.MigrationsAssembly(typeof(AppDbContexto).Assembly.FullName);
                });
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Aplica todas as configurações de mapeamento
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContexto).Assembly);

            // Seed inicial de usuário admin
            var adminId = Guid.NewGuid();
            modelBuilder.Entity<Usuario>().HasData(
                new
                {
                    Id = adminId,
                    Nome = "Administrador",
                    Email = "admin@exemplo.com",
                    // Senha: Admin@123
                    Senha = "$2a$11$zd2CNBJ/6iQn6eN1QkLUAOLkL9JA7LrRrTVLQxXKp1lzHOa1OD3Q2",
                    DataCadastro = DateTime.UtcNow
                }
            );
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
