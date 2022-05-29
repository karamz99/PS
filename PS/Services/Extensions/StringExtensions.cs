using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace PS
{
    public static class StringExtensions
    {
        public static string GetDisplayName<TItem>(this string propertyName)
        {
            MemberInfo myProp = typeof(TItem).GetProperty(propertyName) as MemberInfo;
            var dd = myProp.GetCustomAttribute(typeof(DisplayAttribute)) as DisplayAttribute;
            return dd?.Name ?? "";
        }
    }
}
