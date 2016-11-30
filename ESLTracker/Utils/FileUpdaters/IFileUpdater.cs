using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESLTracker.Utils.FileUpdaters
{
    public interface IFileUpdater
    {
        SerializableVersion TargetVersion { get; }
        bool UpdateFile();
    }
}
