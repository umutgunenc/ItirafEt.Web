using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItirafEt.Shared.DTOs
{
    public record ApiResponse(bool IsSuccess, string? id, string? ErrorMessage)
    {
        public static ApiResponse Success() => new ApiResponse(true, null, null);
        public static ApiResponse Success(string id) => new ApiResponse(true, id, null);
        public static ApiResponse Fail(string errorMessage) => new ApiResponse(false, null, errorMessage);

    }

    public record ApiResponse<T>(bool IsSuccess, T? Data, string? ErrorMessage, bool? isUpdated)
    {
        public static ApiResponse<T> Success(T data, bool _bool) => new(true, data, null, _bool);
        public static ApiResponse<T> Fail(string errorMessage) => new(false, default, errorMessage,null);
    }

}
