using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ItirafEt.Shared.Services;
using TimeZoneConverter;

namespace ItirafEt.Mobile.Services
{
    public class DateTimeHelperService : IDateTimeHelperService
    {
        public Task<string> GetUserTimeZoneIdAsync()
        {
            var timeZoneId = TimeZoneInfo.Local.Id;

            return Task.FromResult(timeZoneId);
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
