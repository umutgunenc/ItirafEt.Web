using ItirafEt.Shared.DTOs;
using Refit;

namespace ItirafEt.Web.Apis
{
    [Headers("Authorization: Bearer")]

    public interface IBanUserApi
    {
        [Post("/api/banUser")]
        Task<ApiResponse> BanUserAsync(BanUserDto bannedUser,Guid AdminId);


        [Get("/api/getAllUsers")]
        Task<List<BanUserDto>> GetAllUserDtosAsync();
    }
}
