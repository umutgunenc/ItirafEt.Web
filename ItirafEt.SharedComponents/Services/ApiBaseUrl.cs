using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItirafEt.SharedComponents.Services
{
    public static class ApiBaseUrl
    {
        public static string BaseUrl { get; } = "https://localhost:7292";
        public static string AndroidBaseUrl { get; } = "https://10.0.2.2:7292";
        public static string AndroidBaseUrlHttp { get; } = "http://10.0.2.2:7292";

    }
}
