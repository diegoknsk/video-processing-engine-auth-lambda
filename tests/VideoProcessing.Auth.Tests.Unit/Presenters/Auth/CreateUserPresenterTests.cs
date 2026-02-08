using FluentAssertions;
using VideoProcessing.Auth.Application.OutputModels.Auth;
using VideoProcessing.Auth.Application.Presenters.Auth;
using VideoProcessing.Auth.Application.ResponseModels.Auth;

namespace VideoProcessing.Auth.Tests.Unit.Presenters.Auth;

public class CreateUserPresenterTests
{
    [Fact]
    public void Present_WhenOutputIsProvided_ShouldMapToResponseModel()
    {
        // Arrange
        var output = new CreateUserOutput
        {
            UserId = "user-sub-123",
            Username = "testuser",
            UserConfirmed = true,
            ConfirmationRequired = false
        };

        // Act
        var result = CreateUserPresenter.Present(output);

        // Assert
        result.Should().NotBeNull();
        result.UserId.Should().Be("user-sub-123");
        result.Username.Should().Be("testuser");
        result.UserConfirmed.Should().BeTrue();
        result.ConfirmationRequired.Should().BeFalse();
    }

    [Fact]
    public void Present_WhenUserConfirmedIsTrue_ShouldSetConfirmationRequiredToFalse()
    {
        // Arrange
        var output = new CreateUserOutput
        {
            UserId = "user-sub-123",
            Username = "testuser",
            UserConfirmed = true,
            ConfirmationRequired = false
        };

        // Act
        var result = CreateUserPresenter.Present(output);

        // Assert
        result.UserConfirmed.Should().BeTrue();
        result.ConfirmationRequired.Should().BeFalse();
    }

    [Fact]
    public void Present_WhenUserConfirmedIsFalse_ShouldSetConfirmationRequiredToTrue()
    {
        // Arrange
        var output = new CreateUserOutput
        {
            UserId = "user-sub-123",
            Username = "testuser",
            UserConfirmed = false,
            ConfirmationRequired = true
        };

        // Act
        var result = CreateUserPresenter.Present(output);

        // Assert
        result.UserConfirmed.Should().BeFalse();
        result.ConfirmationRequired.Should().BeTrue();
    }

    [Fact]
    public void Present_ShouldMapAllPropertiesCorrectly()
    {
        // Arrange
        var output = new CreateUserOutput
        {
            UserId = "user-sub-456",
            Username = "anotheruser",
            UserConfirmed = false,
            ConfirmationRequired = true
        };

        var expected = new CreateUserResponseModel
        {
            UserId = "user-sub-456",
            Username = "anotheruser",
            UserConfirmed = false,
            ConfirmationRequired = true
        };

        // Act
        var result = CreateUserPresenter.Present(output);

        // Assert
        result.Should().BeEquivalentTo(expected);
    }
}
