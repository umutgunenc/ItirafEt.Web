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
        //public string Platform => DeviceInfo.Current.Platform.ToString();

        //public Task<bool> IsMobileAsync()
        //{
        //    bool isMobile = DeviceInfo.Current.Platform == DevicePlatform.Android ||
        //                    DeviceInfo.Current.Platform == DevicePlatform.iOS;
        //    return Task.FromResult(isMobile);
        //}

        private bool _initialized;
        private bool _isMobile;

        public bool IsMobile => _isMobile;

        public Task InitializeAsync()
        {
            if (_initialized)
                return Task.CompletedTask;

            var platform = DeviceInfo.Current.Platform;

            _isMobile = platform == DevicePlatform.Android || platform == DevicePlatform.iOS;

            _initialized = true;

            return Task.CompletedTask;
        }


    }
}
