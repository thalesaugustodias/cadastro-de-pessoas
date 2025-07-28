using CadastroDePessoas.Application.Interfaces;
using CadastroDePessoas.Application.Services;
using CadastroDePessoas.Domain.Interfaces;
using CadastroDePessoas.Infraestructure.Cache;
using CadastroDePessoas.Infraestructure.Contexto;
using CadastroDePessoas.Infraestructure.Repositorios;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace CadastroDePessoas.IoC
{
    public static class InjecaoDependencia
    {
        public static IServiceCollection AdicionarInfraestrutura(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppDbContexto>(options =>
                 options.UseSqlite(configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<IRepositorioPessoa, RepositorioPessoa>();
            services.AddScoped<IRepositorioUsuario, RepositorioUsuario>();

            services.AddScoped<IServiceToken, ServiceToken>();
            services.AddScoped<ExportacaoService>();
            services.AddScoped<ImportacaoService>();

            if (bool.Parse(configuration["UseRedisCache"] ?? "false"))
            {
                services.AddSingleton<IConnectionMultiplexer>(sp =>
                    ConnectionMultiplexer.Connect(configuration.GetConnectionString("Redis")));
                services.AddSingleton<IServiceCache, ServiceRedisCache>();
            }
            else
            {
                services.AddDistributedMemoryCache();
                services.AddSingleton<IServiceCache, ServicoCache>();
            }

            return services;
        }
    }
}
