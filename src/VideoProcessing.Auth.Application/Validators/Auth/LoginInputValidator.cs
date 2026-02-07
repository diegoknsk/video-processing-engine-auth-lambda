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
        RuleFor(x => x.Username)
            .NotEmpty()
            .WithMessage("Username é obrigatório.")
            .MaximumLength(128)
            .WithMessage("Username deve ter no máximo 128 caracteres.");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password é obrigatório.")
            .MinimumLength(8)
            .WithMessage("Password deve ter pelo menos 8 caracteres.")
            .MaximumLength(256)
            .WithMessage("Password deve ter no máximo 256 caracteres.");
    }
}
