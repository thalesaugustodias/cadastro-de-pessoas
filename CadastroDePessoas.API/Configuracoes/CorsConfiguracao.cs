using static System.Net.WebRequestMethods;

namespace CadastroDePessoas.API.Configuracoes
{
    public static class CorsConfiguracao
    {
        public static IServiceCollection AdicionarCorsConfiguracao(this IServiceCollection services, IConfiguration configuration)
        {
            var corsOrigins = configuration.GetSection("Cors:Origins").Get<string[]>() 
                ?? ["http://localhost:3000", "https://localhost:3000", "http://localhost:3001", "https://localhost:3001", "https://cadastro-de-pessoas-vina.onrender.com"];

            services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder.WithOrigins(corsOrigins)
                           .AllowAnyMethod()
                           .AllowAnyHeader()
                           .AllowCredentials();
                });
            });

            return services;
        }

        public static IApplicationBuilder UsarCorsConfiguracao(this IApplicationBuilder app)
        {
            app.UseCors();
            return app;
        }
    }
}
