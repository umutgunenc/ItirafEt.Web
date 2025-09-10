using ItirafEt.Shared.ViewModels;
using ItirafEt.SharedComponents.Pages.Auth;
using Refit;

namespace ItirafEt.SharedComponents.Apis 
{ 

    public interface IAuthApi
    {
        [Post("/api/auth/login")]
        Task<AuthResponse> LoginAsync(LoginViewModel model);


        [Post("/api/auth/register")]
        Task<ApiResponses> RegisterAsync(RegisterViewModel model);

        [Post("/api/auth/forgotPassword")]
        Task<ApiResponses> ForgotPasswordAsync(ForgotPaswordViewModel model);


        [Post("/api/auth/changeUserPassword")]
        Task<ApiResponses> ChangeUserPasswordAsync(ChangeUserPasswordViewModel model);


    }
}
