using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ItirafEt.Shared.ViewModels;
using Microsoft.AspNetCore.Http;
using Refit;

namespace ItirafEt.SharedComponents.Apis
{
    public interface IUserSettingApi
    {
        [Headers("Authorization: Bearer")]
        [Get("/api/getUserSettingsInfo")]
        Task<ApiResponses<UserSettingsInfoViewModel>> GetUserSettingsInfoAsync(Guid userId);


        [Headers("Authorization: Bearer")]
        [Post("/api/changeUserSettingsInfo")]
        Task<ApiResponses<UserSettingsInfoViewModel>> ChangeUserSettingsInfoAsync(Guid userId, UserSettingsInfoViewModel model);

        [Headers("Authorization: Bearer")]
        [Post("/api/changeUserPassword")]
        Task<ApiResponses<string>> ChangeUserPasswordAsync(Guid userId, UserSettingsChangePaswordViewModel model);

        [Headers("Authorization: Bearer")]
        [Post("/api/userDeactive")]
        Task<ApiResponses<string>> UserDeactiveAsync(Guid userId, UserDeactiveViewModel model);


        [Multipart]
        [Headers("Authorization: Bearer")]
        [Post("/api/changeUserProfilePicture")]
        Task<ApiResponses<string>> ChangeProfilePictureAsync(
            [AliasAs("UserId")] string userId,
            [AliasAs("Photo")] StreamPart? photo
        );

        [Headers("Authorization: Bearer")]
        [Post("/api/deleteUserProfilePicture")]
        Task<ApiResponses> DeleteUserProfilePictureAsync(Guid userId);


        [Headers("Authorization: Bearer")]
        [Post("/api/changeProfileVisibilty")]
        Task<ApiResponses> ChangeProfileVisibilty(Guid userId);

        [Post("/api/userActivate")]
        Task<ApiResponses<string>> UserActivateAsync(Guid userId, string token);

    }
}
