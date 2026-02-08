using FluentAssertions;
using VideoProcessing.Auth.Application.OutputModels.Auth;
using VideoProcessing.Auth.Application.Presenters.Auth;
using VideoProcessing.Auth.Application.ResponseModels.Auth;

namespace VideoProcessing.Auth.Tests.Unit.Presenters.Auth;

public class LoginPresenterTests
{
    [Fact]
    public void Present_WhenOutputIsValid_ShouldMapToResponseModel()
    {
        // Arrange
        var output = new LoginOutput
        {
            AccessToken = "access-token",
            IdToken = "id-token",
            RefreshToken = "refresh-token",
            ExpiresIn = 3600,
            TokenType = "Bearer"
        };

        // Act
        var result = LoginPresenter.Present(output);

        // Assert
        result.Should().NotBeNull();
        result.AccessToken.Should().Be("access-token");
        result.IdToken.Should().Be("id-token");
        result.RefreshToken.Should().Be("refresh-token");
        result.ExpiresIn.Should().Be(3600);
        result.TokenType.Should().Be("Bearer");
    }

    [Fact]
    public void Present_WhenRefreshTokenIsNull_ShouldMapCorrectly()
    {
        // Arrange
        var output = new LoginOutput
        {
            AccessToken = "access-token",
            IdToken = "id-token",
            RefreshToken = null,
            ExpiresIn = 3600,
            TokenType = "Bearer"
        };

        // Act
        var result = LoginPresenter.Present(output);

        // Assert
        result.Should().NotBeNull();
        result.RefreshToken.Should().BeNull();
        result.AccessToken.Should().Be("access-token");
        result.IdToken.Should().Be("id-token");
        result.ExpiresIn.Should().Be(3600);
        result.TokenType.Should().Be("Bearer");
    }

    [Fact]
    public void Present_ShouldReturnEquivalentObject()
    {
        // Arrange
        var output = new LoginOutput
        {
            AccessToken = "access-token",
            IdToken = "id-token",
            RefreshToken = "refresh-token",
            ExpiresIn = 3600,
            TokenType = "Bearer"
        };

        var expected = new LoginResponseModel
        {
            AccessToken = "access-token",
            IdToken = "id-token",
            RefreshToken = "refresh-token",
            ExpiresIn = 3600,
            TokenType = "Bearer"
        };

        // Act
        var result = LoginPresenter.Present(output);

        // Assert
        result.Should().BeEquivalentTo(expected);
    }
}
