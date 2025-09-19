using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ItirafEt.SharedComponents.Services;

namespace ItirafEt.Mobile.Services
{
    public class DeviceService : IDeviceService
    {
        public string Platform => DeviceInfo.Current.Platform.ToString();

        public Task<bool> IsMobileAsync()
        {
            bool isMobile = DeviceInfo.Current.Platform == DevicePlatform.Android ||
                            DeviceInfo.Current.Platform == DevicePlatform.iOS;
            return Task.FromResult(isMobile);
        }
    }
}
