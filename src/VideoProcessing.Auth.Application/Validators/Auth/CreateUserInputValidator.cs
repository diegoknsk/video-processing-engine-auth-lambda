using FluentValidation;
using VideoProcessing.Auth.Application.InputModels.Auth;

namespace VideoProcessing.Auth.Application.Validators.Auth;

/// <summary>
/// Validador para CreateUserInput usando FluentValidation.
/// </summary>
public class CreateUserInputValidator : AbstractValidator<CreateUserInput>
{
    public CreateUserInputValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty()
            .WithMessage("Username é obrigatório.")
            .MaximumLength(128)
            .WithMessage("Username deve ter no máximo 128 caracteres.")
            .Matches("^[a-zA-Z0-9_-]+$")
            .WithMessage("Username pode conter apenas letras, números, _ e -.");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password é obrigatório.")
            .MinimumLength(8)
            .WithMessage("Password deve ter pelo menos 8 caracteres.")
            .MaximumLength(256)
            .WithMessage("Password deve ter no máximo 256 caracteres.");

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email é obrigatório.")
            .EmailAddress()
            .WithMessage("Email em formato inválido.")
            .MaximumLength(256)
            .WithMessage("Email deve ter no máximo 256 caracteres.");
    }
}
