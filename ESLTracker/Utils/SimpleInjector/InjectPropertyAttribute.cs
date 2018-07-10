using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESLTracker.Utils.SimpleInjector
{
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    class InjectPropertyAttribute : Attribute
    {
    }
}
