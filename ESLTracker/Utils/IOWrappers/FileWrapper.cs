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
    }
}
