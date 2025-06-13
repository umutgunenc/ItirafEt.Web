using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ItirafEt.Api.Data;
using ItirafEt.Api.Data.Entities;
using ItirafEt.Shared;
using ItirafEt.Shared.Enums;
using ItirafEt.Shared.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace ItirafEt.Api.Services
{
    public class AuthService
    {
        private readonly dbContext _context;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IConfiguration _configuration;

        public AuthService(dbContext context, IPasswordHasher<User> passwordHasher, IConfiguration configuration)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _configuration = configuration;
        }
        public async Task<AuthResponse> LoggingAsync(LoginViewModel model)
        {
            var user = await _context.Users
                .AsNoTracking()
                .Where(u => u.UserName == model.UserName)
                .Select(u => new User
                {
                    Id = u.Id,
                    UserName = u.UserName,
                    PasswordHash = u.PasswordHash,
                    IsDeleted = u.IsDeleted,
                    IsBanned = u.IsBanned,
                    IsTermOfUse = u.IsTermOfUse,
                    RoleName = u.RoleName,
                    BannedDateUntil = u.BannedDateUntil,
                })
                .FirstOrDefaultAsync();

            if (user == null)
                return new AuthResponse(default,"Kullanıcı Adı veya Şifre Hatalı");

            if (user.IsDeleted)
                return new AuthResponse(default, "Hesabınız aktif durumda değil.");

            if (user.IsBanned)
                return new AuthResponse(default, $"Hesabınız {user.BannedDateUntil?.ToString("dd/MM/yyyy")} tarihine kadar banlanmıştır.");

            var passwordResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, model.Password);

            if (passwordResult == PasswordVerificationResult.Failed)
                return new AuthResponse(default, "Kullanıcı Adı veya Şifre Hatalı");

            var jwtToken = GenearteJwtToken(user);
            var loggedInUser = new LoggedInUser(user.Id.ToString(),user.UserName,user.RoleName.ToString(), jwtToken);
            return new AuthResponse(loggedInUser);

        }

        public async Task<ApiResponses> RegisterAsync(RegisterViewModel model)
        {
            var isUserNameNotValid = await _context.Users.AnyAsync(u => u.UserName.ToUpper() == model.UserName.ToUpper());
            if(isUserNameNotValid)
                return ApiResponses.Fail("Bu kullanıcı adı zaten kullanılıyor.");
            var isEmailNotValid = await _context.Users.AnyAsync(u => u.Email.ToUpper() == model.Email.ToUpper());
            if (isEmailNotValid)
                return ApiResponses.Fail("Bu e-posta adresi zaten kullanılıyor.");

           var isModelValid = CheckRegisterModel(model);
            if (!isModelValid.IsSuccess)
                return isModelValid;

            var user = new User
            {
                UserName = model.UserName,
                Email = model.Email,
                PasswordHash = _passwordHasher.HashPassword(null, model.Password),
                CreatedDate = DateTime.Now,
                BirthDate = (DateTime)model.BirthDate,
                IsDeleted = false,
                IsBanned = false,
                IsPremium = false,
                IsTermOfUse = true,
                RoleName = nameof(UserRoleEnum.User),
                GenderId = (int)model.GenderId,

            };
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return ApiResponses.Success();


        }

        public string GenearteJwtToken(User user)
        {
            Claim[] claims = [
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Role, user.RoleName)
                ];

            var secretKey = _configuration.GetValue<string>("Jwt:Secret");
            var symmetricKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var signingCredentials = new SigningCredentials(symmetricKey, SecurityAlgorithms.HmacSha256);

            //TODO : expiration time i degistir arastir

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _configuration.GetValue<string>("Jwt:Issuer"),
                audience: _configuration.GetValue<string>("Jwt:Audience"),
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_configuration.GetValue<int>("Jwt:ExpiryInMinutes")),
                signingCredentials: signingCredentials);

            var token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);

            return token;
        }

        private ApiResponses CheckRegisterModel(RegisterViewModel dto)
        {
            if (string.IsNullOrWhiteSpace(dto.UserName))
                return ApiResponses.Fail("Kullanıcı adı boş olamaz.");

            if (string.IsNullOrWhiteSpace(dto.Password))
                return ApiResponses.Fail("Şifre boş olamaz.");

            if (string.IsNullOrWhiteSpace(dto.PasswordConfirm))
                return ApiResponses.Fail("Şifre tekrarı boş olamaz.");

            if (!dto.Password.Any(char.IsUpper))
                return ApiResponses.Fail("Şifre en az 1 büyük harf içermelidir.");

            if (!dto.Password.Any(char.IsLower))
                return ApiResponses.Fail("Şifre en az 1 küçük harf içermelidir.");

            if (!dto.Password.Any(char.IsDigit))
                return ApiResponses.Fail("Şifre en az 1 rakam içermelidir.");

            if (!dto.Password.Any(ch => !char.IsLetterOrDigit(ch)))
                return ApiResponses.Fail("Şifre en az 1 özel karakter içermelidir.");

            if (dto.Password.Length < 8)
                return ApiResponses.Fail("Şifre en az 6 karakter uzunluğunda olmalıdır.");

            if (dto.Password.Length > 64)
                return ApiResponses.Fail("Şifre en fazla 64 karakter uzunluğunda olmalıdır.");

            if (dto.Password != dto.PasswordConfirm)
                return ApiResponses.Fail("Şifreler eşleşmiyor.");

            if (dto.UserName.Length < 3)
                return ApiResponses.Fail("Kullanıcı adı en az 3 karakter uzunluğunda olmalıdır.");
            if (dto.UserName.Length > 64)
                return ApiResponses.Fail("Kullanıcı adı en fazla 64 karakter uzunluğunda olmalıdır.");

            if (string.IsNullOrWhiteSpace(dto.Email))
                return ApiResponses.Fail("E-posta adresi boş olamaz.");

            if (dto.Password != dto.PasswordConfirm)
                return ApiResponses.Fail("Şifreler eşleşmiyor.");

            if (!dto.isTermsAccepted)
                return ApiResponses.Fail("Kullanım Koşulları ve Gizlilik Politikasını kabul etmelisiniz.");

            if(dto.BirthDate > DateTime.Now.AddYears(-18))
                return ApiResponses.Fail("18 yaşından küçük olamazsınız.");

            if (dto.BirthDate < DateTime.Now.AddYears(-100))
                return ApiResponses.Fail("100 yaşından büyük olamazsınız.");
            
            if(dto.BirthDate == null)
                return ApiResponses.Fail("Doğum tarihi boş olamaz.");

            if(dto.GenderId == null)
                return ApiResponses.Fail("Lütfen Cinsiyet Seçiniz.");

            if (dto.GenderId != (int)GenderEnum.Male && dto.GenderId != (int)GenderEnum.Female)
                return ApiResponses.Fail("Lütfen Cinsiyet Seçiniz.");


            return ApiResponses.Success();
        }
    }
}
