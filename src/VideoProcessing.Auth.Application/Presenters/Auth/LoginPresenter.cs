using VideoProcessing.Auth.Application.OutputModels.Auth;
using VideoProcessing.Auth.Application.ResponseModels.Auth;

namespace VideoProcessing.Auth.Application.Presenters.Auth;

/// <summary>
/// Presenter responsável por transformar LoginOutput (modelo interno) em LoginResponseModel (contrato da API).
/// </summary>
public static class LoginPresenter
{
    /// <summary>
    /// Transforma um LoginOutput em LoginResponseModel.
    /// </summary>
    /// <param name="output">Modelo de saída do caso de uso.</param>
    /// <returns>Modelo de resposta da API.</returns>
    public static LoginResponseModel Present(LoginOutput output)
    {
        return new LoginResponseModel
        {
            AccessToken = output.AccessToken,
            IdToken = output.IdToken,
            RefreshToken = output.RefreshToken,
            ExpiresIn = output.ExpiresIn,
            TokenType = output.TokenType
        };
    }
}
