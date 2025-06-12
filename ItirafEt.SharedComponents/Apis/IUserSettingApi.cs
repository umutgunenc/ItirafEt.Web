using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ItirafEt.Shared.ViewModels;
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
    }
}
