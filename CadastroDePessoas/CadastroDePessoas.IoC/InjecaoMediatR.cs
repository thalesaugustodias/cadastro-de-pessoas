using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace CadastroDePessoas.IoC
{
    public static class InjecaoMediatR
    {
        public static IServiceCollection AdicionarMediatR(this IServiceCollection services)
        {
            var myHandlers = AppDomain.CurrentDomain.Load("CadastroDePessoas.Application");

            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(myHandlers));
            services.AddValidatorsFromAssembly(Assembly.Load("CadastroDePessoas.Application"));

            return services;
        }
    }
}
