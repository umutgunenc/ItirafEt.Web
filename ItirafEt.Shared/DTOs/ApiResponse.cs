using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItirafEt.Shared.DTOs
{
    public record ApiResponses(bool IsSuccess, string? id, string? ErrorMessage)
    {
        public static ApiResponses Success() => new ApiResponses(true, null, null);
        public static ApiResponses Success(string id) => new ApiResponses(true, id, null);
        public static ApiResponses Fail(string errorMessage) => new ApiResponses(false, null, errorMessage);
    }

    public record ApiResponses<T>(bool IsSuccess, T? Data, string? ErrorMessage, bool? isUpdated)
    {
        public static ApiResponses<T> Success(T data) => new(true, data, null, null);
        public static ApiResponses<T> Success(T data, bool? IsUpdated) => new(true, data, null, IsUpdated);
        public static ApiResponses<T> Fail(string errorMessage) => new(false, default, errorMessage,null);
    }

}
