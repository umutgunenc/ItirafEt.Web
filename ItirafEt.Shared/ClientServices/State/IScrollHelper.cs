using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItirafEt.Shared.ClientServices.State
{
    public interface IScrollHelper
    {
        ValueTask SetScrollYAsync(double y);
    }
}
