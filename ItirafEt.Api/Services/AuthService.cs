using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ItirafEt.Api.Data;
using ItirafEt.Api.Data.Entities;
using ItirafEt.Shared;
using ItirafEt.Shared.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace ItirafEt.Api.Services
{
    public class AuthService
    {
        private readonly Context _context;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IConfiguration _configuration;

        public AuthService(Context context, IPasswordHasher<User> passwordHasher, IConfiguration configuration)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _configuration = configuration;
        }
        public async Task<AuthResponseDto> LoggingAsync(LoginDto dto)
        {
            var user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.UserName == dto.UserName);

            if (user == null)
                return new AuthResponseDto(default,"Kullanıcı Adı veya Şifre Hatalı");

            var passwordResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);

            if (passwordResult == PasswordVerificationResult.Failed)
                return new AuthResponseDto(default, "Kullanıcı Adı veya Şifre Hatalı");

            var jwtToken = GenearteJwtToken(user);
            var loggedInUser = new LoggedInUser(user.Id.ToString(),user.UserName,user.RoleId.ToString(), jwtToken);
            return new AuthResponseDto(loggedInUser);

        }

        private string GenearteJwtToken(User user)
        {
            Claim[] claims = [
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Role, user.RoleId.ToString())
                ];

            var secretKey = _configuration.GetValue<string>("Jwt:Secret");
            var symmetricKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var signingCredentials = new SigningCredentials(symmetricKey, SecurityAlgorithms.HmacSha256);

            //TODO : expiration time i degistir arastir

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _configuration.GetValue<string>("Jwt:Issuer"),
                audience: _configuration.GetValue<string>("Jwt:Audience"),
                claims: claims,
                expires: DateTime.Now.AddMinutes(_configuration.GetValue<int>("Jwt:ExpiryInMinutes")),
                signingCredentials: signingCredentials);

            var token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);

            return token;
        }
    }
}
