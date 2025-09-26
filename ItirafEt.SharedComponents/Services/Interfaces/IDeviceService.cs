using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItirafEt.SharedComponents.Services
{
    public interface IDeviceService
    {
        bool IsMobile { get; }
        Task InitializeAsync();
    }
}
