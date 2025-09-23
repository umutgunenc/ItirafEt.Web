using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace ItirafEt.Api.HelperServices
{
    public class SignedUrl
    {
        private readonly string _secret;
        private readonly int _expirySeconds = 5;
        public SignedUrl(IConfiguration configuration)
        {
            _secret = configuration["SignedUrl:Secret"] ?? Guid.NewGuid().ToString();
        }

        public string GenerateThumbnailUrl(string thumbnailId, string conversationId)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim("thumbnailId", thumbnailId),
                new Claim("conversationId", conversationId)
            };

            var token = new JwtSecurityToken(claims: claims, expires: DateTime.UtcNow.AddSeconds(_expirySeconds), signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GeneratePhotoUrl(string photoId, string conversationId)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim("photoId", photoId),
                new Claim("conversationId", conversationId)
            };

            var token = new JwtSecurityToken(claims: claims, expires: DateTime.UtcNow.AddSeconds(_expirySeconds), signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        public JwtSecurityToken? ValidateToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var validationParams = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secret)),
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromSeconds(5) // küçük tolerans
            };

            try
            {
                handler.ValidateToken(token, validationParams, out var validatedToken);

                return (JwtSecurityToken)validatedToken;
            }
            catch
            {
                return null;
            }
        }
    }
}
