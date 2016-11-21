using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESLTracker.Utils.IOWrappers
{
    class PathWrapper : IPathWrapper
    {
        public string Combine(string path1, string path2)
        {
            return System.IO.Path.Combine(path1, path2);
        }

        public string GetFileNameWithoutExtension(string path)
        {
            return System.IO.Path.GetFileNameWithoutExtension(path);
        }

        public string GetDirectoryName(string path)
        {
            return System.IO.Path.GetDirectoryName(path);
        }

        public string ChangeExtension(string path, string extension)
        {
            return System.IO.Path.ChangeExtension(path, extension);
        }

        public string GetExtension(string path)
        {
            return System.IO.Path.GetExtension(path);
        }
    }
}
