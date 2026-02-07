using VideoProcessing.Auth.Application.OutputModels.Auth;
using VideoProcessing.Auth.Application.ResponseModels.Auth;

namespace VideoProcessing.Auth.Application.Presenters.Auth;

/// <summary>
/// Presenter responsável por transformar CreateUserOutput (modelo interno) em CreateUserResponseModel (contrato da API).
/// </summary>
public static class CreateUserPresenter
{
    /// <summary>
    /// Transforma um CreateUserOutput em CreateUserResponseModel.
    /// </summary>
    /// <param name="output">Modelo de saída do caso de uso.</param>
    /// <returns>Modelo de resposta da API.</returns>
    public static CreateUserResponseModel Present(CreateUserOutput output)
    {
        return new CreateUserResponseModel
        {
            UserId = output.UserId,
            Username = output.Username,
            UserConfirmed = output.UserConfirmed,
            ConfirmationRequired = output.ConfirmationRequired
        };
    }
}
