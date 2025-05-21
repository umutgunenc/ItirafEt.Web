using ItirafEt.Shared.DTOs;
using Refit;

namespace ItirafEt.SharedComponents.Apis 
{ 

    public interface IAuthApi
    {
        [Post("/api/auth/login")]
        Task<AuthResponseDto> LoginAsync(LoginDto dto);


        [Post("/api/auth/register")]
        Task<ApiResponses> RegisterAsync(RegisterDto dto);
    }
}
