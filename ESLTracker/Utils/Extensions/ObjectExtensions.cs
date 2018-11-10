using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESLTracker.Utils.Extensions
{
    public static class ObjectExtensions
    {
        public static T ConvertToEnum<T>(this object data)
            where T : struct, IConvertible
        {
            return (T)Enum.Parse(typeof(T), data?.ToString());
        }

        public static int ConvertToInt(this object data)
        {
            return int.Parse(data?.ToString());
        }
    }
}
