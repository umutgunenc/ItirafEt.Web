using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ItirafEt.Shared.ViewModels;
using Refit;

namespace ItirafEt.SharedComponents.Apis
{
    public interface IUserProfileApi
    {
        [Headers("Authorization: Bearer")]
        [Get("/api/getUserProfile")]
        Task<ApiResponses<UserProfileViewModel>> GetUserProfileAsync(Guid userId);


        [Headers("Authorization: Bearer")]
        [Get("/api/getUserPostsDateOrdered")]
        Task<ApiResponses<UserPostsViewModel>> GetUserProfilePostsByDateAsync(Guid userId,int size,int page);

        [Headers("Authorization: Bearer")]
        [Get("/api/getUserPostsLikeOrdered")]
        Task<ApiResponses<UserPostsViewModel>> GetUserProfilePostsByLikeCountAsync(Guid userId,int size,int page);


        [Headers("Authorization: Bearer")]
        [Get("/api/getUserPostsReadOrdered")]
        Task<ApiResponses<UserPostsViewModel>> GetUserProfilePostsByReadCountAsync(Guid userId,int size,int page);


    }
}
