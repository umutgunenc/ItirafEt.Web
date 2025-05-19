using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItirafEt.Shared.ClientServices.State
{
    public class InfiniteScrollState<TItem>
    {
        //public List<TItem> Items { get; set; } = new();
        //public int CurrentPage { get; set; } = 1;
        //public bool HasMore { get; set; } = true;
        //public double ScrollY { get; set; } = 0;

        public int CategoryId { get; set; }
        public List<TItem> Items { get; set; } = new();
        public int CurrentPage { get; set; } = 1;
        public bool HasMore { get; set; } = true;
        public double ScrollY { get; set; }
        public string OrderBy { get; set; } = "date";
    }
}
