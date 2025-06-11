using ItirafEt.Api.Data;
using ItirafEt.Api.Data.Entities;
using ItirafEt.Shared.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace ItirafEt.Api.Services
{
    public class UserSettingService
    {
        private readonly dbContext _context;

        public UserSettingService(dbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponses<UserSettingsInfoViewModel>> GetUserInfoAsync(Guid userId)
        {
            var userInfo = await _context.Users
                .AsNoTracking()
                .Where(u => u.Id == userId)
                .Select(u => new UserSettingsInfoViewModel
                {
                    UserId = u.Id,
                    UserName = u.UserName,
                    Email = u.Email,
                    BirthDate = u.BirthDate,
                    GenderId = u.GenderId,
                    ProfileImageUrl = u.ProfilePictureUrl
                })
                .FirstOrDefaultAsync();

            if (userInfo == null)
                return ApiResponses<UserSettingsInfoViewModel>.Fail("Kullanıcı bulunamadı");

            return ApiResponses<UserSettingsInfoViewModel>.Success(userInfo);

        }
    }
}
