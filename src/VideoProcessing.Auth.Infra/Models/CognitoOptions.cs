namespace VideoProcessing.Auth.Infra.Models;

/// <summary>
/// Opções de configuração do Amazon Cognito.
/// Valores reais devem ser configurados via variáveis de ambiente (formato: Cognito__Region, Cognito__AppClientId, etc.)
/// </summary>
public class CognitoOptions
{
    public string Region { get; set; } = string.Empty;
    public string AppClientId { get; set; } = string.Empty;
    public string AppClientSecret { get; set; } = string.Empty;
    public string UserPoolId { get; set; } = string.Empty;
}
