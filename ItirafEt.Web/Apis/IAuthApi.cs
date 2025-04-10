using ItirafEt.Shared.DTOs;
using Refit;

namespace ItirafEt.Web.Apis
{
    public interface IAuthApi
    {
        [Post("/api/auth/login")]
        Task<AuthResponseDto> LoginAsync(LoginDto dto);
    }
}
