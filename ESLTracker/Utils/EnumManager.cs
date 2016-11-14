using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESLTracker.Utils
{
    public class EnumManager
    {
        public static  T ParseEnumString<T>(string value) 
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }

    }
}
