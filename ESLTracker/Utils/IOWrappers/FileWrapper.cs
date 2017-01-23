using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESLTracker.Utils.IOWrappers
{
    public class FileWrapper : IFileWrapper
    {
        public void Delete(string path)
        {
            System.IO.File.Delete(path);
        }

        public void Move(string sourceFileName, string destFileName)
        {
            System.IO.File.Move(sourceFileName, destFileName);
        }

        public void WriteAllText(string path, string contents)
        {
            System.IO.File.WriteAllText(path, contents);
        }

        public bool Exists(string path)
        {
            return System.IO.File.Exists(path);
        }
    }
}
