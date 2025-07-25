using FluentValidation;

namespace CadastroDePessoas.Application.CQRS.Comandos.Usuario.CriarUsuario
{
    public class CriarUsuarioComandoValidator : AbstractValidator<CriarUsuarioComando>
    {
        public CriarUsuarioComandoValidator()
        {
            RuleFor(x => x.Nome)
                .NotEmpty()
                .WithMessage("O nome � obrigat�rio")
                .MinimumLength(2)
                .WithMessage("O nome deve ter pelo menos 2 caracteres")
                .MaximumLength(100)
                .WithMessage("O nome deve ter no m�ximo 100 caracteres");

            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("O e-mail � obrigat�rio")
                .EmailAddress()
                .WithMessage("E-mail inv�lido")
                .MaximumLength(100)
                .WithMessage("O e-mail deve ter no m�ximo 100 caracteres");

            RuleFor(x => x.Senha)
                .NotEmpty()
                .WithMessage("A senha � obrigat�ria")
                .MinimumLength(6)
                .WithMessage("A senha deve ter pelo menos 6 caracteres")
                .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]")
                .WithMessage("A senha deve conter pelo menos: 1 letra min�scula, 1 mai�scula, 1 n�mero e 1 caractere especial");
        }
    }
}