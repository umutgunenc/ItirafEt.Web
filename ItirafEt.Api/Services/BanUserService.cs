using ItirafEt.Api.Data;
using ItirafEt.Shared.DTOs;
using Microsoft.EntityFrameworkCore;

namespace ItirafEt.Api.Services
{
    public class BanUserService
    {
        private readonly Context _context;
        public BanUserService(Context context)
        {
            _context = context;
        }

        public async Task<List<BanUserDto>> GetAllUsers()
        {
            var userQuery = _context.Users
                .Select(u => new BanUserDto
                {
                    UserId = u.Id,
                    UserName = u.UserName,
                    IsBanned = u.IsBanned,
                    BannedDateUntil = u.BannedDateUntil,
                    AdminUserName = u.AdminastorUserId != null ? _context.Users.FirstOrDefault(x => x.Id == u.AdminastorUserId).UserName : null
                });

            return await userQuery.AsNoTracking().ToListAsync();

        }

        public async Task<ApiResponse> BanUser(BanUserDto dto, Guid AdminastorUserId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == dto.UserId);
            if (user == null)
                return ApiResponse.Fail("Kullanıcı bulunamadı.");

            if(user.Id == AdminastorUserId)
                return ApiResponse.Fail("Kendinizi banlayamazsınız.");

            if(user.UserName.ToUpper() != dto.UserName.ToUpper())
                return ApiResponse.Fail("Kullanıcı adını değiştirmezsiniz.");

            if(dto.BannedDateUntil != null && dto.BannedDateUntil <= DateTime.Now && dto.IsBanned)
                return ApiResponse.Fail("Ban bitiş tarihi geçmiş bir tarih olamaz.");

            if (dto.BannedDateUntil == null && dto.IsBanned)
                return ApiResponse.Fail("Ban bitiş tarihi boş olamaz.");

            user.AdminastorUserId = AdminastorUserId;
            user.IsBanned = dto.IsBanned;
            if(dto.IsBanned)
                user.BannedDateUntil = dto.BannedDateUntil;
            else
                user.BannedDateUntil = null;

            user.BannedDate = DateTime.Now;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return ApiResponse.Success();
        }
    }
}
