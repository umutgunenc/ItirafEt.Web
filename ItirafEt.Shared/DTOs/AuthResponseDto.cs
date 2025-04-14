using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ItirafEt.Shared.DTOs
{
    public record AuthResponseDto(LoggedInUser? User, string? ErrorMessage = null)
    {
        [JsonIgnore]
        public bool HasError => !string.IsNullOrEmpty(ErrorMessage);

    }

}
