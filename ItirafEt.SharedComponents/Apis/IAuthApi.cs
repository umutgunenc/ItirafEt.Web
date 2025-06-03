using ItirafEt.Shared.ViewModels;
using Refit;

namespace ItirafEt.SharedComponents.Apis 
{ 

    public interface IAuthApi
    {
        [Post("/api/auth/login")]
        Task<AuthResponse> LoginAsync(LoginViewModel model);


        [Post("/api/auth/register")]
        Task<ApiResponses> RegisterAsync(RegisterViewModel model);
    }
}
