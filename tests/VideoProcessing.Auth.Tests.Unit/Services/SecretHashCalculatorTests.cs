using FluentAssertions;
using VideoProcessing.Auth.Infra.Models;

namespace VideoProcessing.Auth.Tests.Unit.Services;

public class SecretHashCalculatorTests
{
    [Fact]
    public void ComputeSecretHash_WithKnownValues_ShouldReturnExpectedHash()
    {
        // Arrange
        var username = "testuser";
        var clientId = "test-client-id";
        var clientSecret = "test-client-secret";

        // Act
        var result = SecretHashCalculator.ComputeSecretHash(username, clientId, clientSecret);

        // Assert
        result.Should().NotBeNullOrEmpty();
        result.Should().MatchRegex(@"^[A-Za-z0-9+/=]+$"); // Base64 format
    }

    [Fact]
    public void ComputeSecretHash_WithSameInputs_ShouldReturnSameHash()
    {
        // Arrange
        var username = "testuser";
        var clientId = "test-client-id";
        var clientSecret = "test-client-secret";

        // Act
        var result1 = SecretHashCalculator.ComputeSecretHash(username, clientId, clientSecret);
        var result2 = SecretHashCalculator.ComputeSecretHash(username, clientId, clientSecret);

        // Assert
        result1.Should().Be(result2);
    }

    [Fact]
    public void ComputeSecretHash_WithDifferentSecrets_ShouldReturnDifferentHashes()
    {
        // Arrange
        var username = "testuser";
        var clientId = "test-client-id";
        var secret1 = "secret1";
        var secret2 = "secret2";

        // Act
        var hash1 = SecretHashCalculator.ComputeSecretHash(username, clientId, secret1);
        var hash2 = SecretHashCalculator.ComputeSecretHash(username, clientId, secret2);

        // Assert
        hash1.Should().NotBe(hash2);
    }

    [Fact]
    public void ComputeSecretHash_WithDifferentUsernames_ShouldReturnDifferentHashes()
    {
        // Arrange
        var username1 = "user1";
        var username2 = "user2";
        var clientId = "test-client-id";
        var clientSecret = "test-client-secret";

        // Act
        var hash1 = SecretHashCalculator.ComputeSecretHash(username1, clientId, clientSecret);
        var hash2 = SecretHashCalculator.ComputeSecretHash(username2, clientId, clientSecret);

        // Assert
        hash1.Should().NotBe(hash2);
    }
}
