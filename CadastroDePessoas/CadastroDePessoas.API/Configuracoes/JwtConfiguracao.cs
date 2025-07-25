using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace CadastroDePessoas.API.Configuracoes
{
    public static class JwtConfiguracao
    {
        public static IServiceCollection AdicionarJwtAutenticacao(this IServiceCollection services, IConfiguration configuration)
        {
            var chaveSecreta = Environment.GetEnvironmentVariable("JWT_SECRET_KEY") 
                            ?? configuration["Jwt:Chave"];

            if (string.IsNullOrEmpty(chaveSecreta))
            {
                throw new InvalidOperationException("Chave JWT não configurada. Configure a variável de ambiente JWT_SECRET_KEY ou a configuração Jwt:Chave.");
            }

            var chaveBytes = Encoding.UTF8.GetBytes(chaveSecreta);

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

            services.AddAuthorization(options =>
            {
                // Política padrão: exige autenticação para todos os endpoints
                options.FallbackPolicy = new Microsoft.AspNetCore.Authorization.AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();

                // Política para administradores (futura expansão)
                options.AddPolicy("AdminOnly", policy =>
                    policy.RequireAuthenticatedUser()
                          .RequireClaim("role", "admin"));

                // Política para usuários autenticados
                options.AddPolicy("AuthenticatedUser", policy =>
                    policy.RequireAuthenticatedUser());
            });

            return services;
        }
    }
}