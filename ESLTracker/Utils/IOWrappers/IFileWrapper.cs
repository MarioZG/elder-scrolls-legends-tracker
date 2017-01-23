using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESLTracker.Utils.IOWrappers
{
    public interface IFileWrapper
    {
        void Delete(string s);
        void Move(string sourceFileName, string destFileName);
        void WriteAllText(string path, string contents);
        bool Exists(string path);
    }
}
