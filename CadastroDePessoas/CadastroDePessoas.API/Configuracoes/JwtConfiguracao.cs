using System.Text;

namespace CadastroDePessoas.API.Configuracoes
{
    public static class JwtConfiguracao
    {
        public static IServiceCollection AdicionarJwtAutenticacao(this IServiceCollection services, IConfiguration configuration)
        {
            var chaveSecreta = configuration["Jwt:Chave"];
            var chaveBytes = Encoding.ASCII.GetBytes(chaveSecreta);

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(chaveBytes),
                    ValidateIssuer = true,
                    ValidIssuer = configuration["Jwt:Emissor"],
                    ValidateAudience = true,
                    ValidAudience = configuration["Jwt:Audiencia"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });

            return services;
        }
    }
}