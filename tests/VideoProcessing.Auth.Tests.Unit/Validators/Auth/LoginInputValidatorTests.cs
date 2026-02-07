using FluentAssertions;
using FluentValidation.TestHelper;
using VideoProcessing.Auth.Application.InputModels.Auth;
using VideoProcessing.Auth.Application.Validators.Auth;

namespace VideoProcessing.Auth.Tests.Unit.Validators.Auth;

public class LoginInputValidatorTests
{
    private readonly LoginInputValidator _validator;

    public LoginInputValidatorTests()
    {
        _validator = new LoginInputValidator();
    }

    [Fact]
    public void Validate_WhenUsernameIsEmpty_ShouldHaveValidationError()
    {
        // Arrange
        var input = new LoginInput { Username = string.Empty, Password = "ValidPassword123" };

        // Act
        var result = _validator.TestValidate(input);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Username)
            .WithErrorMessage("Username é obrigatório.");
    }

    [Fact]
    public void Validate_WhenUsernameExceedsMaxLength_ShouldHaveValidationError()
    {
        // Arrange
        var longUsername = new string('a', 129);
        var input = new LoginInput { Username = longUsername, Password = "ValidPassword123" };

        // Act
        var result = _validator.TestValidate(input);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Username)
            .WithErrorMessage("Username deve ter no máximo 128 caracteres.");
    }

    [Fact]
    public void Validate_WhenPasswordIsEmpty_ShouldHaveValidationError()
    {
        // Arrange
        var input = new LoginInput { Username = "testuser", Password = string.Empty };

        // Act
        var result = _validator.TestValidate(input);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Password)
            .WithErrorMessage("Password é obrigatório.");
    }

    [Fact]
    public void Validate_WhenPasswordIsTooShort_ShouldHaveValidationError()
    {
        // Arrange
        var input = new LoginInput { Username = "testuser", Password = "1234567" };

        // Act
        var result = _validator.TestValidate(input);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Password)
            .WithErrorMessage("Password deve ter pelo menos 8 caracteres.");
    }

    [Fact]
    public void Validate_WhenPasswordExceedsMaxLength_ShouldHaveValidationError()
    {
        // Arrange
        var longPassword = new string('a', 257);
        var input = new LoginInput { Username = "testuser", Password = longPassword };

        // Act
        var result = _validator.TestValidate(input);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Password)
            .WithErrorMessage("Password deve ter no máximo 256 caracteres.");
    }

    [Fact]
    public void Validate_WhenInputIsValid_ShouldNotHaveValidationErrors()
    {
        // Arrange
        var input = new LoginInput { Username = "testuser", Password = "ValidPassword123" };

        // Act
        var result = _validator.TestValidate(input);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
