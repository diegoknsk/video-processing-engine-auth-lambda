using FluentAssertions;
using FluentValidation.TestHelper;
using VideoProcessing.Auth.Application.InputModels.Auth;
using VideoProcessing.Auth.Application.Validators.Auth;

namespace VideoProcessing.Auth.Tests.Unit.Validators.Auth;

public class CreateUserInputValidatorTests
{
    private readonly CreateUserInputValidator _validator;

    public CreateUserInputValidatorTests()
    {
        _validator = new CreateUserInputValidator();
    }

    [Fact]
    public void Validate_WhenUsernameIsEmpty_ShouldHaveValidationError()
    {
        // Arrange
        var input = new CreateUserInput { Username = string.Empty, Password = "ValidPassword123", Email = "user@example.com" };

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
        var input = new CreateUserInput { Username = longUsername, Password = "ValidPassword123", Email = "user@example.com" };

        // Act
        var result = _validator.TestValidate(input);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Username)
            .WithErrorMessage("Username deve ter no máximo 128 caracteres.");
    }

    [Fact]
    public void Validate_WhenUsernameContainsInvalidCharacters_ShouldHaveValidationError()
    {
        // Arrange
        var input = new CreateUserInput { Username = "user@123", Password = "ValidPassword123", Email = "user@example.com" };

        // Act
        var result = _validator.TestValidate(input);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Username)
            .WithErrorMessage("Username pode conter apenas letras, números, _ e -.");
    }

    [Fact]
    public void Validate_WhenUsernameIsValid_ShouldNotHaveValidationError()
    {
        // Arrange
        var input = new CreateUserInput { Username = "user_test-123", Password = "ValidPassword123", Email = "user@example.com" };

        // Act
        var result = _validator.TestValidate(input);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Username);
    }

    [Fact]
    public void Validate_WhenPasswordIsEmpty_ShouldHaveValidationError()
    {
        // Arrange
        var input = new CreateUserInput { Username = "testuser", Password = string.Empty, Email = "user@example.com" };

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
        var input = new CreateUserInput { Username = "testuser", Password = "1234567", Email = "user@example.com" };

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
        var input = new CreateUserInput { Username = "testuser", Password = longPassword, Email = "user@example.com" };

        // Act
        var result = _validator.TestValidate(input);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Password)
            .WithErrorMessage("Password deve ter no máximo 256 caracteres.");
    }

    [Fact]
    public void Validate_WhenEmailIsEmpty_ShouldHaveValidationError()
    {
        // Arrange
        var input = new CreateUserInput { Username = "testuser", Password = "ValidPassword123", Email = string.Empty };

        // Act
        var result = _validator.TestValidate(input);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage("Email é obrigatório.");
    }

    [Fact]
    public void Validate_WhenEmailIsInvalid_ShouldHaveValidationError()
    {
        // Arrange
        var input = new CreateUserInput { Username = "testuser", Password = "ValidPassword123", Email = "notanemail" };

        // Act
        var result = _validator.TestValidate(input);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage("Email em formato inválido.");
    }

    [Fact]
    public void Validate_WhenEmailExceedsMaxLength_ShouldHaveValidationError()
    {
        // Arrange
        var longEmail = "a" + new string('b', 250) + "@example.com";
        var input = new CreateUserInput { Username = "testuser", Password = "ValidPassword123", Email = longEmail };

        // Act
        var result = _validator.TestValidate(input);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage("Email deve ter no máximo 256 caracteres.");
    }

    [Fact]
    public void Validate_WhenEmailIsValid_ShouldNotHaveValidationError()
    {
        // Arrange
        var input = new CreateUserInput { Username = "testuser", Password = "ValidPassword123", Email = "user@example.com" };

        // Act
        var result = _validator.TestValidate(input);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Validate_WhenInputIsCompletelyValid_ShouldNotHaveValidationErrors()
    {
        // Arrange
        var input = new CreateUserInput { Username = "testuser", Password = "ValidPassword123", Email = "user@example.com" };

        // Act
        var result = _validator.TestValidate(input);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
