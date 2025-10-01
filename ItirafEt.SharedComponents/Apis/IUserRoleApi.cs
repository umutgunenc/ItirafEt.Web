using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ItirafEt.Shared.ViewModels;
using Refit;

namespace ItirafEt.SharedComponents.Apis
{
    [Headers("Authorization: Bearer")]
    public interface IUserRoleApi
    {

        [Post("/api/selectUser")]
        Task<ApiResponses<ChangeUserRoleViewModel>> SelectUserAsync(ChangeUserRoleViewModel _model);


        [Post("/api/changeUserRole")]
        Task<ApiResponses> ChangeUserRoleAsync(ChangeUserRoleViewModel _model);


        [Get("/api/getUsersWithRoles")]
        Task<ApiResponses<List<UsersWithRolesViewModel>>> GetUsersWithRolesAsync([Query] string roleName);


        [Get("/api/getAllRoles")]
        Task<ApiResponses<List<string>>> GetAllRolesAsync();
    }
}
