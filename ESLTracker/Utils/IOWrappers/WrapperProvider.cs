using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESLTracker.Utils.IOWrappers
{
    public class WrapperProvider : IWrapperProvider
    {
        static Dictionary<Type, object> Wrappers = new Dictionary<Type, object>();

        public object GetWrapper(Type type)
        {
            if (! Wrappers.ContainsKey(type))
            {
                if (type == typeof(IDirectoryWrapper)) {
                    Wrappers.Add(type, new DirectoryWrapper());
                }
                else if (type == typeof(IFileWrapper))
                {
                    Wrappers.Add(type, new FileWrapper());
                }
                else if (type == typeof(IPathWrapper))
                {
                    Wrappers.Add(type, new PathWrapper());
                }
                else
                {
                    throw new NotImplementedException("Unknown interface " + type.FullName);
                }
            }
            return Wrappers[type];
        }
    }
}
