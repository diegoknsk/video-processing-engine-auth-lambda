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
    public void Validate_WhenNameIsEmpty_ShouldHaveValidationError()
    {
        // Arrange
        var input = new CreateUserInput { Name = string.Empty, Email = "user@example.com", Password = "ValidPassword123" };

        // Act
        var result = _validator.TestValidate(input);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("Name é obrigatório.");
    }

    [Fact]
    public void Validate_WhenNameExceedsMaxLength_ShouldHaveValidationError()
    {
        // Arrange
        var longName = new string('a', 257);
        var input = new CreateUserInput { Name = longName, Email = "user@example.com", Password = "ValidPassword123" };

        // Act
        var result = _validator.TestValidate(input);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("Name deve ter no máximo 256 caracteres.");
    }

    [Fact]
    public void Validate_WhenEmailIsEmpty_ShouldHaveValidationError()
    {
        // Arrange
        var input = new CreateUserInput { Name = "Diego", Email = string.Empty, Password = "ValidPassword123" };

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
        var input = new CreateUserInput { Name = "Diego", Email = "notanemail", Password = "ValidPassword123" };

        // Act
        var result = _validator.TestValidate(input);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage("Email deve ser um formato válido.");
    }

    [Fact]
    public void Validate_WhenEmailExceedsMaxLength_ShouldHaveValidationError()
    {
        // Arrange
        var longEmail = "a" + new string('b', 250) + "@example.com";
        var input = new CreateUserInput { Name = "Diego", Email = longEmail, Password = "ValidPassword123" };

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
        var input = new CreateUserInput { Name = "Diego", Email = "user@example.com", Password = "ValidPassword123" };

        // Act
        var result = _validator.TestValidate(input);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Validate_WhenPasswordIsEmpty_ShouldHaveValidationError()
    {
        // Arrange
        var input = new CreateUserInput { Name = "Diego", Email = "user@example.com", Password = string.Empty };

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
        var input = new CreateUserInput { Name = "Diego", Email = "user@example.com", Password = "1234567" };

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
        var input = new CreateUserInput { Name = "Diego", Email = "user@example.com", Password = longPassword };

        // Act
        var result = _validator.TestValidate(input);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Password)
            .WithErrorMessage("Password deve ter no máximo 256 caracteres.");
    }

    [Fact]
    public void Validate_WhenInputIsCompletelyValid_ShouldNotHaveValidationErrors()
    {
        // Arrange
        var input = new CreateUserInput { Name = "Diego", Email = "diegosa@email.com", Password = "ValidPassword123" };

        // Act
        var result = _validator.TestValidate(input);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
