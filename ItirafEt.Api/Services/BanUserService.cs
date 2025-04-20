using ItirafEt.Api.Data;
using ItirafEt.Shared.DTOs;
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

        public async Task<ApiResponses<List<BanUserDto>>> GetAllUsers()
        {
            var userQuery = _context.Users
                .Select(u => new BanUserDto
                {
                    UserId = u.Id,
                    UserName = u.UserName,
                    IsBanned = u.IsBanned,
                    BannedDateUntil = u.BannedDateUntil,
                });

            var users = await userQuery.AsNoTracking().ToListAsync();

            if (users == null)
                return ApiResponses<List<BanUserDto>>.Fail("Kullanıcı bulunamadı.");
            return ApiResponses<List<BanUserDto>>.Success(users);

        }

        public async Task<ApiResponses> BanUser(BanUserDto dto, Guid AdminastorUserId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == dto.UserId);
            if (user == null)
                return ApiResponses.Fail("Kullanıcı bulunamadı.");

            if(user.Id == AdminastorUserId)
                return ApiResponses.Fail("Kendinizi banlayamazsınız.");

            if(user.UserName.ToUpper() != dto.UserName.ToUpper())
                return ApiResponses.Fail("Kullanıcı adını değiştirmezsiniz.");

            if(dto.BannedDateUntil != null && dto.BannedDateUntil <= DateTime.Now && dto.IsBanned)
                return ApiResponses.Fail("Ban bitiş tarihi geçmiş bir tarih olamaz.");

            if (dto.BannedDateUntil == null && dto.IsBanned)
                return ApiResponses.Fail("Ban bitiş tarihi boş olamaz.");

            user.AdminastorUserId = AdminastorUserId;
            user.IsBanned = dto.IsBanned;
            if(dto.IsBanned)
                user.BannedDateUntil = dto.BannedDateUntil;
            else
                user.BannedDateUntil = null;

            user.BannedDate = DateTime.Now;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return ApiResponses.Success();
        }
    }
}
