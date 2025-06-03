using ItirafEt.Shared.ViewModels;
using Refit;

namespace ItirafEt.SharedComponents.Apis
{
    [Headers("Authorization: Bearer")]

    public interface IBanUserApi
    {
        [Post("/api/banUser")]
        Task<ApiResponses> BanUserAsync(BanUserViewModel bannedUser,Guid AdminId);


        [Get("/api/getAllUsers")]
        Task<ApiResponses<List<BanUserViewModel>>> GetAllUserDtosAsync();
    }
}
