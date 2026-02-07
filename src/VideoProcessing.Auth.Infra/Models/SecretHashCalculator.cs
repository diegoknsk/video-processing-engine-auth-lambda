using System.Security.Cryptography;
using System.Text;

namespace VideoProcessing.Auth.Infra.Models;

/// <summary>
/// Calculadora de SECRET_HASH para autenticação com Amazon Cognito.
/// O SECRET_HASH é necessário quando o App Client tem um Client Secret configurado.
/// </summary>
public static class SecretHashCalculator
{
    /// <summary>
    /// Calcula o SECRET_HASH usando HMAC-SHA256 conforme especificação do Amazon Cognito.
    /// </summary>
    /// <param name="username">Nome de usuário.</param>
    /// <param name="clientId">ID do App Client do Cognito.</param>
    /// <param name="clientSecret">Secret do App Client do Cognito.</param>
    /// <returns>SECRET_HASH codificado em Base64.</returns>
    public static string ComputeSecretHash(string username, string clientId, string clientSecret)
    {
        var message = username + clientId;
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(clientSecret));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(message));
        return Convert.ToBase64String(hash);
    }
}
