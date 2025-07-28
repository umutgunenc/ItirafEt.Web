using ItirafEt.Api.Data;
using ItirafEt.Shared.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace ItirafEt.Api.Services
{
    public class BanUserService
    {
        private readonly dbContext _context;
        public BanUserService(dbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponses<List<BanUserViewModel>>> GetAllUsers()
        {
            var userQuery = _context.Users
                .Select(u => new BanUserViewModel
                {
                    UserId = u.Id,
                    UserName = u.UserName,
                    IsBanned = u.IsBanned,
                    BannedDateUntil = u.BannedDateUntil,
                });

            var users = await userQuery.AsNoTracking().ToListAsync();

            if (users == null)
                return ApiResponses<List<BanUserViewModel>>.Fail("Kullanıcı bulunamadı.");
            return ApiResponses<List<BanUserViewModel>>.Success(users);

        }

        public async Task<ApiResponses> BanUser(BanUserViewModel model, Guid AdminastorUserId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == model.UserId);
            if (user == null)
                return ApiResponses.Fail("Kullanıcı bulunamadı.");

            if(user.Id == AdminastorUserId)
                return ApiResponses.Fail("Kendinizi banlayamazsınız.");

            if(user.UserName.ToUpper() != model.UserName.ToUpper())
                return ApiResponses.Fail("Kullanıcı adını değiştirmezsiniz.");

            if(model.BannedDateUntil != null && model.BannedDateUntil <= DateTime.UtcNow && model.IsBanned)
                return ApiResponses.Fail("Ban bitiş tarihi geçmiş bir tarih olamaz.");

            if (model.BannedDateUntil == null && model.IsBanned)
                return ApiResponses.Fail("Ban bitiş tarihi boş olamaz.");

            user.AdminastorUserId = AdminastorUserId;
            user.IsBanned = model.IsBanned;
            if(model.IsBanned)
                user.BannedDateUntil = model.BannedDateUntil;
            else
                user.BannedDateUntil = null;

            user.BannedDate = DateTime.UtcNow;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return ApiResponses.Success();
        }
    }
}
