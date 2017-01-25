using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ESLTracker.Utils
{
    public class ApplicationService : IApplicationService
    {
        public SerializableVersion GetAssemblyVersion()
        {
            return new SerializableVersion(new Version(GitVersionInformation.AssemblySemVer));
        }

        public SerializableVersion GetAssemblyInformationalVersion()
        {
            return new SerializableVersion(new Version(GitVersionInformation.InformationalVersion));
        }
    }
}
