using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItirafEt.Shared.ClientServices
{
    public interface IScrollHelper
    {
        ValueTask SetScrollYAsync(double y);
        ValueTask<double> GetScrollYAsync();
    }
}
