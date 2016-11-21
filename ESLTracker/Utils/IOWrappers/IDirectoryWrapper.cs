using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESLTracker.Utils.IOWrappers
{
    public interface IDirectoryWrapper
    {
        IEnumerable<string> EnumerateFiles(string path, string searchPattern);
    }
}
