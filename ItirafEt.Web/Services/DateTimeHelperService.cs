using ItirafEt.Shared.Services;
using Microsoft.JSInterop;
using TimeZoneConverter;

namespace ItirafEt.Web.Services
{
    public class DateTimeHelperService : IDateTimeHelperService
    {
        private readonly IJSRuntime _js;

        public DateTimeHelperService(IJSRuntime js)
        {
            _js = js;
        }
        public async Task<string> GetUserTimeZoneIdAsync()
        {
            var timzeZoneID = await _js.InvokeAsync<string>("dateTimeHelper.getTimeZone");
            return timzeZoneID;
        }

        public DateTime ToLocal(DateTime utc, string timeZoneId)
        {
            var tz = TZConvert.GetTimeZoneInfo(timeZoneId);
            var local = TimeZoneInfo.ConvertTimeFromUtc(DateTime.SpecifyKind(utc, DateTimeKind.Utc), tz);
            return local;

        }

        public DateTime ToUtc(DateTime localTime, string timeZoneId)
        {
            var tz = TZConvert.GetTimeZoneInfo(timeZoneId);
            var utc = TimeZoneInfo.ConvertTimeToUtc(localTime, tz);
            return utc;
        }


    }
}
