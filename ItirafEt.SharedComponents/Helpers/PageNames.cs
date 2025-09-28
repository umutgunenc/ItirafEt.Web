using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItirafEt.SharedComponents.Helpers
{
    public static class PageNameConstants
    {

        public const string Category = "Category";
        public const string Post = "Post";
        public const string Conversation = "Conversation";
        public const string Layout = "Layout";


        public enum PageType
        {
            Category,
            Post,
            Conversation,
            Layout
        }

        public static string GetPageName(PageType pageType)
        {
            return pageType switch
            {
                PageType.Category => Category,
                PageType.Post => Post,
                PageType.Conversation => Conversation,
                PageType.Layout => Layout,
                _ => throw new ArgumentOutOfRangeException(nameof(PageType), pageType, null)
            };
        }
    }
}
