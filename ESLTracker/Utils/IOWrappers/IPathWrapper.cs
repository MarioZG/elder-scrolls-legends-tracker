using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESLTracker.Utils.IOWrappers
{
    public interface IPathWrapper
    {
        string Combine(string path1, string path2);
        string GetFileNameWithoutExtension(string path);
        string GetDirectoryName(string path);
        string ChangeExtension(string path, string extension);
        string GetExtension(string path);
    }
}
