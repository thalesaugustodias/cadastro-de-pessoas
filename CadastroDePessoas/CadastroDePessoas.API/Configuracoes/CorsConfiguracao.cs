namespace CadastroDePessoas.API.Configuracoes
{
    public static class CorsConfiguracao
    {
        public static IServiceCollection AdicionarCorsConfiguracao(this IServiceCollection services, IConfiguration configuration)
        {
            var corsOrigins = configuration.GetSection("Cors:Origins").Get<string[]>() 
                ?? new[] { "http://localhost:3000", "https://localhost:3000" };

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
