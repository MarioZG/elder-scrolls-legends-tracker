using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESLTracker.Utils.DrawingWrappers;

namespace ESLTracker.Utils.IOWrappers
{
    public class WrapperProvider : IWrapperProvider
    {
        private static IWrapperProvider _instance;
        [Obsolete("Use tracker factory to obtain instance")]
        public static IWrapperProvider Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new WrapperProvider();
                }
                return _instance;
            }
        }

        private WrapperProvider()
        {

        }

        static Dictionary<Type, object> Wrappers = new Dictionary<Type, object>();

        public T GetWrapper<T>() //where T: object
        {
            Type type = typeof(T);
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
                else if (type == typeof(IBitmapWrapper))
                {
                    Wrappers.Add(type, new BitmapWrapper());
                }
                else
                {
                    throw new NotImplementedException("Unknown interface " + type.FullName);
                }
            }
            return (T)Wrappers[type];// as T;
        }
    }
}
