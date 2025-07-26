using CadastroDePessoas.Domain.Interfaces;
using BC = BCrypt.Net.BCrypt;
using FluentValidation;
using CadastroDePessoas.Application.Interfaces;

namespace CadastroDePessoas.Application.CQRS.Comandos.Usuario.CriarUsuario
{
    public class CriarUsuarioComandoHandler(IRepositorioUsuario repositorioUsuario) : IHandlerComando<CriarUsuarioComando, bool>
    {
        public async Task<bool> Handle(CriarUsuarioComando comando, CancellationToken cancellationToken)
        {
            var usuarioExistente = await repositorioUsuario.ObterPorEmailAsync(comando.Email);
            if (usuarioExistente != null)
            {
                throw new ValidationException("Já existe um usuário com este e-mail");
            }

            var senhaHasheada = BC.HashPassword(comando.Senha, 12);

            var novoUsuario = new Domain.Entidades.Usuario(
                comando.Nome,
                comando.Email,
                senhaHasheada
            );

            await repositorioUsuario.AdicionarAsync(novoUsuario);

            return true;
        }
    }
}