using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItirafEt.Shared.Services
{
    public interface IDateTimeHelperService
    {
        Task<string> GetUserTimeZoneIdAsync();

        DateTime ToLocal(DateTime utc,string timeZoneId);
        DateTime ToUtc(DateTime dateTime,string timzeZoneId);

    }
}
