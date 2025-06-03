using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItirafEt.Shared.ViewModels
{
    public record ApiResponses(bool IsSuccess, string? ErrorMessage)
    {
        public static ApiResponses Success() => new ApiResponses(true, null);
        public static ApiResponses Fail(string errorMessage) => new ApiResponses(false, errorMessage);
    }

    public record ApiResponses<T>(bool IsSuccess, T? Data, string? ErrorMessage, bool? isUpdated)
    {
        public static ApiResponses<T> Success(T data) => new(true, data, null, null);
        public static ApiResponses<T> Success(T data, bool? IsUpdated) => new(true, data, null, IsUpdated);
        public static ApiResponses<T> Fail(string errorMessage) => new(false, default, errorMessage, null);

    }

}
