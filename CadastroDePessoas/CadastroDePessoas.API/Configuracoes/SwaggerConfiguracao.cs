using Microsoft.OpenApi.Models;
using System.Reflection;

namespace CadastroDePessoas.API.Configuracoes
{
    public static class SwaggerConfiguracao
    {
        public static IServiceCollection AdicionarSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Cadastro de Pessoas API",
                    Version = "v1",
                    Description = "API para cadastro de pessoas com operações CRUD",
                    Contact = new OpenApiContact
                    {
                        Name = "Seu Nome",
                        Email = "seuemail@exemplo.com"
                    }
                });

                c.SwaggerDoc("v2", new OpenApiInfo
                {
                    Title = "Cadastro de Pessoas API",
                    Version = "v2",
                    Description = "API para cadastro de pessoas com operações CRUD - Versão 2 com endereço obrigatório",
                    Contact = new OpenApiContact
                    {
                        Name = "Seu Nome",
                        Email = "seuemail@exemplo.com"
                    }
                });

                // Configuração para JWT
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header usando o esquema Bearer. Exemplo: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });

                // Adicionar comentários XML
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath))
                {
                    c.IncludeXmlComments(xmlPath);
                }
            });

            return services;
        }

        public static IApplicationBuilder UsarSwagger(this IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Cadastro de Pessoas API v1");
                c.SwaggerEndpoint("/swagger/v2/swagger.json", "Cadastro de Pessoas API v2");
                c.RoutePrefix = string.Empty;
            });

            return app;
        }
    }
}
