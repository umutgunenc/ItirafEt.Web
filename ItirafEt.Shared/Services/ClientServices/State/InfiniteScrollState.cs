using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItirafEt.Shared.Services.ClientServices.State
{
    public class InfiniteScrollState<TItem>
    {
        public List<TItem>? Items { get; set; } = new();
        public int CurrentPage { get; set; } = 1;
        public bool HasMore { get; set; } = true;
        public double ScrollY { get; set; } = 0;
        public DateTime? NextBefore { get; set; }
        public int? LastId { get; set; }
        public int Take { get; set; } = PageItemSize.Size;
    }
}
