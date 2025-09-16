using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ItirafEt.Api.BackgorunServices.RabbitMQ;
using ItirafEt.Api.ConstStrings;
using ItirafEt.Api.Data;
using ItirafEt.Api.Data.Entities;
using ItirafEt.Api.EmailServices;
using ItirafEt.Shared;
using ItirafEt.Shared.Enums;
using ItirafEt.Shared.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace ItirafEt.Api.Services
{
    public class AuthService
    {
        private readonly dbContext _context;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IConfiguration _configuration;
        private readonly EmailSenderProducer _emailSenderProducer;

        public AuthService(dbContext context, IPasswordHasher<User> passwordHasher, IConfiguration configuration, IEmailSender emailSender, EmailSenderProducer emailSenderProducer)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _configuration = configuration;
            _emailSenderProducer = emailSenderProducer;
        }
        public async Task<AuthResponse> LoginAsync(LoginViewModel model)
        {
            model.EmailOrUserName = model.EmailOrUserName.Trim();

            var user = await _context.Users
                .AsNoTracking()
                .Where(u => u.Email.ToLower() == model.EmailOrUserName.ToLower() || u.UserName.ToLower() == model.EmailOrUserName.ToLower())
                .Select(u => new User
                {
                    Id = u.Id,
                    UserName = u.UserName,
                    Email = u.Email,
                    PasswordHash = u.PasswordHash,
                    IsDeleted = u.IsDeleted,
                    IsBanned = u.IsBanned,
                    IsTermOfUse = u.IsTermOfUse,
                    RoleName = u.RoleName,
                    BannedDateUntil = u.BannedDateUntil,
                    UserLoginAttempts = u.UserLoginAttempts
                        .OrderByDescending(ula => ula.AttemptDate)
                        .Take(5)
                        .ToList()
                })
                .FirstOrDefaultAsync();

            if (user == null)
                return new AuthResponse(default, "Girdiğiniz kullanıcı adı, e-posta veya şifre hatalı.");

            if (user.IsDeleted)
                return new AuthResponse(default, "Hesabınız aktif durumda değil.");

            if (user.IsBanned)
                return new AuthResponse(default, $"Hesabınız {user.BannedDateUntil?.ToString("dd/MM/yyyy")} tarihine kadar banlanmıştır.");

            if (user.UserLoginAttempts.Count == 5)
            {

                var lastFiveAttempts = user.UserLoginAttempts;
                if (lastFiveAttempts.All(la => !la.IsSuccessful))
                {
                    var lastFailDate = lastFiveAttempts.Max(a => a.AttemptDate);

                    var lastPasswordReset = await _context.PasswordResetTokens
                        .Where(prt => prt.UserId == user.Id && prt.IsUsed)
                        .OrderByDescending(prt => prt.Id)
                        .FirstOrDefaultAsync();

                    if (lastPasswordReset == null || lastPasswordReset.ExpTime < lastFailDate)
                    {
                        await _emailSenderProducer.PublishAsync(EmailTypes.AccountBlocked, EmailCreateFactory.CreateEmail(EmailTypes.AccountBlocked, user));

                        return new AuthResponse(default, "Şifrenizi üst üste 5 kere yanlış girdiğiniz için hesabınız kitlenmiştir.\n Lütfen şifremi unuttum seçeneğini kullanarak şifrenizi yenileyiniz.");
                    }
                }
            }

            var passwordResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, model.Password);

            var loginAttempt = new UserLoginAttempt
            {
                UserId = user.Id,
                AttemptDate = DateTime.UtcNow,
                IpAddress = model.IpAddress,
                DeviceInfo = model.DeviceInfo
            };



            if (passwordResult == PasswordVerificationResult.Failed)
            {
                loginAttempt.IsSuccessful = false;
                await _context.UserLoginAttempts.AddAsync(loginAttempt);
                await _context.SaveChangesAsync();
                return new AuthResponse(default, "Girdiğiniz kullanıcı adı, e-posta veya şifre hatalı.");
            }



            loginAttempt.IsSuccessful = true;
            var jwtToken = GenearteJwtToken(user);
            var loggedInUser = new LoggedInUser(user.Id.ToString(), user.UserName, user.RoleName.ToString(), jwtToken);

            await _context.UserLoginAttempts.AddAsync(loginAttempt);
            await _context.SaveChangesAsync();

            return new AuthResponse(loggedInUser);

        }

        public async Task<ApiResponses> RegisterAsync(RegisterViewModel model)
        {
            model.Email = model.Email.Trim();
            model.UserName = model.UserName.Trim();

            var isUserNameNotValid = await _context.Users.AnyAsync(u => u.UserName.ToUpper() == model.UserName.ToUpper());
            if (isUserNameNotValid)
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
                CreatedDate = DateTime.UtcNow,
                BirthDate = (DateTime)model.BirthDate,
                IsDeleted = false,
                IsBanned = false,
                IsProfilePrivate = false,
                IsTermOfUse = true,
                RoleName = nameof(UserRoleEnum.User),
                GenderId = (int)model.GenderId,
            };
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            await _emailSenderProducer.PublishAsync(EmailTypes.Welcome, EmailCreateFactory.CreateEmail(EmailTypes.Welcome, user));
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

            if (dto.BirthDate > DateTime.UtcNow.AddYears(-18))
                return ApiResponses.Fail("18 yaşından küçük olamazsınız.");

            if (dto.BirthDate < DateTime.UtcNow.AddYears(-100))
                return ApiResponses.Fail("100 yaşından büyük olamazsınız.");

            if (dto.BirthDate == null)
                return ApiResponses.Fail("Doğum tarihi boş olamaz.");

            if (dto.GenderId == null)
                return ApiResponses.Fail("Lütfen Cinsiyet Seçiniz.");

            if (dto.GenderId != (int)GenderEnum.Male && dto.GenderId != (int)GenderEnum.Female)
                return ApiResponses.Fail("Lütfen Cinsiyet Seçiniz.");


            return ApiResponses.Success();
        }

        public async Task<ApiResponses> CreatePasswordResetTokenAsync(ForgotPaswordViewModel model)
        {
            model.UserNameOrEmailAdres = model.UserNameOrEmailAdres.Trim();
            var user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.UserName.ToLower() == model.UserNameOrEmailAdres.ToLower() || u.Email.ToLower() == model.UserNameOrEmailAdres.ToLower());

            if (user == null)
                return ApiResponses.Fail("Kullanıcı bulunamadı.");

            if (user.IsDeleted)
                return ApiResponses.Fail("Hesabınız aktif durumda değil.");

            var token = PasswordResetTokenHelperService.GenerateRawToken();
            var tokenHash = PasswordResetTokenHelperService.HashToken(token);

            var passwordResetToken = new PasswordResetToken
            {
                UserId = user.Id,
                TokenHash = tokenHash,
                ExpTime = DateTime.UtcNow.AddMinutes(30),
                IsUsed = false,
                DeviceInfo = model.DeviceInfo,
                IpAddress = model.IpAddress
            };

            var baseUrl = _configuration["Jwt:Issuer"];
            var resetLink = $"{baseUrl}/reset-password?userId={user.Id}&token={token}";

            await _context.PasswordResetTokens.AddAsync(passwordResetToken);
            await _context.SaveChangesAsync();


            await _emailSenderProducer.PublishAsync(EmailTypes.Reset, EmailCreateFactory.CreateEmail(EmailTypes.Reset, user, resetLink));

            return ApiResponses.Success();

        }

        public async Task<ApiResponses> ChangeUserPasswordAsync(ChangeUserPasswordViewModel model)
        {

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == model.UserId);

            if (user == null)
                return ApiResponses.Fail("Kullanıcı bulunamadı.");
            if (model.NewPassword != model.NewPasswordConfirm)
                return ApiResponses.Fail("Şifreler eşleşmiyor.");
            if (model.NewPassword.Length < 8)
                return ApiResponses.Fail("Şifre en az 8 karakter uzunluğunda olmalıdır.");
            if (model.NewPassword.Length > 64)
                return ApiResponses.Fail("Şifre en fazla 64 karakter uzunluğunda olmalıdır.");
            if (!model.NewPassword.Any(char.IsUpper))
                return ApiResponses.Fail("Şifre en az 1 büyük harf içermelidir.");
            if (!model.NewPassword.Any(char.IsLower))
                return ApiResponses.Fail("Şifre en az 1 küçük harf içermelidir.");
            if (!model.NewPassword.Any(char.IsDigit))
                return ApiResponses.Fail("Şifre en az 1 rakam içermelidir.");
            if (!model.NewPassword.Any(ch => !char.IsLetterOrDigit(ch)))
                return ApiResponses.Fail("Şifre en az 1 özel karakter içermelidir.");

            var passwordResetToken = await _context.PasswordResetTokens
                .Where(prt => prt.UserId == model.UserId && !prt.IsUsed)
                .OrderByDescending(prt => prt.Id)
                .FirstOrDefaultAsync();

            if (passwordResetToken == null)
                return ApiResponses.Fail("Şifre sıfırlama talebiniz bulunamadı. Lütfen yeniden 'Şifremi Unuttum' sayfasından talepte bulunun.");

            if (passwordResetToken.ExpTime < DateTime.UtcNow)
                return ApiResponses.Fail("Şifre sıfırlama linkiniz süresi dolmuş. Lütfen yeni bir talep oluşturun.");

            bool isTokenValid = PasswordResetTokenHelperService.VerifyToken(model.Token, passwordResetToken.TokenHash);

            if (!isTokenValid)
                return ApiResponses.Fail("Şifre sıfırlama linkiniz geçersiz. Lütfen tekrar talep oluşturun.");

            passwordResetToken.IsUsed = true;
            user.PasswordHash = _passwordHasher.HashPassword(user, model.NewPassword);
            await _context.SaveChangesAsync();

            return ApiResponses.Success();

        }

    }
}
