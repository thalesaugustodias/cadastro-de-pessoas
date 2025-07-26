using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using CadastroDePessoas.Application.Behaviors;
using MediatR;

namespace CadastroDePessoas.IoC
{
    public static class InjecaoMediatR
    {
        public static IServiceCollection AdicionarMediatR(this IServiceCollection services)
        {
            var applicationAssembly = Assembly.Load("CadastroDePessoas.Application");

            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(applicationAssembly));
            
            services.AddValidatorsFromAssembly(applicationAssembly);
            
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

            return services;
        }
    }
}
