using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ItirafEt.Shared
{
    public record LoggedInUser(string id, string userName, string roleId, string Token)
    {
        public string ToJson() => JsonSerializer.Serialize(this);

        public List<Claim> ToClaims() => new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, id.ToString()),
            new Claim(ClaimTypes.Name, userName),
            new Claim(ClaimTypes.Role, roleId.ToString()),
            new Claim(nameof(Token),Token)
        };

        public static LoggedInUser? FromJson(string json) => string.IsNullOrWhiteSpace(json) ? null : JsonSerializer.Deserialize<LoggedInUser>(json);


    }

}
