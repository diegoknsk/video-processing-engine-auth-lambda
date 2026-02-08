using FluentValidation;
using VideoProcessing.Auth.Application.InputModels.Auth;

namespace VideoProcessing.Auth.Application.Validators.Auth;

/// <summary>
/// Validador para LoginInput usando FluentValidation.
/// </summary>
public class LoginInputValidator : AbstractValidator<LoginInput>
{
    public LoginInputValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email é obrigatório.")
            .MaximumLength(256)
            .WithMessage("Email deve ter no máximo 256 caracteres.")
            .EmailAddress()
            .WithMessage("Email deve ser um formato válido.");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password é obrigatório.")
            .MinimumLength(8)
            .WithMessage("Password deve ter pelo menos 8 caracteres.")
            .MaximumLength(256)
            .WithMessage("Password deve ter no máximo 256 caracteres.");
    }
}
