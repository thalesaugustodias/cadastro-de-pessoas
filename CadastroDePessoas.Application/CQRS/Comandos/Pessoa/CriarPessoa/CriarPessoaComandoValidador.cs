using CadastroDePessoas.Domain.Validacoes;
using FluentValidation;

namespace CadastroDePessoas.Application.CQRS.Comandos.Pessoa.CriarPessoa
{
    public class CriarPessoaComandoValidador : AbstractValidator<CriarPessoaComando>
    {
        public CriarPessoaComandoValidador()
        {
            RuleFor(x => x.Nome)
                .NotEmpty().WithMessage("O nome é obrigatório")
                .MaximumLength(100).WithMessage("O nome deve ter no máximo 100 caracteres");

            RuleFor(x => x.Email)
                .EmailAddress().WithMessage("O e-mail informado não é válido")
                .When(x => !string.IsNullOrEmpty(x.Email));

            RuleFor(x => x.DataNascimento)
                .NotEmpty().WithMessage("A data de nascimento é obrigatória")
                .Must(BeValidBirthDate).WithMessage("A data de nascimento deve ser menor que a data atual e maior que 01/01/1900");

            RuleFor(x => x.CPF)
                .NotEmpty().WithMessage("O CPF é obrigatório")
                .Must(ValidadorCPF.Validar).WithMessage("O CPF informado não é válido");
        }

        private bool BeValidBirthDate(DateTime date)
        {
            return date < DateTime.Now && date > new DateTime(1900, 1, 1);
        }
    }
}
