using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ItirafEt.Shared.Enums
{
    public static class EnumService
    {
        public static string GetDisplayName(Enum enumValue)
        {
            var member = enumValue.GetType().GetMember(enumValue.ToString()).First();
            var display = member.GetCustomAttribute<DisplayAttribute>();
            return display?.Name ?? enumValue.ToString();
        }
    }
}
