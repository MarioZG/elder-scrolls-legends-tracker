using System;
using System.Linq;
using System.Reflection;
using SimpleInjector.Advanced;

namespace ESLTracker.Utils.SimpleInjector
{
    class ImportPropertySelectionBehavior : IPropertySelectionBehavior
    {
        public bool SelectProperty(Type implementationType, PropertyInfo prop) =>
            prop.GetCustomAttributes(typeof(InjectPropertyAttribute)).Any();
    }
}