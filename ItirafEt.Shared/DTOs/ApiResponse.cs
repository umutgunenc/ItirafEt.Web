using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItirafEt.Shared.DTOs
{
    public record ApiResponse(bool IsSuccess, string? ErrorMessage)
    {
        public static ApiResponse Success() => new ApiResponse(true, null);
        public static ApiResponse Fail(string errorMessage) => new ApiResponse(false, errorMessage);

    }

}
