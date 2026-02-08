namespace VideoProcessing.Auth.Infra.Models;

/// <summary>
/// Opções de configuração do Amazon Cognito (App Client público, sem secret).
/// Valores reais devem ser configurados via variáveis de ambiente (formato: Cognito__Region, Cognito__ClientId, Cognito__UserPoolId).
/// </summary>
public class CognitoOptions
{
    public string Region { get; set; } = string.Empty;
    public string UserPoolId { get; set; } = string.Empty;
    public string ClientId { get; set; } = string.Empty;
}
