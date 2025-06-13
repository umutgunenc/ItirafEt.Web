using System.ComponentModel.DataAnnotations;
using ItirafEt.Api.Data;
using ItirafEt.Api.Data.Entities;
using ItirafEt.Shared.Enums;
using ItirafEt.Shared.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ItirafEt.Api.Services
{
    public class UserSettingService
    {
        private readonly dbContext _context;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly AuthService _authService;

        public UserSettingService(dbContext context, IPasswordHasher<User> passwordHasher, AuthService authService)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _authService = authService;
        }

        public async Task<ApiResponses<UserSettingsInfoViewModel>> GetUserInfoAsync(Guid userId)
        {
            var userInfo = await _context.Users
                .AsNoTracking()
                .Where(u => u.Id == userId)
                .Select(u => new UserSettingsInfoViewModel
                {
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

        public async Task<ApiResponses> ChangeUserInfoAsync(Guid userId, UserSettingsInfoViewModel model)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                return ApiResponses.Fail("Kullanıcı bulunamadı");

            if (string.IsNullOrEmpty(model.UserName))
                return ApiResponses.Fail("Kullanıcı adı boş olamaz.");

            if (model.UserName != user.UserName)
            {
                if (_context.Users.Any(u => u.UserName.ToUpper() == model.UserName.ToUpper() && u.Id != userId))
                    return ApiResponses.Fail("Bu kullanıcı adı zaten kullanımda.");
            }

            if (model.UserName.Length < 3 || model.UserName.Length > 64)
                return ApiResponses.Fail("Kullanıcı adı 3 ile 64 karakter arasında olmalıdır.");

            if (string.IsNullOrEmpty(model.Email))
                return ApiResponses.Fail("E-posta adresi boş olamaz.");

            if (!new EmailAddressAttribute().IsValid(model.Email))
                return ApiResponses.Fail("Geçersiz e-posta adresi.");

            if (DateTime.Now - model.BirthDate < TimeSpan.FromDays(365 * 18))
                return ApiResponses.Fail("Kullanıcı 18 yaşından küçük olamaz.");

            if (model.GenderId != (int)GenderEnum.Male && model.GenderId != (int)GenderEnum.Female)
                return ApiResponses.Fail("Geçersiz cinsiyet seçimi.");





            if (model.Email != user.Email)
            {
                if (await _context.Users.AnyAsync(u => u.Email == model.Email && u.Id != userId))
                    return ApiResponses.Fail("Bu e-posta adresi zaten kullanımda.");
                else
                {
                    //Todo kullanıcının yeni mail adresine mail gönder, ilgili maile onay linki gönder
                    //Eğer onaylanırsa, kullanıcının email adresini güncelle
                    //Eğer onaylanmazsa, eski email adresi ile devam et
                    //Bu işlem için bir EmailService sınıfı oluşturulabilir.
                    return ApiResponses.Fail("Mail adresiniz değiştiği için ilgili değişikliğin onaylanması için mailinizi kontrol ediniz.");

                }

            }
            else
            {
                user.UserName = model.UserName;
                user.GenderId = model.GenderId;
                user.BirthDate = model.BirthDate;

                _context.Users.Update(user);
                await _context.SaveChangesAsync();

                return ApiResponses.Success();

            }



        }

        public async Task<ApiResponses<string>> ChangeUserPasswordAsync(Guid userId, UserSettingsChangePaswordViewModel model)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                return ApiResponses<string>.Fail("Kullanıcı Bulunamadı.");

            if (string.IsNullOrEmpty(model.OldPassword))
                return ApiResponses<string>.Fail("Eski Şifrenizi Giriniz.");
            if (string.IsNullOrEmpty(model.Password))
                return ApiResponses<string>.Fail("Yeni Şifrenizi Giriniz.");
            if (string.IsNullOrEmpty(model.PasswordConfirm))
                return ApiResponses<string>.Fail("Yeni Şifrenizi Tekrardan Giriniz.");
            if (model.Password != model.PasswordConfirm)
                return ApiResponses<string>.Fail("Yeni Şifreniz ve Şifre Tekrarı Aynı Değil");

            var isOldAndNewPasswordSame = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, model.Password);
            if (isOldAndNewPasswordSame == PasswordVerificationResult.Success)
                return ApiResponses<string>.Fail("Yeni Şifreniz Eski Şifreniz İle Aynı Olamaz.");

            var passwordHashedResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, model.OldPassword);
            if (passwordHashedResult == PasswordVerificationResult.Failed)
                return ApiResponses<string>.Fail("Eski Şifrenizi Yanlış Girdiniz.");

            var newPasswordHash = _passwordHasher.HashPassword(user, model.Password);

            user.PasswordHash = newPasswordHash;
            _context.Update(user);
            await _context.SaveChangesAsync();

            var newToken = _authService.GenearteJwtToken(user);

            return ApiResponses<string>.Success(newToken);

        }

        public async Task<ApiResponses> UserDeactiveAsync(Guid userId, UserDeactiveViewModel model)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
                return ApiResponses.Fail("Kullanıcı bulunamadı.");
            if (string.IsNullOrEmpty(model.Password))
                return ApiResponses.Fail("Şifrenizi Giriniz.");
            var passwordResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, model.Password);
            if (passwordResult == PasswordVerificationResult.Failed)
                return ApiResponses.Fail("Şifreniz yanlış.");

            if(user.RoleName == nameof(UserRoleEnum.Admin) || user.RoleName == nameof(UserRoleEnum.SuperAdmin))
                return ApiResponses.Fail("Admin görevindeki kullanıcılar hesabını donduramaz.");
            
            if(user.RoleName == nameof(UserRoleEnum.Moderator))
                return ApiResponses.Fail("Moderator görevindeki kullanıcılar hesabını donduramaz.");

            user.IsDeleted = true;
            _context.Update(user);
            await _context.SaveChangesAsync();
            return ApiResponses.Success();
        }
    }
}
