using ItirafEt.Shared.DTOs;
using Refit;

namespace ItirafEt.SharedComponents.Apis
{
    [Headers("Authorization: Bearer")]

    public interface IBanUserApi
    {
        [Post("/api/banUser")]
        Task<ApiResponses> BanUserAsync(BanUserDto bannedUser,Guid AdminId);


        [Get("/api/getAllUsers")]
        Task<ApiResponses<List<BanUserDto>>> GetAllUserDtosAsync();
    }
}
